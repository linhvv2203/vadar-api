// <copyright file="InviteWorkspaceRole.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Invite WorkspaceRole Model.
    /// </summary>
    public class InviteWorkspaceRole : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("WorkspaceRoleId")]
        [JsonIgnore]
        public virtual WorkspaceRole WorkspaceRole { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid WorkspaceRoleId { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User InvitedUser { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// Gets or sets expired date.
        /// </summary>
        public DateTime ExpriredDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("CreatedById")]
        [JsonIgnore]
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string InviteTo { get; set; }
    }
}
