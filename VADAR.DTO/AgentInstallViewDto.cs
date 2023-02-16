// <copyright file="AgentInstallViewDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// AgentInstallViewDto.
    /// </summary>
    public class AgentInstallViewDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public IEnumerable<AgentInstallDetailViewDto> AgentInstallDetailViews { get; set; }
    }
}
