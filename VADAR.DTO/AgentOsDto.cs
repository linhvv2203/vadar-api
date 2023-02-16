// <copyright file="AgentOsDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// AlertsDto.
    /// </summary>
    public class AgentOsDto
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets Icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual WorkspaceDto Workspace { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual IEnumerable<AgentInstallDto> AgentInstalls { get; set; }
    }
}
