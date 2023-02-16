// <copyright file="WorkspaceRole.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Group Role Data Model.
    /// </summary>
    public class WorkspaceRole : AuditableEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        [JsonIgnore]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Column(Order = 0)]
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceRolePermission> WorkspaceRolePermissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceRoleUser> WorkspaceRoleUsers { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<InviteWorkspaceRole> InviteWorkspaceRoles { get; set; }
    }
}
