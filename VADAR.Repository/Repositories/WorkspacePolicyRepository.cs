// <copyright file="WorkspacePolicyRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// WorkspacePolicy Repository.
    /// </summary>
    public class WorkspacePolicyRepository : GenericRepository<WorkspacePolicy>, IWorkspacePolicyRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspacePolicyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public WorkspacePolicyRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
