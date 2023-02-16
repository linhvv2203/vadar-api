using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using VADAR.Service.Interfaces;

namespace VADAR.WorkerNotifications
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWorkerNotificationService workerNotificationService;
        private readonly IConfiguration configurations;

        public Worker(ILogger<Worker> logger, IWorkerNotificationService workerNotificationService, IConfiguration configurations)
        {
            _logger = logger;
            this.workerNotificationService = workerNotificationService;
            this.configurations = configurations;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
