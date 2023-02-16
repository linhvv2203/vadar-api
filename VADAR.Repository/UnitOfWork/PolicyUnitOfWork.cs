// <copyright file="PolicyUnitOfWork.cs" company="VSEC">
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
    /// Policy Unit Of Work.
    /// </summary>
    public class PolicyUnitOfWork : UnitOfWorkBase, IPolicyUnitOfWork
    {
        private IPolicyRepository policyRepository;
        private IWhiteListRepository whiteListRepository;
        private IHostRepository hostRepository;
        private IWorkspaceHostRepository workspaceHostRepository;
        private IWorkspaceRepository workspaceRepository;
        private IWorkspacePolicyRepository workspacePolicyRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="PolicyUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">context.</param>
        public PolicyUnitOfWork(IDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IPolicyRepository PolicyRepository => this.policyRepository ??= new PolicyRepository(this.dbContext);

        /// <inheritdoc/>
        public IWhiteListRepository WhiteListRepository => this.whiteListRepository ??= new WhiteListRepository(this.dbContext);

        /// <inheritdoc/>
        public IHostRepository HostRepository => this.hostRepository ??= new HostRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceHostRepository WorkspaceHostRepository => this.workspaceHostRepository ??= new WorkspaceHostRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspacePolicyRepository WorkspacePolicyRepository => this.workspacePolicyRepository ??= new WorkspacePolicyRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);
    }
}
