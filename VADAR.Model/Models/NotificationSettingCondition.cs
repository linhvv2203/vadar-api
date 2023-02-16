// <copyright file="NotificationSettingCondition.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VADAR.Helpers.Enums;

namespace VADAR.Model.Models
{
    /// <summary>
    /// NotificationSettingCondition Model.
    /// </summary>
    public class NotificationSettingCondition
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets condition.
        /// </summary>
        public EnConditionType Condition { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets Notification Type.
        /// </summary>
        public EnNotificationType NotificationType { get; set; }

        /// <summary>
        /// Gets or sets Notification Setting Id.
        /// </summary>
        public int NotificationSettingId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("NotificationSettingId")]
        public virtual NotificationSetting NotificationSetting { get; set; }
    }
}
