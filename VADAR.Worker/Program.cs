using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Mapping;
using VADAR.Model;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;
using VADAR.Service.Services;

namespace VADAR.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    Log.Logger = new LoggerConfiguration().WriteTo.RollingFile("logs/log-{Date}.log").CreateLogger();
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = configuration["RedisConnectionString"];
                        options.InstanceName = "VADAR";
                    });
                    services.AddSingleton(configuration);
                    services.AddSingleton(typeof(ILoggerHelper<>), typeof(LoggerHelper<>));
                    services.AddTransient<IRedisCachingHelper, RedisCachingHelper>();
                    services.AddSingleton<IServiceBusHelper, ServiceBusHelper>();
                    services.AddSingleton<IWorkerService, WorkerService>();
                    services.AddSingleton<IDashboardUnitOfWork, DashboardUnitOfWork>();
                    services.AddSingleton<IWorkspaceUnitOfWork, WorkspaceUnitOfWork>();
                    services.AddSingleton<ILicenseUnitOfWork, LicenseUnitOfWork>();
                    services.AddSingleton<IWorkspaceHostUnitOfWork, WorkspaceHostUnitOfWork>();
                    services.AddSingleton<IElasticSearchCallApiHelper, CallApiElasticSearchHelper>();
                    services.AddSingleton<IDbContext, VADARDbContext>();
                    services.AddSingleton<IStringHelper, StringHelper>();
                    services.AddSingleton<ICallApiHostZabbixHelper, CallApiHostZabbixHelper>();
                    services.AddSingleton<IMiniIOHelper, MiniIoHelper>();
                    services.AddAutoMapper(typeof(DtoProfile));
                    services.AddSingleton<IMapper, Mapper>();
                    services.AddHostedService<Worker>();
                });
    }
}
