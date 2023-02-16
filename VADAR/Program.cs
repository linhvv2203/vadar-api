// <copyright file="Program.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace VADAR
{
    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        private static string sentryDSN;
        private static string environmentName;

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">args.</param>
        public static void Main(string[] args)
        {
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

        /// <summary>
        /// Build Web Host.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>IWebHost.</returns>
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
                .UseUrls(environmentName == "Development" ? "http://*:60000" : "http://*:60000")

                // .UseSentry(sentryDSN)
                .Build();
    }
}
