// <copyright file="WorkspaceRoleUnitOfWork.cs" company="VSEC">
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
    /// Workspace Role Unit Of Work.
    /// </summary>
    public class WorkspaceRoleUnitOfWork : UnitOfWorkBase, IWorkspaceRoleUnitOfWork
    {
        private IWorkspaceRoleRepository workspaceRoleRepository;
        private IWorkspaceRepository workspaceRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;
        private IWorkspaceRoleUserRepository workspaceRoleUserRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IPermissionRepository permissionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceRoleUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">db context.</param>
        public WorkspaceRoleUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IWorkspaceRoleRepository WorkspaceRoleRepository => this.workspaceRoleRepository ??= new WorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleUserRepository WorkspaceRoleUserRepository => this.workspaceRoleUserRepository ??= new WorkspaceRoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository =>
            this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IPermissionRepository PermissionRepository =>
            this.permissionRepository ??= new PermissionRepository(this.dbContext);
    }
}
