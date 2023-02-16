// <copyright file="User.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VADAR.Model.Models
{
    /// <summary>
    /// User.
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        [Key]
        [MaxLength(150)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Full Name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets.
        /// </summary>
        public bool IsProfileUpdated { get; set; }

        /// <summary>
        /// Gets or sets. 1: pending, 2: active, 3: blocked, 4: rejected, 5: cancel.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets join Date.
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// Gets or sets Approved By.
        /// </summary>
        [ForeignKey("ApprovedById")]
        [JsonIgnore]
        public virtual User ApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets Approved By.
        /// </summary>
        [MaxLength(150)]
        public string ApprovedById { get; set; }

        /// <summary>
        /// Gets or sets Approved Date.
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Gets or sets Approver Comment.
        /// </summary>
        public string ApproverComment { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("CountryId")]
        [JsonIgnore]
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<UserClaim> UserClaims { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<UserLanguage> UserLanguages { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<RoleUser> RoleUsers { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceRoleUser> WorkspaceRoleUsers { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<InviteWorkspaceRole> InviteWorkspaceRoles { get; set; }
    }
}
