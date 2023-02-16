// <copyright file="ChartLineReturnDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// ChartLineReturnDto.
    /// </summary>
    public class ChartLineReturnDto
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        public List<EventChartDto> Value { get; set; }
    }
}
