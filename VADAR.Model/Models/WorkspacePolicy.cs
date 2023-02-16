// <copyright file="WorkspacePolicy.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Workspace Policy Model.
    /// </summary>
    public class WorkspacePolicy
    {
        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        [Column(Order = 0)]
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        [JsonIgnore]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets Policy Id.
        /// </summary>
        [Column(Order = 1)]
        public int PolicyId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("PolicyId")]
        [JsonIgnore]
        public Policy Policy { get; set; }
    }
}
