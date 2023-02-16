// <copyright file="NotificationSetting.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VADAR.Model.Models
{
    /// <summary>
    /// NotificationSetting.
    /// </summary>
    public class NotificationSetting
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
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
        /// Gets or sets Workspace Id.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets Workspace.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        public virtual Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<NotificationSettingCondition> NotificationSettingConditions { get; set; }
    }
}
