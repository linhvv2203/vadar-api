// <copyright file="LogsPerformanceResultDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Logs Performance Result Class.
    /// </summary>
    public class LogsPerformanceResultDto
    {
        /// <summary>
        /// Gets or sets Time.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets Severity.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets Host Name.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets Event Name.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public string Status { get; set; }
    }
}
