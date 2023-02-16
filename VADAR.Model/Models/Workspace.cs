// <copyright file="Workspace.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Country Class.
    /// </summary>
    public class Workspace : AuditableEntity
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

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
        /// Gets or sets grafanaFolderUID.
        /// </summary>
        public string GrafanaFolderUID { get; set; }

        /// <summary>
        /// Gets or sets GrafanaInventoryDashboardUrl.
        /// </summary>
        public string GrafanaInventoryDashboardUrl { get; set; }

        /// <summary>
        /// Gets or sets GrafanaInventoryDashboardId.
        /// </summary>
        public long GrafanaInventoryDashboardId { get; set; }

        /// <summary>
        /// Gets or sets GrafanaSecurityDashboardUrl.
        /// </summary>
        public string GrafanaSecurityDashboardUrl { get; set; }

        /// <summary>
        /// Gets or sets GrafanaSecurityDashboardId.
        /// </summary>
        public long GrafanaSecurityDashboardId { get; set; }

        /// <summary>
        /// Gets or sets GrafanaPerformanceDashboardUrl.
        /// </summary>
        public string GrafanaPerformanceDashboardUrl { get; set; }

        /// <summary>
        /// Gets or sets GrafanaPerformanceDashboardId.
        /// </summary>
        public long GrafanaPerformanceDashboardId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets OrgId.
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// Gets or sets GrafanaOrgId.
        /// </summary>
        public long GrafanaOrgId { get; set; }

        /// <summary>
        /// Gets or sets License Id.
        /// </summary>
        public Guid? LicenseId { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        [ForeignKey("LicenseId")]
        public virtual License License { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceRole> WorkspaceRoles { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspacePolicy> WorkspacePolicies { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WhiteIp> WhiteLists { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<AgentOs> AgentOs { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<WorkspaceNotification> WorkspaceNotifications { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual ICollection<NotificationSetting> NotificationSettings { get; set; }
    }
}
