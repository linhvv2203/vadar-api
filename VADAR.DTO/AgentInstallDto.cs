// <copyright file="AgentInstallDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// AlertsDto.
    /// </summary>
    public class AgentInstallDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets Phone, Email.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets 0: Inactive, 1: Active.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string LinkDownload { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual AgentOsDto AgentOsDto { get; set; }

        /// <summary>
        /// Gets or sets GroupNotificationId.
        /// </summary>
        public long OsId { get; set; }
    }
}
