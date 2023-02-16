// <copyright file="WorkspaceHostUnitOfWork.cs" company="VSEC">
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
    /// Workspace Host Unit Of Work.
    /// </summary>
    public class WorkspaceHostUnitOfWork : UnitOfWorkBase, IWorkspaceHostUnitOfWork
    {
        private IWorkspaceHostRepository workspaceHostRepository;
        private IRolePermissionRepository rolePermissionRepository;
        private IWorkspaceRolePermissionRepository workspaceRolePermissionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceHostUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public WorkspaceHostUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <summary>
        /// Gets WorkspaceHostRepository.
        /// </summary>
        public IWorkspaceHostRepository WorkspaceHostRepository => this.workspaceHostRepository ?? (this.workspaceHostRepository = new WorkspaceHostRepository(this.dbContext));

        /// <summary>
        /// Gets RolePermissionRepository.
        /// </summary>
        public IRolePermissionRepository RolePermissionRepository => this.rolePermissionRepository ?? (this.rolePermissionRepository = new RolePermissionRepository(this.dbContext));

        /// <summary>
        /// Gets WorkspaceRolePermissionRepository.
        /// </summary>
        public IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository => this.workspaceRolePermissionRepository ?? (this.workspaceRolePermissionRepository = new WorkspaceRolePermissionRepository(this.dbContext));
    }
}
