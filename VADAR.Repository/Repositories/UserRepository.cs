// <copyright file="UserRepository.cs" company="VSEC">
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
    /// User Repository class.
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UserRepository"/> class.
        /// User Repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public UserRepository(IDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<User> GetUserById(string userId)
        {
            return await this.dbset.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        }
    }
}
