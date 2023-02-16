// <copyright file="PolicyRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Policy Repository.
    /// </summary>
    public class PolicyRepository : GenericRepository<Policy>, IPolicyRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PolicyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public PolicyRepository(IDbContext dbContext)
        : base(dbContext)
        {
        }
    }
}
