// <copyright file="GroupHostRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// GroupHost Repository.
    /// </summary>
    public class GroupHostRepository : GenericRepository<GroupHost>, IGroupHostRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GroupHostRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public GroupHostRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
