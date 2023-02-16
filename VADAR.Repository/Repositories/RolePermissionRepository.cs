// <copyright file="RolePermissionRepository.cs" company="VSEC">
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
    /// Role Permission Repository.
    /// </summary>
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RolePermissionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public RolePermissionRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <inheritdoc/>
        public async Task<long[]> GetPermissionIdsByUserId(string userId)
        {
            var rolePermissions = await this.dbset.Where(rp => rp.Role.RoleUsers.Any(el => el.UserId == userId)).ToListAsync();
            var permissionIds = rolePermissions.Select(r => r.PermissionId).ToArray();
            return permissionIds;
        }
    }
}
