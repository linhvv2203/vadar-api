// <copyright file="DashboardUnitOfWork.cs" company="VSEC">
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
    /// Dashboard Unit Of Work.
    /// </summary>
    public class DashboardUnitOfWork : UnitOfWorkBase, IDashboardUnitOfWork
    {
        private IRoleRepository roleRepository;
        private IPermissionRepository permissionRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IRoleUserRepository roleUserRepository;
        private IWorkspaceRepository workspaceRepository;
        private IGroupHostRepository groupHostRepository;
        private IHostRepository hostRepository;
        private IGroupRepository groupRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;
        private IWorkspaceRoleUserRepository workspaceRoleUserRepository;
        private IWorkspaceRoleRepository workspaceRoleRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="DashboardUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public DashboardUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IRoleRepository RoleRepository =>
            this.roleRepository ??= new RoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IPermissionRepository PermissionRepository => this.permissionRepository ??= new PermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IRoleUserRepository RoleUserRepository => this.roleUserRepository ??= new RoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IGroupHostRepository GroupHostRepository => this.groupHostRepository ??= new GroupHostRepository(this.dbContext);

        /// <inheritdoc/>
        public IGroupRepository GroupRepository => this.groupRepository ??= new GroupRepository(this.dbContext);

        /// <inheritdoc/>
        public IHostRepository HostRepository => this.hostRepository ??= new HostRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository =>
            this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleUserRepository WorkspaceRoleUserRepository =>
            this.workspaceRoleUserRepository ??= new WorkspaceRoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleRepository WorkspaceRoleRepository =>
            this.workspaceRoleRepository ??= new WorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceHostRepository WorkspaceHostRepository => new WorkspaceHostRepository(this.dbContext);
    }
}
