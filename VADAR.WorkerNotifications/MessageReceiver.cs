using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Service.Interfaces;

namespace VADAR.WorkerNotifications
{
    public class MessageReceiver : BackgroundService
    {
        private readonly IModel _channel;
        private readonly ILogger<MessageReceiver> logger;
        private readonly IConfiguration configuration;
        private readonly IElasticSearchCallApiHelper elasticSearchCallApiHelper;
        private readonly IEmailService emailService;
        private readonly ISlackBotMessagesHelper slackBotMessagesHelper;
        private readonly ITelegramHelper telegramHelper;
        private readonly IWorkerNotificationService workerNotificationService;

        public MessageReceiver(
            ILogger<MessageReceiver> logger,
            IConfiguration configuration,
            IElasticSearchCallApiHelper elasticSearchCallApiHelper,
            IEmailService emailService,
            ISlackBotMessagesHelper slackBotMessagesHelper,
            ITelegramHelper telegramHelper,
            IWorkerNotificationService workerNotificationService)
        {
            this.elasticSearchCallApiHelper = elasticSearchCallApiHelper;
            this.emailService = emailService;
            this.slackBotMessagesHelper = slackBotMessagesHelper;
            this.telegramHelper = telegramHelper;
            this.workerNotificationService = workerNotificationService;
            this.logger = logger;
            this.configuration = configuration;
            var factory = new ConnectionFactory() { 
                HostName = this.configuration["RabbitMQ:HostName"],
                UserName = this.configuration["RabbitMQ:UserName"],
                Password = this.configuration["RabbitMQ:Password"],
                VirtualHost = "/",
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: this.configuration["RabbitMQ:QueueName"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel = channel;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            var counter = 0;

            consumer.Received += async (ch, ea) =>
            {
                counter++;
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                this.logger.LogInformation($"Received: {content}");
                this.logger.LogInformation($"counter: {counter}");
                RabbitMQRequestDto dataRequest = null;
                try
                {
                    dataRequest = JsonSerializer.Deserialize<RabbitMQRequestDto>(content);
                }
                catch
                {
                    _ = await this.elasticSearchCallApiHelper.CreateNotificationError(content);
                }

                if (dataRequest != null)
                {
                    var emailContent = string.Empty;
                    if (dataRequest.IsSecurity)
                    {
                        emailContent = await RazorTemplateEngine.RenderAsync(
                            "/Views/EmailTemplates/vi-vn/SendWarningVadar.cshtml",
                            dataRequest.Payload);
                    }
                    else
                    {
                        emailContent = await RazorTemplateEngine.RenderAsync(
                            "/Views/EmailTemplates/vi-vn/SendPerformanceVadar.cshtml",
                            dataRequest.Payload);
                    }

                    var notificationDto = new NotificationDto()
                    {
                        Content = dataRequest.Payload.FullLog,
                        CreatedDate = DateTime.UtcNow,
                        Receiver = new string[] { dataRequest.Receiver },
                        Sender = "VADAR",
                        HostName = dataRequest.Payload.Host,
                        Type = new int[] { dataRequest.Type },
                    };

                    switch ((EnNotificationType)dataRequest.Type)
                    {
                        case EnNotificationType.Email:
                            var emailSubject = dataRequest.IsSecurity ? "Vadar Notification Security" : "Vadar Notification Performance";
                            await this.emailService.SendAsync(dataRequest.Receiver, emailSubject, emailContent, true);
                            break;
                        case EnNotificationType.Slack:
                            await this.slackBotMessagesHelper.SendMessage(dataRequest.Payload, dataRequest.Receiver, (EnNotificationContent)dataRequest.Type);
                            break;
                        case EnNotificationType.TeleGram:

                            var chatIds = await this.GetChatIds(dataRequest.Receiver, dataRequest.Payload.WorkspaceId);
                            if (chatIds != null && chatIds.Any())
                            {
                                await this.telegramHelper.SendMessage(dataRequest.Receiver, chatIds, dataRequest);
                            }
                            break;
                    }

                    var res = await this.elasticSearchCallApiHelper.CreateNotification(notificationDto);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(this.configuration["RabbitMQ:QueueName"], false, consumer);

            return Task.CompletedTask;

        }

        public async Task<IEnumerable<long>> GetChatIds(string token, int workspaceId)
        {
            // get chatIds from db.
            var chatIdsFromDb = await this.workerNotificationService.GetChatIdsFromDatabase(workspaceId);

            return chatIdsFromDb;
        }
    }
}
