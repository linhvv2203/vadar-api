// <copyright file="WorkspaceRolePermissionRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Workspace Role Permission Repository.
    /// </summary>
    public class WorkspaceRolePermissionRepository : GenericRepository<WorkspaceRolePermission>, IWorkspaceRolePermissionRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceRolePermissionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public WorkspaceRolePermissionRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
