// <copyright file="UserProfileDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// User Profile Dto.
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets Field Of Activity.
        /// </summary>
        public IEnumerable<UserClaimDto> UserClaims { get; set; }

        /// <summary>
        /// Gets or sets Tags.
        /// </summary>
        public IEnumerable<LanguageDto> Languages { get; set; }
    }
}
