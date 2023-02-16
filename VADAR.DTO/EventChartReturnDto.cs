// <copyright file="EventChartReturnDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// EventChartReturnDto Dto.
    /// </summary>
    public partial class EventChartReturnDto
    {
        /// <summary>
        /// Gets or sets levelName.
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        public List<EventChartDto> Value { get; set; }
    }
}
