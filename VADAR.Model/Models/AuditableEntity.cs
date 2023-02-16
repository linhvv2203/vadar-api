// <copyright file="AuditableEntity.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Auditable Entity Class.
    /// </summary>
    public class AuditableEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UpdateById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("UpdateById")]
        [JsonIgnore]
        public virtual User UpdateBy { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

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
    }
}
