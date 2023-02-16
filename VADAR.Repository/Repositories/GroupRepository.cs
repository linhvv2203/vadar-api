// <copyright file="GroupRepository.cs" company="VSEC">
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
    /// Group Repository.
    /// </summary>
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GroupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public GroupRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <inheritdoc/>
        public async Task<Group> GetGroupById(Guid id)
        {
            return await this.dbset.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<Group> GetGroupByName(string name)
        {
            return await this.dbset.Where(x => x.Name.Equals(name)).FirstOrDefaultAsync();
        }
    }
}
