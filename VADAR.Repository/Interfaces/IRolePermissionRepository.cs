// <copyright file="IRolePermissionRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// RolePermission Repository Interface.
    /// </summary>
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        /// <summary>
        /// GetPermissionIdsByUserId.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>array.</returns>
        Task<long[]> GetPermissionIdsByUserId(string userId);
    }
}
