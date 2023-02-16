// <copyright file="HostDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VADAR.DTO
{
    /// <summary>
    /// HostDto.
    /// </summary>
    public class HostDto
    {
        /// <summary>
        /// Gets or sets host Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string NameEngine { get; set; }

        /// <summary>
        /// Gets or sets.
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
        /// Gets or sets type of devices.
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public string TokenWorkspace { get; set; }

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
        [JsonProperty("MACHINE_ID")]
        public string MachineId { get; set; }

        /// <summary>
        /// Gets or sets groups.
        /// </summary>
        public string[] Groups { get; set; }
    }
}
