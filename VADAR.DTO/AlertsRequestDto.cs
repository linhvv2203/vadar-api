// <copyright file="AlertsRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Alert Request Dto.
    /// </summary>
    public class AlertsRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string[] Emails { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
