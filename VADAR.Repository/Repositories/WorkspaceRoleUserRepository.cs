// <copyright file="WorkspaceRoleUserRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Workspace UserWorkspaceRole Repository.
    /// </summary>
    public class WorkspaceRoleUserRepository : GenericRepository<WorkspaceRoleUser>, IWorkspaceRoleUserRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceRoleUserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public WorkspaceRoleUserRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
