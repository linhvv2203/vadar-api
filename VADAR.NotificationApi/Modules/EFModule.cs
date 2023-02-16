// <copyright file="EFModule.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Autofac;
using VADAR.Model;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.NotificationApi.Modules
{
    /// <summary>
    /// Register autofac.
    /// </summary>
    public class EfModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(VADARDbContext)).As(typeof(IDbContext)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(UnitOfWorkBase)).As(typeof(IUnitOfWork)).InstancePerLifetimeScope();
        }
    }
}
