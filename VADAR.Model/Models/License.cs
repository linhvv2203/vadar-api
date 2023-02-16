// <copyright file="License.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VADAR.Model.Models
{
    /// <summary>
    /// License Model.
    /// </summary>
    public class License : AuditableEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        [MaxLength(36)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int HostLimit { get; set; }

        /// <summary>
        /// Gets or sets StartDate.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets EndDate.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
