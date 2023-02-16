// <copyright file="WorkspaceHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// WorkspaceHost Data Model.
    /// </summary>
    public class WorkspaceHost : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        public virtual Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        [Column(Order = 0)]
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("HostId")]
        [JsonIgnore]
        public virtual Host Host { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Column(Order = 1)]
        public Guid HostId { get; set; }
    }
}
