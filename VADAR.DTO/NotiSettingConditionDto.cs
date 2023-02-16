// <copyright file="NotiSettingConditionDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Notifications Setting Condition Dto.
    /// </summary>
    public class NotiSettingConditionDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Condition Type.
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets Notification Type.
        /// </summary>
        public int NotificationType { get; set; }

        /// <summary>
        /// Gets or sets Notification Setting Id.
        /// </summary>
        public int NotificationSettingId { get; set; }
    }
}
