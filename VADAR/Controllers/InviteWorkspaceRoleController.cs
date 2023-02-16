// <copyright file="InviteWorkspaceRoleController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// InviteWorkspaceRole Controller.
    /// </summary>
    public class InviteWorkspaceRoleController : BaseController
    {
        private readonly IInviteWorkspaceRoleService service;
        private readonly ILoggerHelper<InviteWorkspaceRoleController> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="InviteWorkspaceRoleController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="service">service.</param>
        public InviteWorkspaceRoleController(
            ILoggerHelper<InviteWorkspaceRoleController> logger,
            IInviteWorkspaceRoleService service)
        {
            this.service = service;
            this.logger = logger;
        }

        /// <summary>
        /// GetInvitationById.
        /// </summary>
        /// <param name="invitationId">invitationId.</param>
        /// <returns>true: success, false: fail.</returns>
        [AllowAnonymous]
        [HttpGet("VerifyInvitation/{invitationId}")]
        public async Task<ApiResponse<string>> VerifyInvitation(Guid invitationId)
        {
            try
            {
                return new ApiResponse<string>(EnApiStatusCode.Success, await this.service.VerifyInvitation(invitationId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<string>(ex.HResult);
            }
        }

        /// <summary>
        /// Cancel Invitation.
        /// </summary>
        /// <param name="invitationId">Invitation Id.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpDelete("{invitationId}")]
        public async Task<ApiResponse<bool>> CancelInvitation(Guid invitationId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.service.CancelInvitation(this.CurrentUserId, invitationId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Delete Invitation.
        /// </summary>
        /// <param name="invitationId">invitation Id.</param>
        /// <param name="workspaceId">workspace Id.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpDelete("DeleteInvitation/{invitationId}/{workspaceId}")]
        public async Task<ApiResponse<bool>> DeleteInvitation(Guid invitationId, int workspaceId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.service.DeleteInvitation(this.CurrentUserId, invitationId, workspaceId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Accept Reject Invitation.
        /// </summary>
        /// <param name="acceptRejectInvitationDto">Accept Reject Invitation Data Object.</param>
        /// <returns>true: success; false: failed.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse<bool>> AcceptRejectInvitation([FromBody] AcceptRejectInvitationDto acceptRejectInvitationDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.service.AcceptRejectInvitation(this.CurrentUserId, acceptRejectInvitationDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Resend Invitation.
        /// </summary>
        /// <param name="invitationId">invitation Id.</param>
        /// <param name="language">language.</param>
        /// <returns>true: success; false: failed.</returns>
        [HttpGet("ResendInvitation/{invitationId}/{language}")]
        public async Task<ApiResponse<bool>> ResendInvitation(Guid invitationId, string language)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.service.ResendInvitation(this.CurrentUserId, invitationId, language));
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
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="emailAddress">emailAddress.</param>
        /// <returns>MembersByWorkspaceViewDto.</returns>
        [HttpGet("GetMembersByWorkspace/{workspaceId}")]
        public async Task<ApiResponse<IEnumerable<MembersByWorkspaceViewDto>>> GetMembersByWorkspace(int workspaceId, string emailAddress)
        {
            try
            {
                return new ApiResponse<IEnumerable<MembersByWorkspaceViewDto>>(EnApiStatusCode.Success, await this.service.GetMembersByWorkspace(workspaceId, emailAddress, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<IEnumerable<MembersByWorkspaceViewDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Create Invite For Workspace.
        /// </summary>
        /// <param name="inviteWorkspaceRequestDto">inviteWorkspaceRequestDto.</param>
        /// <returns>WorkspaceDto.</returns>
        [HttpPost("CreateInviteForWorkspace")]
        public async Task<ApiResponse<InviteWorkspaceRequestDto>> CreateInviteForWorkspace([FromBody] InviteWorkspaceRequestDto inviteWorkspaceRequestDto)
        {
            try
            {
                return new ApiResponse<InviteWorkspaceRequestDto>(EnApiStatusCode.Success, await this.service.CreateInviteForWorkspace(inviteWorkspaceRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<InviteWorkspaceRequestDto>(ex.HResult);
            }
        }

        /// <summary>
        /// Update WorkspaceRole For User.
        /// </summary>
        /// <param name="workspaceRoleUserUpdateRequestDto">workspaceRoleUserUpdateRequestDto.</param>
        /// <returns>true: success, false: fail.</returns>
        [HttpPut("UpdateWorkspaceRoleForUser")]
        public async Task<ApiResponse<bool>> UpdateWorkspaceRoleForUser([FromBody] WorkspaceRoleUserUpdateRequestDto workspaceRoleUserUpdateRequestDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.service.UpdateWorkspaceRoleForUser(workspaceRoleUserUpdateRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
