// <copyright file="LogsNetworkRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using VADAR.DTO.AbtractClasses;

namespace VADAR.DTO
{
    /// <summary>
    /// Logs Network Request Dto.
    /// </summary>
    public class LogsNetworkRequestDto : PagingRequestDto
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
        /// Gets or sets Host Name.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets Devices.
        /// </summary>
        public int? Devices { get; set; }

        /// <summary>
        /// Gets or sets Souce Address.
        /// </summary>
        public string SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets Destination Address.
        /// </summary>
        public string DestinationAddress { get; set; }

        /// <summary>
        /// Gets or sets workspaceId.
        /// </summary>
        public int? WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets hosts.
        /// </summary>
        public List<string> Hosts { get; set; }

        /// <summary>
        /// Gets or sets RequestUserId.
        /// </summary>
        public string RequestUserId { get; set; }
    }
}
