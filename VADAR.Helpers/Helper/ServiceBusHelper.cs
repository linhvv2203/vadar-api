// <copyright file="ServiceBusHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Service Bus Helper.
    /// </summary>
    public class ServiceBusHelper : IServiceBusHelper
    {
        private readonly ILoggerHelper<ServiceBusHelper> logger;
        private readonly IConfiguration configuration;
        private string queueName;
        private string connectionString;
        private IQueueClient queueClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="ServiceBusHelper"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        /// <param name="logger">logger.</param>
        public ServiceBusHelper(ILoggerHelper<ServiceBusHelper> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.queueName = this.configuration["ServiceBus:QueueName"];
            this.connectionString = this.configuration["ServiceBus:ServiceBusConnectionString"];
            this.queueClient = new QueueClient(this.connectionString, this.queueName);
        }

        /// <inheritdoc/>
        public async Task SendMessage(dynamic message)
        {
            try
            {
                // Send the message to the queue
                string messageBody = $"{JsonSerializer.Serialize(message)}";
                var messageDto = new Message(Encoding.UTF8.GetBytes(messageBody));
                await this.queueClient.SendAsync(messageDto);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task RecieveMessages()
        {
            this.queueClient = new QueueClient(this.connectionString, this.queueName);

            // Register the queue message handler and receive messages in a loop
            this.RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadLine();

            await this.queueClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(this.ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false,
            };

            // Register the function that processes messages.
            this.queueClient.RegisterMessageHandler(this.ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            this.logger.LogInfo($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await this.queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
