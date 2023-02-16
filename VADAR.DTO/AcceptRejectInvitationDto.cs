// <copyright file="AcceptRejectInvitationDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Accept Reject Invitation Data Transfer Object.
    /// </summary>
    public class AcceptRejectInvitationDto
    {
        /// <summary>
        /// Gets or sets status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets invitationId.
        /// </summary>
        public Guid InvitationId { get; set; }
    }
}
