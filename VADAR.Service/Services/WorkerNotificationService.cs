// <copyright file="WorkerNotificationService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Razor.Templating.Core;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Repository.Interfaces;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// WorkerNotification Service.
    /// </summary>
    public class WorkerNotificationService : IWorkerNotificationService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<WorkerNotificationService> logger;
        private readonly IWorkerNotificationUnitOfWork workerNotificationUnitOfWork;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkerNotificationService"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        /// <param name="logger">logger.</param>
        /// <param name="workerNotificationUnitOfWork">workerNotificationUnitOfWork.</param>
        public WorkerNotificationService(
            IConfiguration configuration,
            ILogger<WorkerNotificationService> logger,
            IWorkerNotificationUnitOfWork workerNotificationUnitOfWork)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.workerNotificationUnitOfWork = workerNotificationUnitOfWork;
        }

        /// <inheritdoc/>
        public void ReceiveMessages()
        {
            var factory = new ConnectionFactory() { HostName = this.configuration["RabbitMQ:HostName"] };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                    queue: this.configuration["RabbitMQ:QueueName"],
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            var counter = 0;
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("new message:" + DateTime.Now.ToString());
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
                this.logger.LogInformation($"Received: {message}");
                counter++;
                Console.WriteLine("Counter: " + counter);
            };

            channel.BasicConsume(
                queue: this.configuration["RabbitMQ:QueueName"],
                autoAck: true,
                consumer: consumer);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<long>> GetChatIdsFromDatabase(int workspaceId)
        {
            var workspaceClaims = (await this.workerNotificationUnitOfWork.WorkspaceClaimRepository.GetAll())
                .Where(x => x.WorkspaceId == workspaceId && x.ClaimType == Helpers.Const.Constants.WorkspaceClaims.ChatIdTelegram)
                .Select(s => long.Parse(s.ClaimValue)).ToList();
            return workspaceClaims;
        }

        /// <inheritdoc/>
        public async Task<bool> AddChatIdsToDatabase(IEnumerable<long> chatIds, int workspaceId)
        {
            if (chatIds == null || !chatIds.Any())
            {
                return false;
            }

            chatIds = chatIds.Distinct();
            foreach (var chatId in chatIds)
            {
                if (chatId != 0)
                {
                    await this.workerNotificationUnitOfWork.WorkspaceClaimRepository.Add(new Model.WorkspaceClaim
                    {
                        ClaimType = Helpers.Const.Constants.WorkspaceClaims.ChatIdTelegram,
                        ClaimValue = chatId.ToString(),
                        WorkspaceId = workspaceId,
                    });
                }
            }

            return await this.workerNotificationUnitOfWork.Commit() > 0;
        }
    }
}
