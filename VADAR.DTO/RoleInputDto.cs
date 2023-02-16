// <copyright file="RoleInputDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;

namespace VADAR.DTO
{
    /// <summary>
    /// Add Role Dto.
    /// </summary>
    public class RoleInputDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public long[] PermissionIds { get; set; }
    }
}
