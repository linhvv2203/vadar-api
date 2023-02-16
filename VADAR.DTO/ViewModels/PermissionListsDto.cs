// <copyright file="PermissionListsDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO.ViewModels
{
    /// <summary>
    /// Permission List.
    /// </summary>
    public class PermissionListsDto
    {
        /// <summary>
        /// Gets or sets assigned Permissions.
        /// </summary>
        public IEnumerable<PermissionDto> AssignedPermissions { get; set; }

        /// <summary>
        /// Gets or sets unAssigned Permissions.
        /// </summary>
        public IEnumerable<PermissionDto> UnAssignedPermissions { get; set; }
    }
}
