// <copyright file="IGrafanaHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Grafana Helper Interface.
    /// </summary>
    public interface IGrafanaHelper
    {
        /// <summary>
        /// Get Grafana Account Detail.
        /// </summary>
        /// <param name="email">Email Address.</param>
        /// <returns>Account data.</returns>
        Task<GrafanaAccountDto> GetGrafanaAccountDetail(string email);

        /// <summary>
        /// Logout All Devices.
        /// </summary>
        /// <param name="grafanaAccountId">Grafana Account Id.</param>
        /// <returns>Logout Result.</returns>
        Task<dynamic> LogoutAllDevices(long grafanaAccountId);

        /// <summary>
        /// Create new Folder.
        /// </summary>
        /// <param name="folderTitle">Folder Title.</param>
        /// <returns>Folder Object.</returns>
        Task<GrafanaFolderDto> CreateNewFolder(string folderTitle);

        /// <summary>
        /// Delete Folder.
        /// </summary>
        /// <param name="uId">Uid of the folder.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> DeleteFolder(string uId);

        /// <summary>
        /// Create new performance Dashboard in a folder.
        /// </summary>
        /// <param name="folderId">Folder Id.</param>
        /// <param name="dashboardTitle">Dashboard Title.</param>
        /// <param name="workspaceGroupName">Workspace Wazhu group name.</param>
        /// <returns>Dashboard Object.</returns>
        Task<GrafanaDashboardDto> ImportPerformanceDashboard(long folderId, string dashboardTitle, string workspaceGroupName);

        /// <summary>
        /// Create new security Dashboard in a folder.
        /// </summary>
        /// <param name="folderId">Folder Id.</param>
        /// <param name="dashboardTitle">Dashboard Title.</param>
        /// <param name="workspaceGroupName">Workspace Wazhu group name.</param>
        /// <returns>Dashboard Object.</returns>
        Task<GrafanaDashboardDto> ImportSecurityDashboard(long folderId, string dashboardTitle, string workspaceGroupName);

        /// <summary>
        /// Create new inventory Dashboard in a folder.
        /// </summary>
        /// <param name="folderId">Folder Id.</param>
        /// <param name="dashboardTitle">Dashboard Title.</param>
        /// <param name="workspaceGroupName">Workspace Wazhu group name.</param>
        /// <returns>Dashboard Object.</returns>
        Task<GrafanaDashboardDto> ImportInventoryDashboard(long folderId, string dashboardTitle, string workspaceGroupName);

        /// <summary>
        /// Assign Dashboard Permission To User By Email.
        /// </summary>
        /// <param name="email">User Email.</param>
        /// <param name="dashboardId">Dashboard Id.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> AssignPermissionToUserByEmail(string email, long dashboardId);

        /// <summary>
        /// Get Dashboard Permissions.
        /// </summary>
        /// <param name="dashboardId">Dashboard Id.</param>
        /// <returns>Grafana Permissions.</returns>
        Task<IEnumerable<GrafanaPermissionDto>> GetDashboardPermissions(long dashboardId);

        /// <summary>
        /// Update Dashboard Permissions.
        /// </summary>
        /// <param name="permissions">Permissions List.</param>
        /// <param name="dashboardId">Dashboard Id.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> UpdateDashboardPermission(IEnumerable<GrafanaPermissionDto> permissions, long dashboardId);

        /// <summary>
        /// Create new Grafana Account.
        /// </summary>
        /// <param name="account">Account Object.</param>
        /// <returns>true: success;false:failed.</returns>
        Task<bool> CreateGrafanaAccount(GrafanaAccountDto account);

        /// <summary>
        /// Set deafult folder permission.
        /// </summary>
        /// <param name="folderUID">Folder UID.</param>
        /// <param name="permissions">Permissions.</param>
        /// <returns>Task completed.</returns>
        Task SetDefaultFolderPermission(string folderUID, IEnumerable<GrafanaPermissionDto> permissions = null);

        /// <summary>
        /// Remove dashboard permission.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="dashboardId">DashboardId.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> RemoveDashboardPermission(string email, long dashboardId);

        /// <summary>
        /// Get Admin Account Default Organization.
        /// </summary>
        /// <returns>Grafana Organization.</returns>
        Task<GrafanaOrganizationDto> GetAdminAccountDefaultOrganization();

        /// <summary>
        /// Add User To Organization.
        /// </summary>
        /// <param name="userEmail">User email.</param>
        /// <returns>Task Completed.</returns>
        Task AddUserToOrganization(string userEmail);
    }
}
