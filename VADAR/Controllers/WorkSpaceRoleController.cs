// <copyright file="WorkSpaceRoleController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.DTO.ViewModels;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// Work Space Role Controller.
    /// </summary>
    [ApiController]
    public class WorkspaceRoleController : BaseController
    {
        private readonly ILoggerHelper<WorkspaceRoleController> logger;
        private readonly IWorkSpaceRoleService workspaceRoleService;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceRoleController"/> class.
        /// </summary>
        /// <param name="logger">The _logger.</param>
        /// <param name="workspaceRoleService">Workspace Role Service.</param>
        public WorkspaceRoleController(
            ILoggerHelper<WorkspaceRoleController> logger,
            IWorkSpaceRoleService workspaceRoleService)
        {
            this.logger = logger;
            this.workspaceRoleService = workspaceRoleService;
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="workspaceRoleName">workspaceRole Name.</param>
        /// <returns>WorkspaceDto.</returns>
        [HttpGet("{workspaceId}")]
        public async Task<ApiResponse<IEnumerable<WorkspaceRoleDto>>> GetWorkspaceRoleByWorkspaceId(int workspaceId, string workspaceRoleName)
        {
            try
            {
                return new ApiResponse<IEnumerable<WorkspaceRoleDto>>(EnApiStatusCode.Success, await this.workspaceRoleService.GetWorkspaceRoleByWorkspaceId(workspaceId, workspaceRoleName, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<IEnumerable<WorkspaceRoleDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetRolePaging.
        /// </summary>
        /// <param name="rolePagingRequestDto">rolePagingRequestDto.</param>
        /// <returns>WorkspaceRoleDto.</returns>
        [HttpGet("GetRolePaging")]
        public async Task<ApiResponse<RoleResultPagingDto>> GetRolePaging([FromQuery] RolePagingRequestDto rolePagingRequestDto)
        {
            try
            {
                return new ApiResponse<RoleResultPagingDto>(EnApiStatusCode.Success, await this.workspaceRoleService.GetWorkspaceRoleByWorkspaceIdPaging(rolePagingRequestDto, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<RoleResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="workspaceRoleId">workspaceRoleId.</param>
        /// <returns>WorkspaceDto.</returns>
        [HttpGet("GetPermissions/{workspaceRoleId}")]
        public async Task<ApiResponse<PermissionListsDto>> GetPermissions(Guid workspaceRoleId)
        {
            try
            {
                return new ApiResponse<PermissionListsDto>(EnApiStatusCode.Success, await this.workspaceRoleService.GetWorkspacePermissionByWorkspaceRoleIdAnUserId(workspaceRoleId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<PermissionListsDto>(ex.HResult);
            }
        }

        /// <summary>
        /// Assign Workspace Role Permissions.
        /// </summary>
        /// <param name="workspacePermissionAssignment">Workspace Permission Assignment.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpPost("AssignWorkspaceRolePermissions")]
        public async Task<ApiResponse<bool>> AssignWorkspaceRolePermissions([FromBody] AssignWorkspacePermissionDto workspacePermissionAssignment)
        {
            try
            {
                if (workspacePermissionAssignment == null)
                {
                    throw new VadarException(ErrorCode.ArgumentNull);
                }

                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceRoleService.AssignWorkspacePermissions(workspacePermissionAssignment.PermissionIds, this.CurrentUserId, workspacePermissionAssignment.WorkspaceRoleId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Assign User To Workspace Roles.
        /// </summary>
        /// <param name="workspaceRoleToUserdto">Workspace Role To User Dto.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpPost("AssignUserToWorkspaceRole")]
        public async Task<ApiResponse<bool>> AssignUserToWorkspaceRole([FromBody] AssignWorkspaceRoleToUserDto workspaceRoleToUserdto)
        {
            try
            {
                if (workspaceRoleToUserdto == null)
                {
                    throw new VadarException(ErrorCode.ArgumentNull);
                }

                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceRoleService.AssignUserToWorkspaceRole(workspaceRoleToUserdto.UserId, this.CurrentUserId, workspaceRoleToUserdto.WorkspaceRoleIds));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="workspaceRole">Workspace Role.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpPost]
        public async Task<ApiResponse<bool>> CreateNewWorkspaceRole([FromBody] WorkspaceRoleDto workspaceRole)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceRoleService.AddWorkspaceRole(workspaceRole, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="workspaceRoleId">Workspace Role Id.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpDelete("{workspaceRoleId}")]

        public async Task<ApiResponse<bool>> DeleteWorkspaceRoleById(Guid workspaceRoleId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceRoleService.DeleteById(workspaceRoleId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="workspaceRoleIds">Workspace Role Ids.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpPost("DeleteWorkspaceRoleIds")]

        public async Task<ApiResponse<bool>> DeleteWorkspaceRoleIds([FromBody] Guid[] workspaceRoleIds)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceRoleService.DeleteByIds(workspaceRoleIds, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
