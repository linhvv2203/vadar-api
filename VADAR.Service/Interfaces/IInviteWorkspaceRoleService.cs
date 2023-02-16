// <copyright file="IInviteWorkspaceRoleService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// InviteWorkspaceRole Service Interface.
    /// </summary>
    public interface IInviteWorkspaceRoleService
    {
        /// <summary>
        /// Get Members By Workspace.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="emailAddress">email address.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>IEnumerable MembersByWorkspaceViewDto.</returns>
        Task<IEnumerable<MembersByWorkspaceViewDto>> GetMembersByWorkspace(int workspaceId, string emailAddress, string currentUserId);

        /// <summary>
        /// Cancel Invitation.
        /// </summary>
        /// <param name="currentUserId">Current User id.</param>
        /// <param name="invitationId">Invitation Id.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> CancelInvitation(string currentUserId, Guid invitationId);

        /// <summary>
        /// Cancel Invitation.
        /// </summary>
        /// <param name="currentUserId">Current User id.</param>
        /// <param name="invitationId">Invitation Id.</param>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> DeleteInvitation(string currentUserId, Guid invitationId, int workspaceId);

        /// <summary>
        /// Accept Reject Invitation.
        /// </summary>
        /// <param name="currentUserId">Current User id.</param>
        /// <param name="acceptRejectInvitation">Accept Reject Invitation Data Transfer Object.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> AcceptRejectInvitation(string currentUserId, AcceptRejectInvitationDto acceptRejectInvitation);

        /// <summary>
        /// Resend Invitation.
        /// </summary>
        /// <param name="currentUserId">currentUserId.</param>
        /// <param name="invitationId">invitationId.</param>
        /// <param name="language">language.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> ResendInvitation(string currentUserId, Guid invitationId, string language);

        /// <summary>
        /// Create Invite For Workspace.
        /// </summary>
        /// <param name="inviteWorkspaceRequestDto">inviteWorkspaceRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <param name="isAuth">isAuth.</param>
        /// <returns>Email Array Invite Success.</returns>
        Task<InviteWorkspaceRequestDto> CreateInviteForWorkspace(InviteWorkspaceRequestDto inviteWorkspaceRequestDto, string currentUserId, bool isAuth = true);

        /// <summary>
        /// Update WorkspaceRole For User.
        /// </summary>
        /// <param name="workspaceRoleUserUpdateRequestDto">workspaceRoleUserUpdateRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success, false: fail.</returns>
        Task<bool> UpdateWorkspaceRoleForUser(WorkspaceRoleUserUpdateRequestDto workspaceRoleUserUpdateRequestDto, string currentUserId);

        /// <summary>
        /// Verify Invitation.
        /// </summary>
        /// <param name="invitationId">invitationId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success, false: fail.</returns>
        Task<string> VerifyInvitation(Guid invitationId, string currentUserId);
    }
}
