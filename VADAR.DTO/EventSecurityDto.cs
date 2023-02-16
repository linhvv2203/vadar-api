// <copyright file="EventSecurityDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// EventSecurity Dto.
    /// </summary>
    public partial class EventSecurityDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<int> Data { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Label { get; set; }
    }
}
