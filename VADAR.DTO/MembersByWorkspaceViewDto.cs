// <copyright file="MembersByWorkspaceViewDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Members By Workspace View Dto.
    /// </summary>
    public class MembersByWorkspaceViewDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int InviteStatus { get; set; }

        /// <summary>
        /// Gets or sets Invite Id.
        /// </summary>
        public Guid InviteId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid WorkspaceRoleId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
