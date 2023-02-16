// <copyright file="AgentInstallWorkspaceDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// AlertsDto.
    /// </summary>
    public class AgentInstallWorkspaceDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual AgentInstallDto AgentInstallDto { get; set; }

        /// <summary>
        /// Gets or sets AgentInstallId.
        /// </summary>
        public long AgentInstallId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual WorkspaceDto WorkspaceDto { get; set; }

        /// <summary>
        /// Gets or sets WorkspaceId.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string LinkDownload { get; set; }
    }
}
