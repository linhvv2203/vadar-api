// <copyright file="UserClaimRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// User Repository class.
    /// </summary>
    public class UserClaimRepository : GenericRepository<UserClaim>, IUserClaimRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UserClaimRepository"/> class.
        /// User Repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public UserClaimRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
