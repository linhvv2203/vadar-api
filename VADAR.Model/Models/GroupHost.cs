// <copyright file="GroupHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Group Host Data Model.
    /// </summary>
    public class GroupHost : BaseEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("HostId")]
        [JsonIgnore]
        public Host Host { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Column(Order = 0)]
        public Guid HostId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("GroupId")]
        [JsonIgnore]
        public Group Group { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Column(Order = 1)]
        public Guid GroupId { get; set; }
    }
}
