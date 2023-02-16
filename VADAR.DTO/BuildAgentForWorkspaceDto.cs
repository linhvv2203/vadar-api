// <copyright file="BuildAgentForWorkspaceDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// BuildAgentForWorkspace Dto.
    /// </summary>
    public class BuildAgentForWorkspaceDto
    {
        /// <summary>
        /// Gets or sets Token Workspace.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets Name workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether: true: prod, false: dev.
        /// </summary>
        public bool IsProd { get; set; }

        /// <summary>
        /// Gets or sets: ubuntu, centos, windows, mac...
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Folders { get; set; }
    }
}
