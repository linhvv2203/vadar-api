// <copyright file="RoleUnitOfWork.cs" company="VSEC">
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
    /// Role Unit Of Work.
    /// </summary>
    public class RoleUnitOfWork : UnitOfWorkBase, IRoleUnitOfWork
    {
        private IRoleRepository roleRepository;
        private IPermissionRepository permissionRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IRoleUserRepository roleUserRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="RoleUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public RoleUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IRoleRepository RoleRepository => this.roleRepository ??= new RoleRepository(this.dbContext);

        /// <inheritdoc/>
        public IPermissionRepository PermissionRepository => this.permissionRepository ??= new PermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IRoleUserRepository RoleUserRepository => this.roleUserRepository ??= new RoleUserRepository(this.dbContext);
    }
}
