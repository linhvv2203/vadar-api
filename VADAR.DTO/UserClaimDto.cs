// <copyright file="UserClaimDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// UserClaimDto.
    /// </summary>
    public class UserClaimDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPublic.
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
