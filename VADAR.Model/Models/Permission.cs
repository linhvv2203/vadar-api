// <copyright file="Permission.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Permission Data Model.
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Permission Type.
        /// 0: Workspace permission; 1: System permission.
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceRolePermission> WorkspaceRolePermissions { get; set; }
    }
}
