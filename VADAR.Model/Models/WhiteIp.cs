// <copyright file="WhiteIp.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VADAR.Model.Models
{
    /// <summary>
    /// White List Model.
    /// </summary>
    public class WhiteIp
    {
        /// <summary>
        /// Gets or sets Ip.
        /// </summary>
        [Key]
        [MaxLength(150)]
        [Column(Order = 0)]
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        [Column(Order = 1)]
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets Workspace.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? CreatedDate { get; set; }
    }
}
