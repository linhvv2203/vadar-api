// <copyright file="WorkspaceRoleUserUpdateRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// WorkspaceRoleUserUpdateRequestDto.
    /// </summary>
    public class WorkspaceRoleUserUpdateRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid[] WorkspaceRoles { get; set; }
    }
}
