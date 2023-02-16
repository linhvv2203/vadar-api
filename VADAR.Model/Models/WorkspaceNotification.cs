// <copyright file="WorkspaceNotification.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VADAR.Helpers.Enums;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Workspace Notification Model.
    /// </summary>
    public class WorkspaceNotification : AuditableEntity
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets WorkspaceId.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets Notification Type.
        /// </summary>
        public EnNotificationType Type { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        public virtual Workspace Workspace { get; set; }
    }
}
