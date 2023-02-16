// <copyright file="RoleUser.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Group User Data Model.
    /// </summary>
    public class RoleUser : BaseEntity
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
