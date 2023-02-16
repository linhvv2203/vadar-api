// <copyright file="Policy.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Policy Model.
    /// </summary>
    public class Policy
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspacePolicy> WorkspacePolicies { get; set; }
    }
}
