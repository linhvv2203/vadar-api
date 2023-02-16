// <copyright file="WorkspaceHostRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// WorkspaceHost Repository.
    /// </summary>
    public class WorkspaceHostRepository : GenericRepository<WorkspaceHost>, IWorkspaceHostRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceHostRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public WorkspaceHostRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
