// <copyright file="IEntityService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Interfaces;

namespace VADAR.Service.Common.Interfaces
{
    /// <summary>
    /// IEntityService.
    /// </summary>
    /// <typeparam name="T">Entity model.</typeparam>
    public interface IEntityService<T> : IService
    {
        /// <summary>
        /// Create an entity.
        /// </summary>
        /// <param name="entity">T.</param>
        /// <returns>Created Entity.</returns>
        Task<T> Create(T entity);

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entity">T.</param>
        /// <returns>true if delete successfully, otherwise return false.</returns>
        Task<bool> Delete(T entity);

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>IEnumerable T.</returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// Update entity.
        /// </summary>
        /// <param name="entity">T.</param>
        /// <returns>true if update successfully, otherwise return false.</returns>
        Task<bool> Update(T entity);

        /// <summary>
        /// Permission Filter.
        /// </summary>
        /// <param name="userId">user Id.</param>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <param name="permissionIds">Permission Ids.</param>
        /// <param name="rolePermissionRepo">Role Permission Repository.</param>
        /// <param name="workspaceRolePermissionRepository">Workspace Role Permission Repository.</param>
        /// <returns>true: permission allowed; false: permission unallowed.</returns>
        Task<bool> ValidatePermission(
            string userId,
            int? workspaceId,
            long[] permissionIds,
            IRolePermissionRepository rolePermissionRepo,
            IWorkspaceRolePermissionRepository workspaceRolePermissionRepository);

        /// <summary>
        /// Get Permissions.
        /// </summary>
        /// <param name="userId">user Id.</param>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <param name="rolePermissionRepo">Role Permission Repository.</param>
        /// <param name="workspaceRolePermissionRepository">Workspace Role Permission Repository.</param>
        /// <returns>PermissionIds.</returns>
        Task<IEnumerable<long>> GetPermissions(
            string userId,
            int? workspaceId,
            IRolePermissionRepository rolePermissionRepo,
            IWorkspaceRolePermissionRepository workspaceRolePermissionRepository);

        /// <summary>
        /// GetDefaultAgentOs.
        /// </summary>
        /// <returns>List AgentOs.</returns>
        List<AgentOs> GetDefaultAgentOs();
    }
}
