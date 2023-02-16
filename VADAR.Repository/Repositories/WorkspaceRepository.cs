// <copyright file="WorkspaceRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Workspace Repository class.
    /// </summary>
    public class WorkspaceRepository : GenericRepository<Workspace>, IWorkspaceRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceRepository"/> class.
        /// Workspace Repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public WorkspaceRepository(IDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<Workspace> GetWorkspaceById(int id)
        {
            return await this.dbset.Where(x => x.Id.Equals(id)).Include(i => i.License).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<Workspace> GetWorkspaceByToken(string tokenWorkspace)
        {
            return await this.dbset.Where(x => x.TokenWorkspace.Equals(tokenWorkspace)).FirstOrDefaultAsync();
        }
    }
}
