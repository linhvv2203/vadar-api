// <copyright file="HostRepository.cs" company="VSEC">
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
    /// Host Repository.
    /// </summary>
    public class HostRepository : GenericRepository<Host>, IHostRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HostRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public HostRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <inheritdoc/>
        public async Task<Host> GetHostByName(string hostName)
        {
            return await this.dbset.Where(x => x.Name.Equals(hostName)).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<Host> GetHostById(Guid hostId)
        {
            return await this.dbset.Where(x => x.Id.Equals(hostId)).FirstOrDefaultAsync();
        }
    }
}
