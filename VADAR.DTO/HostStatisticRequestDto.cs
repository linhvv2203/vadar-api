// <copyright file="HostStatisticRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Host Statistic RequestDtot.
    /// </summary>
    public class HostStatisticRequestDto
    {
        /// <summary>
        /// Gets or sets From date.
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Gets or sets To Date.
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Gets or sets Level.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Gets or sets EventGroup.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets workSpaceId.
        /// </summary>
        public int? WorkSpaceId { get; set; }

        /// <summary>
        /// Gets or sets requestUserId.
        /// </summary>
        public string RequestUserId { get; set; }

        /// <summary>
        /// Gets or sets hosts.
        /// </summary>
        public List<string> Hosts { get; set; }

        /// <summary>
        /// Gets or sets EventGroup.
        /// </summary>
        public string Serverity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets SearchLevel.
        /// </summary>
        public bool SearchLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IntoDay.
        /// </summary>
        public bool IntoDay { get; set; }

        /// <summary>
        /// Gets or sets HostName.
        /// </summary>
        public string HostName { get; set; }
    }
}
