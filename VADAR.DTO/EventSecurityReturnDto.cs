// <copyright file="EventSecurityReturnDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// EventSecurity Dto.
    /// </summary>
    public partial class EventSecurityReturnDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<EventSecurityDto> X { get; set; } = new List<EventSecurityDto>();

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<string> Y { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Total { get; set; }
    }
}
