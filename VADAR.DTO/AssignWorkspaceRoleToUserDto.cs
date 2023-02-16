// <copyright file="AssignWorkspaceRoleToUserDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Assign Workspace Role To User Dto.
    /// </summary>
    public class AssignWorkspaceRoleToUserDto
    {
        /// <summary>
        /// Gets or sets workspace Role Ids.
        /// </summary>
        public Guid[] WorkspaceRoleIds { get; set; }

        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        public string UserId { get; set; }
    }
}
