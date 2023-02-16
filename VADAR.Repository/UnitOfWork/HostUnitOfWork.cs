// <copyright file="HostUnitOfWork.cs" company="VSEC">
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
    /// Unit of work.
    /// </summary>
    public class HostUnitOfWork : UnitOfWorkBase, IHostUnitOfWork
    {
        private IHostRepository hostRepository;
        private IWorkspaceHostRepository workspaceHostRepository;
        private IGroupHostRepository groupHostRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;
        private IGroupRepository groupRepository;
        private IWorkspaceRepository workspaceRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbContext.</param>
        public HostUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public IHostRepository HostRepository => this.hostRepository ?? (this.hostRepository = new HostRepository(this.dbContext));

        /// <inheritdoc/>
        public IWorkspaceHostRepository WorkspaceHostRepository => this.workspaceHostRepository ??= new WorkspaceHostRepository(this.dbContext);

        /// <inheritdoc/>
        public IGroupHostRepository GroupHostRepository => this.groupHostRepository ??= new GroupHostRepository(this.dbContext);

        /// <inheritdoc/>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ??= new RolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ??= new WorkspaceRolePermissionRepository(this.dbContext);

        /// <inheritdoc/>
        public IGroupRepository GroupRepository => this.groupRepository ??= new GroupRepository(this.dbContext);

        /// <inheritdoc />
        public IWorkspaceRepository WorkspaceRepository => this.workspaceRepository ??= new WorkspaceRepository(this.dbContext);
    }
}
