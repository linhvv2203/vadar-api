// <copyright file="SecurityEventReturnDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// LogSecurityData Dto.
    /// </summary>
    public partial class SecurityEventReturnDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Description { get; set; }
    }
}
