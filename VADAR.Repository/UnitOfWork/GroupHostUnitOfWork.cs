// <copyright file="GroupHostUnitOfWork.cs" company="VSEC">
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
    /// GroupHostUnitOfWork.
    /// </summary>
    public class GroupHostUnitOfWork : UnitOfWorkBase, IGroupHostUnitOfWork
    {
        private IGroupHostRepository groupHostRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="GroupHostUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public GroupHostUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IGroupHostRepository GroupHostRepository => this.groupHostRepository ?? (this.groupHostRepository = new GroupHostRepository(this.dbContext));

        /// <inheritdoc />
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);
    }
}
