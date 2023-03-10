// <copyright file="HelperModule.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Reflection;
using Autofac;

namespace VADAR.NotificationApi.Modules
{
    /// <summary>
    /// Implement Dependency Injection for Helpers.
    /// </summary>
    public class HelperModule : Autofac.Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("VADAR.Helpers"))

                      .Where(t => t.Name.EndsWith("Helper"))

                      .AsImplementedInterfaces()

                      .InstancePerLifetimeScope();
        }
    }
}
