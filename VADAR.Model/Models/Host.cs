// <copyright file="Host.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VADAR.Helpers.Enums;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Host Data Model.
    /// </summary>
    public class Host : AuditableEntity
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string NameEngine { get; set; }

        /// <summary>
        /// Gets or sets type of divices.
        /// </summary>
        public EnTypeOfDevices Type { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets. WINDOW , CENTOS , UBUTU , MACOS.
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets. 0: inactive, 1: active.
        /// </summary>
        public int Status { get; set; }

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
        public string MachineId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<GroupHost> GroupHosts { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceHost> WorkspaceHosts { get; set; }
    }
}
