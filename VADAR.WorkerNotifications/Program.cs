using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Serilog;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Model;
using VADAR.Model.Models;
using VADAR.Repository.Interfaces;
using VADAR.Repository.Repositories;
using VADAR.Repository.UnitOfWork;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;
using VADAR.Service.Services;

namespace VADAR.WorkerNotifications
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
                    var serilogLogger = new LoggerConfiguration()
                    .WriteTo.RollingFile("logs/log-{Date}.log")
                    .CreateLogger();

                    services.AddLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Information);
                        builder.AddSerilog(logger: serilogLogger, dispose: true);
                    });

                    services.AddDbContext<VADARDbContext>();
                    services.AddScoped<IWorkspaceClaimRepository, WorkspaceClaimRepository>();
                    services.AddScoped<IWorkerNotificationUnitOfWork, WorkerNotificationUnitOfWork>();
                    services.AddScoped<IWorkerNotificationService, WorkerNotificationService>();
                    services.AddScoped<IMessageQueueHelper, MessageQueueHelper>();
                    services.AddScoped<IElasticSearchCallApiHelper, CallApiElasticSearchHelper>();
                    services.AddScoped<IEmailService, EmailService>();
                    services.AddScoped<ISlackBotMessagesHelper, SlackBotMessagesHelper>();
                    services.AddScoped<ITelegramHelper, TelegramHelper>();
                    services.AddScoped(typeof(ILoggerHelper<>), typeof(LoggerHelper<>));
                    services.AddHostedService<MessageReceiver>();
                    services.AddMailKit(optionBuilder =>
                    {
                        optionBuilder.UseMailKit(new MailKitOptions
                        {
                            // get options from sercets.json
                            Server = configuration["EmailSetting:Server"],
                            Port = Convert.ToInt32(configuration["EmailSetting:Port"]),
                            SenderName = configuration["EmailSetting:SenderName"],
                            SenderEmail = configuration["EmailSetting:SenderEmail"],

                            // enable ssl or tls
                            Security = true,
                        });
                    });
                });
    }
}
