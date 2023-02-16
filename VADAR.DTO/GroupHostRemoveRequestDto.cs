// <copyright file="GroupHostRemoveRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// GroupHostRemoveRequestDto.
    /// </summary>
    public class GroupHostRemoveRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<string> Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string GroupId { get; set; }
    }
}
