// <copyright file="WorkspaceController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Attributes.Filter;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// Initialises a new instance of the <see cref="WorkspaceController"/> class.
    /// </summary>
    public class WorkspaceController : BaseController
    {
        private readonly ILoggerHelper<WorkspaceController> logger;
        private readonly IWorkspaceService workspaceService;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceController"/> class.
        /// </summary>
        /// <param name="logger">The _logger.</param>
        /// <param name="workspaceService">Workspace Service.</param>
        public WorkspaceController(
            ILoggerHelper<WorkspaceController> logger,
            IWorkspaceService workspaceService)
        {
            this.logger = logger;
            this.workspaceService = workspaceService;
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <returns>WorkspaceDto.</returns>
        [HttpGet("GetAllWorkspace")]
        public async Task<ApiResponse<IEnumerable<WorkspaceDto>>> GetAllWorkspace()
        {
            try
            {
                return new ApiResponse<IEnumerable<WorkspaceDto>>(EnApiStatusCode.Success, await this.workspaceService.GetAllWorkspaceByUserId(this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<IEnumerable<WorkspaceDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>WorkspaceDto.</returns>
        [HttpGet("{workspaceId}")]
        public async Task<ApiResponse<WorkspaceViewModelDto>> GetWorkspaceById(int workspaceId)
        {
            try
            {
                return new ApiResponse<WorkspaceViewModelDto>(EnApiStatusCode.Success, await this.workspaceService.GetWorkspaceById(workspaceId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<WorkspaceViewModelDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetAllWorkspace.
        /// </summary>
        /// <param name="workspaceRequestDto">workspaceRequestDto.</param>
        /// <returns>WorkspaceResultPagingDto.</returns>
        [HttpGet]
        [PermissionFilter((int)EnPermissions.FullPermission)]
        public async Task<ApiResponse<WorkspaceResultPagingDto>> WorkspacePaging([FromQuery] WorkspacePagingRequestDto workspaceRequestDto)
        {
            try
            {
                return new ApiResponse<WorkspaceResultPagingDto>(EnApiStatusCode.Success, await this.workspaceService.GetAllWorkspace(workspaceRequestDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<WorkspaceResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="workspaceDto">workspaceDto.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        [PermissionFilter((int)EnPermissions.FullPermission)]
        public async Task<ApiResponse<bool>> AddWorkspace([FromBody] WorkspaceDto workspaceDto)
        {
            try
            {
                if (workspaceDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(workspaceDto));
                }

                workspaceDto.CreatedById = this.CurrentUserId;
                workspaceDto.CreatedDate = DateTime.UtcNow;
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceService.AddWorkspace(workspaceDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="registrationDto">registrationDto.</param>
        /// <returns>bool.</returns>
        [HttpPost("AutoCreateWorkspace")]
        [AllowAnonymous]
        public async Task<ApiResponse<bool>> AutoCreateWorkspace([FromBody]RegistrationDto registrationDto)
        {
            try
            {
                if (registrationDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(registrationDto));
                }

                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceService.AutoCreateWorkspace(registrationDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// CheckLicensePreInstall.
        /// </summary>
        /// <param name="tokenWorkspace">tokenWorkspace.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("CheckLicensePreInstall")]
        [AllowAnonymous]
        public async Task<ApiResponse<bool>> CheckLicensePreInstall(string tokenWorkspace)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceService.CheckLicensePreInstall(tokenWorkspace));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// UpdateWorkspace.
        /// </summary>
        /// <param name="workspaceDto">workspaceDto.</param>
        /// <returns>bool.</returns>
        [HttpPut]
        [PermissionFilter((int)EnPermissions.FullPermission)]
        public async Task<ApiResponse<bool>> UpdateWorkspace([FromBody] WorkspaceDto workspaceDto)
        {
            try
            {
                if (workspaceDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(workspaceDto));
                }

                workspaceDto.UpdateById = this.CurrentUserId;
                workspaceDto.UpdatedDate = DateTime.Now;
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceService.UpdateWorkspace(workspaceDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// DeleteWorkspace.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>bool.</returns>
        [HttpDelete("{workspaceId}")]
        [PermissionFilter((int)EnPermissions.FullPermission)]
        public async Task<ApiResponse<bool>> DeleteWorkspace(int workspaceId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceService.DeleteWorkspace(workspaceId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Delete By List Id.
        /// </summary>
        /// <param name="workspaceIds">workspaceIds.</param>
        /// <returns>bool.</returns>
        [HttpPost("DeleteByListId")]
        [PermissionFilter((int)EnPermissions.FullPermission)]
        public async Task<ApiResponse<bool>> DeleteByListId([FromBody] int[] workspaceIds)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.workspaceService.DeleteByListId(workspaceIds));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
