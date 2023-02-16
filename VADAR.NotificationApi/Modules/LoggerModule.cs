// <copyright file="LoggerModule.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Autofac;
using NLog;
using VADAR.Helpers.Helper;

namespace VADAR.NotificationApi.Modules
{
    /// <summary>
    /// Autofac for logger.
    /// </summary>
    public class LoggerModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType(typeof(Logger)).As(typeof(ILogger)).SingleInstance();
            containerBuilder.RegisterGeneric(typeof(LoggerHelper<>)).As(typeof(ILoggerHelper<>)).SingleInstance();
        }
    }
}
