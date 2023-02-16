// <copyright file="RoleUserDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Update Role.
    /// </summary>
    public class RoleUserDto
    {
        /// <summary>
        /// Gets or sets User Id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Role Id.
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
