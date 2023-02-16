// <copyright file="AgentInstallWorkspaceViewDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// AgentInstallWorkspaceViewDto.
    /// </summary>
    public class AgentInstallWorkspaceViewDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public IEnumerable<AgentInstallDto> AgentInstallDtos { get; set; }
    }
}
