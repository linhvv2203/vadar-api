// <copyright file="Group.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Group Data Model.
    /// </summary>
    public class Group : AuditableEntity
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Description.
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
        public string ZabbixRef { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string WazuhRef { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<GroupHost> GroupHosts { get; set; }
    }
}
