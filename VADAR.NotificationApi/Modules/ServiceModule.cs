// <copyright file="ServiceModule.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Reflection;
using Autofac;

namespace VADAR.NotificationApi.Modules
{
    /// <summary>
    /// Autofac for Service.
    /// </summary>
    public class ServiceModule : Autofac.Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("VADAR.Service"))
                      .Where(t => t.Name.EndsWith("Service"))
                      .PropertiesAutowired()
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope();
        }
    }
}
