// <copyright file="Role.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Group Data Model.
    /// </summary>
    public class Role : AuditableEntity
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

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
        public virtual ICollection<RoleUser> RoleUsers { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
