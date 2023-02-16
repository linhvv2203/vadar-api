// <copyright file="AgentInstallUnitOfWork.cs" company="VSEC">
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
    /// AgentInstall Unit Of Work.
    /// </summary>
    public class AgentInstallUnitOfWork : UnitOfWorkBase, IAgentInstallUnitOfWork
    {
        private IAgentInstallRepository agentInstallRepository;
        private IWorkspaceRepository workspaceRepository;
        private IAgentOsRepository agentOsRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="AgentInstallUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public AgentInstallUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IAgentInstallRepository AgentInstallRepository => this.agentInstallRepository ??= new AgentInstallRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IAgentOsRepository AgentOsRepository => this.agentOsRepository ??= new AgentOsRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);
    }
}
