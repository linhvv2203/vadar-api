// <copyright file="LogSecurityRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// Logs Security request.
    /// </summary>
    public class LogSecurityRequestDto : PagingRequestDto
    {
        /// <summary>
        /// Gets or sets From date.
        /// </summary>
        public DateTime? FromDate { get; set; } = DateTime.Now.AddDays(-1);

        /// <summary>
        /// Gets or sets To Date.
        /// </summary>
        public DateTime? ToDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets Devices.
        /// </summary>
        public int? Devices { get; set; }

        /// <summary>
        /// Gets or sets Level.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Gets or sets EventGroup.
        /// </summary>
        public string EventGroup { get; set; }

        /// <summary>
        /// Gets or sets EventName.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets HostName.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets request User Id.
        /// </summary>
        public string RequestUserId { get; set; }

        /// <summary>
        /// Gets or sets workspace Id.
        /// </summary>
        public int? WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets workspace AgentGroup.
        /// </summary>
        public string AgentGroup { get; set; }

        /// <summary>
        /// Gets or sets hosts.
        /// </summary>
        public List<string> Hosts { get; set; }

        /// <summary>
        /// Gets or sets Levels.
        /// </summary>
        public List<int> Levels { get; set; }

        /// <summary>
        /// Gets or sets Mitre.
        /// </summary>
        public string Mitre { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IntoDay.
        /// </summary>
        public bool IntoDay { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<string> EventList { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Sort { get; set; } = "desc";
    }
}
