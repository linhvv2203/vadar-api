// <copyright file="UserUnitOfWork.cs" company="VSEC">
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
    /// User Unit Of Work Class.
    /// </summary>
    public class UserUnitOfWork : UnitOfWorkBase, IUserUnitOfWork
    {
        private IUserRepository userRepository;
        private IRoleUserRepository roleUserRepository;
        private IUserClaimRepository userClaimRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">context.</param>
        public UserUnitOfWork(IDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets User Repository.
        /// </summary>
        public IUserRepository UserRepository => this.userRepository ??= new UserRepository(this.dbContext);

        /// <inheritdoc/>
        public IRoleUserRepository RoleUserRepository => this.roleUserRepository ??= new RoleUserRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IUserClaimRepository UserClaimsRepository => this.userClaimRepository ?? (this.userClaimRepository = new UserClaimRepository(this.dbContext));
    }
}
