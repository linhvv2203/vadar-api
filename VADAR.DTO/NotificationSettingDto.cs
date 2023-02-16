// <copyright file="NotificationSettingDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VADAR.DTO
{
    /// <summary>
    /// Notification Setting Dto.
    /// </summary>
    public class NotificationSettingDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Event Name.
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Activate.
        /// </summary>
        public bool Activate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<NotiSettingConditionDto> NotificationSettingConditions { get; set; }
    }
}
