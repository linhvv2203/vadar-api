// <copyright file="Worker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VADAR.SyslogWorker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Worker Class.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// Constructor method.
        /// </summary>
        /// <param name="logger">logger.</param>
        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this.logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // Get log from RabbitMQ
                // Customize log
                // Push log into Wazuh core
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
