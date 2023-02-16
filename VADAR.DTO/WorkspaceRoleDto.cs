// <copyright file="WorkspaceRoleDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Work Space Role Dto.
    /// </summary>
    public class WorkspaceRoleDto
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
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
