// <copyright file="AlertSettingRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// Alert Setting Request Dto.
    /// </summary>
    public class AlertSettingRequestDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public NotificationSettingDto NotificationSettings { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
