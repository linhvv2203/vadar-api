// <copyright file="WorkspaceRoleUser.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Workspace Users.
    /// </summary>
    public class WorkspaceRoleUser : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

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
