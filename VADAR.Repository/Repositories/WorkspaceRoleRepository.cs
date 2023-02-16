// <copyright file="WorkspaceRoleRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// WorkspaceRole Repository.
    /// </summary>
    public class WorkspaceRoleRepository : GenericRepository<WorkspaceRole>, IWorkspaceRoleRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceRoleRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public WorkspaceRoleRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <inheritdoc/>
        public async Task<IQueryable<WorkspaceRole>> GetWorkspaceRoleByWorkspaceId(int workspaceId)
        {
            return await Task.FromResult(this.dbset.Where(w => w.WorkspaceId == workspaceId));
        }

        /// <inheritdoc/>
        public async Task<WorkspaceRole> GetWorkspaceRoleById(Guid id)
        {
            return await this.dbset.FirstOrDefaultAsync(w => w.Id == id);
        }
    }
}
