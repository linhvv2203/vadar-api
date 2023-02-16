// <copyright file="InviteWorkspaceRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// InviteWorkspace Request Dto.
    /// </summary>
    public class InviteWorkspaceRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string[] Emails { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid[] WorkspaceRoles { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int UserFirstRegister { get; set; }
    }
}
