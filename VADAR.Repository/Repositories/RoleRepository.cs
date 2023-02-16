// <copyright file="RoleRepository.cs" company="VSEC">
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
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RoleRepository"/> class.
        /// Role Repository.
        /// </summary>
        /// <param name="context">context.</param>
        public RoleRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
