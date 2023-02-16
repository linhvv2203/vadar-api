// <copyright file="MultiChannelAlertsRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using VADAR.DTO;

namespace VADAR.DTO
{
    /// <summary>
    /// Multi-Channel Alerts Request Dto.
    /// </summary>
    public class MultiChannelAlertsRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public List<WorkspaceNotificationsRequestDto> WorkspaceNotifications { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
