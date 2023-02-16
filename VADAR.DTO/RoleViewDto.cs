// <copyright file="RoleViewDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Role View Dto.
    /// </summary>
    public class RoleViewDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public IEnumerable<PermissionDto> Permissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public IEnumerable<UserDto> UsersOfRole { get; set; }
    }
}
