// <copyright file="GrafanaDashboardDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Grafana Dashboard Dto.
    /// </summary>
    public class GrafanaDashboardDto
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public long DashboardId { get; set; }

        /// <summary>
        /// Gets or sets Imported Url.
        /// </summary>
        public string ImportedUrl { get; set; }
    }
}
