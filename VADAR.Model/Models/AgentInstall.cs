// <copyright file="AgentInstall.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VADAR.Model.Models
{
    /// <summary>
    /// NotificationReceiver Model.
    /// </summary>
    public class AgentInstall : AuditableEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets 0: Inactive, 1: Active.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string LinkDownload { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("OsId")]
        public virtual AgentOs AgentOs { get; set; }

        /// <summary>
        /// Gets or sets GroupNotificationId.
        /// </summary>
        public long OsId { get; set; }
    }
}
