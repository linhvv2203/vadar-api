// <copyright file="PermissionRepository.cs" company="VSEC">
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
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PermissionRepository"/> class.
        /// Category Repository.
        /// </summary>
        /// <param name="context">context.</param>
        public PermissionRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
