// <copyright file="LogUnitOfWork.cs" company="VSEC">
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
    /// Log Unit Of Work.
    /// </summary>
    public class LogUnitOfWork : UnitOfWorkBase, ILogUnitOfWork
    {
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;
        private IGroupRepository groupRepository;
        private IWorkspaceRepository workspaceRepository;
        private IWorkspaceRoleUserRepository workspaceRoleUserRepository;
        private IWorkspaceRoleRepository workspaceRoleRepository;
        private IHostRepository hostRepository;
        private IGroupHostRepository groupHostRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="LogUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">db context.</param>
        public LogUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository =>
            this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IGroupRepository GroupRepository => this.groupRepository ?? (this.groupRepository = new GroupRepository(this.dbContext));

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleUserRepository WorkspaceRoleUserRepository =>
            this.workspaceRoleUserRepository ??= new WorkspaceRoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleRepository WorkspaceRoleRepository =>
            this.workspaceRoleRepository ??= new WorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceHostRepository WorkspaceHostRepository => new WorkspaceHostRepository(this.dbContext);

        /// <inheritdoc/>
        public IHostRepository HostRepository => this.hostRepository ??= new HostRepository(this.dbContext);

        /// <inheritdoc/>
        public IGroupHostRepository GroupHostRepository => this.groupHostRepository ??= new GroupHostRepository(this.dbContext);
    }
}
