// <copyright file="LogSecurityDataDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// LogSecurityData Dto.
    /// </summary>
    public partial class LogSecurityDataDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string FullLog { get; set; }

        /// <summary>
        /// Gets or sets Groups.
        /// </summary>
        public string Groups { get; set; }

        /// <summary>
        /// Gets or sets Level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets References.
        /// </summary>
        public string[] References { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Rationale.
        /// </summary>
        public string Rationale { get; set; }

        /// <summary>
        /// Gets or sets Mitre.
        /// </summary>
        public string[] Mitre { get; set; }

        /// <summary>
        /// Gets or sets RawLogs.
        /// </summary>
        public string RawLogs { get; set; }

        /// <summary>
        /// Gets or sets NameDisplay.
        /// </summary>
        public string NameDisplay { get; set; }
    }
}
