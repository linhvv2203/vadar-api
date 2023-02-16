// <copyright file="IWorkSpaceRoleService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;
using VADAR.DTO.ViewModels;
using VADAR.Model.Models;
using VADAR.Service.Common.Interfaces;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Work space role service interface.
    /// </summary>
    public interface IWorkSpaceRoleService : IEntityService<WorkspaceRole>
    {
        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="workspaceRole">Work Space Role.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AddWorkspaceRole(WorkspaceRoleDto workspaceRole, string currentUserId);

        /// <summary>
        /// Assign Role to User.
        /// </summary>
        /// <param name="roleIds">RoleIds.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <param name="userId">user assigned.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> AssignRoleToUser(Guid[] roleIds, string currentUserId, string userId);

        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="workspaceId">Work Space Id.</param>
        /// <param name="workspaceRoleName">workspaceRole Name.</param>
        /// <param name="currentUserId">Current user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<WorkspaceRoleDto>> GetWorkspaceRoleByWorkspaceId(int workspaceId, string workspaceRoleName, string currentUserId);

        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="workspaceId">Work Space Id.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <param name="userId">user id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<WorkspaceRoleDto>> GetWorkspaceRoleByWorkspaceIdAndUserId(int workspaceId, string currentUserId, string userId);

        /// <summary>
        /// Delete By Id.
        /// </summary>
        /// <param name="id">Workspace Role Id.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> DeleteById(Guid id, string currentUserId);

        /// <summary>
        /// Delete by Ids.
        /// </summary>
        /// <param name="ids">Workspace Role Ids.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> DeleteByIds(Guid[] ids, string currentUserId);

        /// <summary>
        /// Get Workspace Permission By Workspace Id and user Id.
        /// </summary>
        /// <param name="workspaceRoleId">Workspace Id.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Permission List.</returns>
        Task<PermissionListsDto> GetWorkspacePermissionByWorkspaceRoleIdAnUserId(
            Guid workspaceRoleId,
            string currentUserId);

        /// <summary>
        /// Assign Workspace Permission.
        /// </summary>
        /// <param name="permissionIds">Permission Ids.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <param name="roleId">Role Id.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> AssignWorkspacePermissions(int[] permissionIds, string currentUserId, Guid roleId);

        /// <summary>
        /// Assign User To Workspace Roles.
        /// </summary>
        /// <param name="userId">User assigned.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <param name="workspaceRoleIds">Workspace Role Ids.</param>
        /// <returns>true: success; false: failed;.</returns>
        Task<bool> AssignUserToWorkspaceRole(string userId, string currentUserId, Guid[] workspaceRoleIds);

        /// <summary>
        /// GetWorkspaceRoleByWorkspaceIdPaging.
        /// </summary>
        /// <param name="rolePagingRequestDto">rolePagingRequestDto.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<RoleResultPagingDto> GetWorkspaceRoleByWorkspaceIdPaging(RolePagingRequestDto rolePagingRequestDto, string userId);
    }
}
