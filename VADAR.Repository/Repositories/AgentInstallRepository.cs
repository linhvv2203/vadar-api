// <copyright file="AgentInstallRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Permission Repository.
    /// </summary>
    public class AgentInstallRepository : GenericRepository<AgentInstall>, IAgentInstallRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AgentInstallRepository"/> class.
        /// Category Repository.
        /// </summary>
        /// <param name="context">context.</param>
        public AgentInstallRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
