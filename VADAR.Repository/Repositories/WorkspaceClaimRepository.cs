// <copyright file="WorkspaceClaimRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VADAR.Model;
using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Workspace Claim Repository.
    /// </summary>
    public class WorkspaceClaimRepository : GenericRepository<WorkspaceClaim>, IWorkspaceClaimRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceClaimRepository"/> class.
        /// </summary>
        /// <param name="dbContext">context.</param>
        public WorkspaceClaimRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
