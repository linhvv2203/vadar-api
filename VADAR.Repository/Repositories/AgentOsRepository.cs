// <copyright file="AgentOsRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Role Repository Class.
    /// </summary>
    public class AgentOsRepository : GenericRepository<AgentOs>, IAgentOsRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AgentOsRepository"/> class.
        /// Role Repository.
        /// </summary>
        /// <param name="context">context.</param>
        public AgentOsRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
