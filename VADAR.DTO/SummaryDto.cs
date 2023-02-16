// <copyright file="SummaryDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Rule Dto.
    /// </summary>
    public partial class SummaryDto
    {
        /// <summary>
        /// Gets or sets number of hosts.
        /// </summary>
        public int TotalHosts { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Disconnect { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Healthy { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int UnHealthy { get; set; }
    }
}
