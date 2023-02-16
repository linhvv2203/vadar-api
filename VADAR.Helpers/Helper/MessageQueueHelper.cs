// <copyright file="MessageQueueHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VADAR.DTO;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// MessageQueueHelper.
    /// </summary>
    public class MessageQueueHelper : IMessageQueueHelper
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageQueueHelper"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public MessageQueueHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public void SendToRabbitMQ(RabbitMQRequestDto data)
        {
            var factory = new ConnectionFactory()
            {
                HostName = this.configuration["RabbitMQ:HostName"],
                UserName = this.configuration["RabbitMQ:UserName"],
                Password = this.configuration["RabbitMQ:Password"],
                VirtualHost = "/",
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                        queue: this.configuration["RabbitMQ:QueueName"],
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: this.configuration["RabbitMQ:QueueName"],
                basicProperties: null,
                body: body);
        }

        /// <inheritdoc/>
        public void ReceiveRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = this.configuration["RabbitMQ:HostName"] };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                        queue: this.configuration["RabbitMQ:QueueName"],
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
            };
            channel.BasicConsume(
                queue: this.configuration["RabbitMQ:QueueName"],
                autoAck: true,
                consumer: consumer);
        }
    }
}
