// <copyright file="LogsPerformanceRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// Logs performance request.
    /// </summary>
    public class LogsPerformanceRequestDto : PagingRequestDto
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
        /// Gets or sets Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Devices.
        /// </summary>
        public int? Devices { get; set; }

        /// <summary>
        /// Gets or sets Group Name.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets severity.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets requestUserId.
        /// </summary>
        public string RequestUserId { get; set; }

        /// <summary>
        /// Gets or sets workspaceId.
        /// </summary>
        public int? WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets hosts.
        /// </summary>
        public List<string> Hosts { get; set; }
    }
}
