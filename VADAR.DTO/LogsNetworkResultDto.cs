// <copyright file="LogsNetworkResultDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// LogsNetworkResultDto.
    /// </summary>
    public class LogsNetworkResultDto
    {
        /// <summary>
        /// Gets or sets Time.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets Host Name.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets Workspace Name.
        /// </summary>
        public string WorkspaceName { get; set; }

        /// <summary>
        /// Gets or sets Action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string SourcePort { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string DestinationAddress { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string DestinationPort { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Message { get; set; }
    }
}
