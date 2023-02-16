using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VADAR.Service.Interfaces;

namespace VADAR.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWorkerService workerService;
        private readonly IConfiguration configurations;

        public Worker(
            ILogger<Worker> logger,
            IWorkerService workerService,
            IConfiguration configurations)
        {
            _logger = logger;
            this.workerService = workerService;
            this.configurations = configurations;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Console.WriteLine($"Worker running at: {DateTimeOffset.Now}");
                await workerService.ReceiveMessages();
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
