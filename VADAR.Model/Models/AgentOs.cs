// <copyright file="AgentOs.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VADAR.Model.Models
{
    /// <summary>
    /// AgentOs Data Model.
    /// </summary>
    public class AgentOs : AuditableEntity
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets Icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets name.
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
        public virtual Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public ICollection<AgentInstall> AgentInstalls { get; set; }
    }
}
