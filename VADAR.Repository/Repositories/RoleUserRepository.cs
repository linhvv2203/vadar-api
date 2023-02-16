// <copyright file="RoleUserRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Role User Repository.
    /// </summary>
    public class RoleUserRepository : GenericRepository<RoleUser>, IRoleUserRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RoleUserRepository"/> class.
        /// Role User Repository.
        /// </summary>
        /// <param name="context">context.</param>
        public RoleUserRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
