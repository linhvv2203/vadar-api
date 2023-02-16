// <copyright file="IRoleService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;
using VADAR.Model.Models;
using VADAR.Service.Common.Interfaces;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Interface Role Service.
    /// </summary>
    public interface IRoleService : IEntityService<Role>
    {
        /// <summary>
        /// Get All Roles.
        /// </summary>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<RoleViewDto>> GetAllRoles(string currentUserId);

        /// <summary>
        /// Get Permissions.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<PermissionDto>> GetPermissions();

        /// <summary>
        /// Add Role.
        /// </summary>
        /// <param name="roleInputDto">roleInputDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AddRole(RoleInputDto roleInputDto, string currentUserId);

        /// <summary>
        /// Update Role.
        /// </summary>
        /// <param name="roleInputDto">roleInputDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateRole(RoleInputDto roleInputDto, string currentUserId);

        /// <summary>
        /// Delete Role.
        /// </summary>
        /// <param name="roleId">RoleId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteRole(Guid roleId, string currentUserId);

        /// <summary>
        /// GetPermissionIdsByUserId.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>array permissionId.</returns>
        Task<long[]> GetPermissionIdsByUserId(string userId);

        /// <summary>
        /// Get Role Ids by User id.
        /// </summary>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>RoleIds.</returns>
        Task<IEnumerable<Guid>> GetRoleIdsByUserId(string currentUserId);
    }
}
