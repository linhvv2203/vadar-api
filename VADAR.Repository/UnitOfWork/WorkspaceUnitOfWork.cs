// <copyright file="WorkspaceUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;
using VADAR.Repository.Repositories;
using VADAR.Repository.UnitOfWork.Interfaces;

namespace VADAR.Repository.UnitOfWork
{
    /// <summary>
    /// Workspace Unit Of Work.
    /// </summary>
    public class WorkspaceUnitOfWork : UnitOfWorkBase, IWorkspaceUnitOfWork
    {
        private IWorkspaceRepository workspaceRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;
        private IWorkspaceRoleRepository workspaceRoleRepository;
        private IWorkspaceRoleUserRepository workspaceRoleUserRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IAgentInstallRepository agentInstallRepository;
        private IAgentOsRepository agentOsRepository;
        private IWorkspaceNotificationRepository workspaceNotificationRepository;
        private IInviteWorkspaceRoleRepository inviteWorkspaceRoleRepository;
        private INotificationSettingRepository notificationSettingRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">db context.</param>
        public WorkspaceUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository =>
            this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleRepository WorkspaceRoleRepository =>
            this.workspaceRoleRepository ??= new WorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleUserRepository WorkspaceRoleUserRepository =>
            this.workspaceRoleUserRepository ??= new WorkspaceRoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository =>
            this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IAgentInstallRepository AgentInstallRepository =>
            this.agentInstallRepository ??= new AgentInstallRepository(this.dbContext);

        /// <inheritdoc/>
        public IAgentOsRepository AgentOsRepository =>
            this.agentOsRepository ??= new AgentOsRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceNotificationRepository WorkspaceNotificationRepository =>
            this.workspaceNotificationRepository ??= new WorkspaceNotificationRepository(this.dbContext);

        /// <inheritdoc/>
        public IInviteWorkspaceRoleRepository InviteWorkspaceRoleRepository =>
            this.inviteWorkspaceRoleRepository ??= new InviteWorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public INotificationSettingRepository NotificationSettingRepository =>
            this.notificationSettingRepository ??= new NotificationSettingRepository(this.dbContext);
    }
}
