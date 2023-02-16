// <copyright file="PerformanceEventReturnDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// LogSecurityData Dto.
    /// </summary>
    public partial class PerformanceEventReturnDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Time { get; set; }
    }
}
