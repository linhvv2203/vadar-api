// <copyright file="AssignWorkspacePermissionDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Assign Workspace Permission Dto.
    /// </summary>
    public class AssignWorkspacePermissionDto
    {
        /// <summary>
        /// Gets or sets workspace Role Id.
        /// </summary>
        public Guid WorkspaceRoleId { get; set; }

        /// <summary>
        /// Gets or sets permission Ids.
        /// </summary>
        public int[] PermissionIds { get; set; }
    }
}
