// <copyright file="LinkDownloadViewDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// LinkDownloadViewDto.
    /// </summary>
    public class LinkDownloadViewDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets 1 performce 2 security .
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public int AgentInstallId { get; set; }
    }
}
