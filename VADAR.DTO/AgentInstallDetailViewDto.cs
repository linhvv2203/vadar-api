// <copyright file="AgentInstallDetailViewDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// AgentInstallDetailViewDto.
    /// </summary>
    public class AgentInstallDetailViewDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string LinkDownload { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Type { get; set; }
    }
}
