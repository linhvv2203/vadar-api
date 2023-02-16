// <copyright file="LogsSecuritySummaryResultDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// LogsSecuritySummaryResultDto.
    /// </summary>
    public class LogsSecuritySummaryResultDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int? LogsOverview { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int? LogsAboveLevel9 { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int? LogsToday { get; set; }
    }
}
