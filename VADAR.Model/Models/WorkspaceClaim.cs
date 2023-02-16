// <copyright file="WorkspaceClaim.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VADAR.Model.Models;

namespace VADAR.Model
{
    /// <summary>
    /// Workspace Claim Model.
    /// </summary>
    public class WorkspaceClaim : BaseEntity
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets workspaceId.
        /// </summary>
        public int WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets Workspace.
        /// </summary>
        [ForeignKey("WorkspaceId")]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets Claim Type.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets Claim Value.
        /// </summary>
        public string ClaimValue { get; set; }
    }
}
