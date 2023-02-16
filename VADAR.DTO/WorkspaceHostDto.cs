// <copyright file="WorkspaceHostDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.DTO
{
    /// <summary>
    /// Work Space Role Dto.
    /// </summary>
    public class WorkspaceHostDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid HostId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
