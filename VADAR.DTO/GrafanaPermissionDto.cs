// <copyright file="GrafanaPermissionDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Grafana Permission Dto.
    /// </summary>
    public class GrafanaPermissionDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets permission.
        /// </summary>
        public long Permission { get; set; }

        /// <summary>
        /// Gets or sets team Id.
        /// </summary>
        public long TeamId { get; set; }
    }
}
