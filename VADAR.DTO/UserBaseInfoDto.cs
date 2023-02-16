// <copyright file="UserBaseInfoDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace VADAR.DTO
{
    /// <summary>
    /// User Base Information Class.
    /// </summary>
    public class UserBaseInfoDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets user Name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Full Name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets Avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets email Address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets roles List.
        /// </summary>
        public Guid[] RoleId { get; set; }

        /// <summary>
        /// Gets or sets Permissions.
        /// </summary>
        public IEnumerable<long> SystemPermissions { get; set; }

        /// <summary>
        /// Gets or sets UserClaims.
        /// </summary>
        public virtual ICollection<UserClaimDto> UserClaims { get; set; }
    }
}
