// <copyright file="UnitOfWorkModule.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Autofac;
using VADAR.Repository.Interfaces;
using VADAR.Repository.Repositories;
using VADAR.Repository.UnitOfWork;
using VADAR.Repository.UnitOfWork.Interfaces;

namespace VADAR.NotificationApi.Modules
{
    /// <summary>
    /// Autofac for Unit of work.
    /// </summary>
    public class UnitOfWorkModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(GenericUnitOfWork<LanguageRepository, ILanguageRepository>))
               .As(typeof(IGenericUnitOfWork<ILanguageRepository>))
               .InstancePerDependency();

            builder.RegisterType(typeof(UserUnitOfWork)).As(typeof(IUserUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(WorkspaceUnitOfWork)).As(typeof(IWorkspaceUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(DashboardUnitOfWork)).As(typeof(IDashboardUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(GroupUnitOfWork)).As(typeof(IGroupUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(WorkspaceRoleUnitOfWork)).As(typeof(IWorkspaceRoleUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(HostUnitOfWork)).As(typeof(IHostUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(HostUnitOfWork)).As(typeof(IHostUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(InviteWorkspaceRoleUnitOfWork)).As(typeof(IInviteWorkspaceRoleUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(LogUnitOfWork)).As(typeof(ILogUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(GroupHostUnitOfWork)).As(typeof(IGroupHostUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(WorkspaceHostUnitOfWork)).As(typeof(IWorkspaceHostUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(RoleUnitOfWork)).As(typeof(IRoleUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(PolicyUnitOfWork)).As(typeof(IPolicyUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(AgentInstallUnitOfWork)).As(typeof(IAgentInstallUnitOfWork)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(LicenseUnitOfWork)).As(typeof(ILicenseUnitOfWork)).InstancePerLifetimeScope();
        }
    }
}
