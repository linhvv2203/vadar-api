// <copyright file="InviteWorkspaceRoleUnitOfWork.cs" company="VSEC">
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
    /// InviteWorkspaceRole Unit Of Work.
    /// </summary>
    public class InviteWorkspaceRoleUnitOfWork : UnitOfWorkBase, IInviteWorkspaceRoleUnitOfWork
    {
        private IInviteWorkspaceRoleRepository inviteWorkspaceRoleRepository;
        private IWorkspaceRepository workspaceRepository;
        private IWorkspaceRoleRepository workspaceRoleRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;
        private IWorkspaceRoleUserRepository workspaceRoleUserRepository;
        private IUserRepository userRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="InviteWorkspaceRoleUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">context.</param>
        public InviteWorkspaceRoleUnitOfWork(IDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public IWorkspaceRoleUserRepository WorkspaceRoleUserRepository => this.workspaceRoleUserRepository ??= new WorkspaceRoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IInviteWorkspaceRoleRepository InviteWorkspaceRoleRepository => this.inviteWorkspaceRoleRepository ??= new InviteWorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRoleRepository WorkspaceRoleRepository => this.workspaceRoleRepository ??= new WorkspaceRoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository =>
            this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IUserRepository UserRepository => this.userRepository ??= new UserRepository(this.dbContext);
    }
}
