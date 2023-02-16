// <copyright file="WorkspaceDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;

namespace VADAR.DTO
{
    /// <summary>
    /// User Dto.
    /// </summary>
    public class WorkspaceDto
    {
        /// <summary>
        /// Gets or sets user Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string UpdateById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string TokenWorkspace { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string ZabbixRef { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string WazuhRef { get; set; }

        /// <summary>
        /// Gets or sets GrafanaInventoryDashboardUrl.
        /// </summary>
        public string GrafanaInventoryDashboardUrl { get; set; }

        /// <summary>
        /// Gets or sets GrafanaSecurityDashboardUrl.
        /// </summary>
        public string GrafanaSecurityDashboardUrl { get; set; }

        /// <summary>
        /// Gets or sets GrafanaPerformanceDashboardUrl.
        /// </summary>
        public string GrafanaPerformanceDashboardUrl { get; set; }

        /// <summary>
        /// Gets or sets GrafanaOrgId.
        /// </summary>
        public long GrafanaOrgId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public int HostLimit { get; set; }

        /// <summary>
        /// Gets or sets StartDate.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets EndDate.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public int Status { get; set; }
    }
}
