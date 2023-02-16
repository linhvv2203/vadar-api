// <copyright file="AddHostToGroupByIdRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// AddHostToGroupByIdRequestDto.
    /// </summary>
    public class AddHostToGroupByIdRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<Guid> HostIds { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
