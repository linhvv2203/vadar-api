// <copyright file="WorkspaceRolePermission.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Workspace Permission Data Model.
    /// </summary>
    public class WorkspaceRolePermission : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("PermissionId")]
        [JsonIgnore]
        public virtual Permission Permission { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public long PermissionId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("WorkspaceRoleId")]
        [JsonIgnore]
        public virtual WorkspaceRole WorkspaceRole { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid WorkspaceRoleId { get; set; }
    }
}
