using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace VADAR.NotificationApi
{
    public class Program
    {
        private static string sentryDSN;
        private static string environmentName;
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            environmentName =
                Environment.GetEnvironmentVariable(
                    "HOST_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            sentryDSN = config.GetSection("SentryDSN").Value;
            var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 90000000;
                })
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                    // options.Limits.MaxRequestBodySize = 30 * 1024;
                    options.Limits.MinRequestBodyDataRate =
                        new MinDataRate(100, TimeSpan.FromSeconds(10));
                    options.Limits.MinResponseDataRate =
                        new MinDataRate(100, TimeSpan.FromSeconds(10));
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                })
                .UseNLog()
                .UseUrls(environmentName == "Development" ? "http://*:5000" : "http://*:5000")
                .Build();
    }
}
