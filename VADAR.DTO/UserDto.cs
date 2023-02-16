// <copyright file="UserDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VADAR.DTO
{
    /// <summary>
    /// User Dto.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets.
        /// </summary>
        public bool IsProfileUpdated { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets join Date.
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public long[] PermissionIds { get; set; }

        /// <summary>
        /// Gets or sets Tags.
        /// </summary>
        public virtual IEnumerable<LanguageDto> Languages { get; set; }

        /// <summary>
        /// Gets or sets UserClaims.
        /// </summary>
        public virtual ICollection<UserClaimDto> UserClaims { get; set; }
    }
}
