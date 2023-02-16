// <copyright file="RolePermission.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Group Permission Data Model.
    /// </summary>
    public class RolePermission : BaseEntity
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
        [ForeignKey("RoleId")]
        [JsonIgnore]
        public virtual Role Role { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid RoleId { get; set; }
    }
}
