// <copyright file="IVadarAlertHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Vadar Alert Helper Interface.
    /// </summary>
    public interface IVadarAlertHelper
    {
        /// <summary>
        /// List Email Alerts.
        /// </summary>
        /// <param name="workspaceName">workspaceName.</param>
        /// <returns>List Email.</returns>
        Task<List<string>> ListEmailAlerts(string workspaceName);

        /// <summary>
        /// Init Alerts.
        /// </summary>
        /// <param name="alertsDto">alertsDto.</param>
        /// <returns>True: Success, False: Fail.</returns>
        Task<bool> InitAlerts(AlertsDto alertsDto);

        /// <summary>
        /// Add Email To Alerts.
        /// </summary>
        /// <param name="alertsDto">alertsDto.</param>
        /// <returns>True: Success, False: Fail.</returns>
        Task<bool> AddEmailToAlerts(AlertsDto alertsDto);

        /// <summary>
        /// Remove Email From Alerts.
        /// </summary>
        /// <param name="alertsDto">alertsDto.</param>
        /// <returns>True: Success, False: Fail.</returns>
        Task<bool> RemoveEmailFromAlerts(AlertsDto alertsDto);

        /// <summary>
        /// Build Agent Centos For Workspace.
        /// </summary>
        /// <param name="buildAgentForWorkspaceDto">buildAgentForWorkspaceDto.</param>
        /// <returns>job Id.</returns>
        Task<List<AgentInstallDto>> BuildAgentCentosForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto);

        /// <summary>
        /// Build Agent Window For Workspace.
        /// </summary>
        /// <param name="buildAgentForWorkspaceDto">buildAgentForWorkspaceDto.</param>
        /// <returns>List AgentInstallDto.</returns>
        List<AgentInstallDto> BuildAgentWindowForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto);

        /// <summary>
        /// BuildAgentUbuntuForWorkspace.
        /// </summary>
        /// <param name="buildAgentForWorkspaceDto">buildAgentForWorkspaceDto.</param>
        /// <returns>List AgentInstallDto.</returns>
        List<AgentInstallDto> BuildAgentUbuntuForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto);

        /// <summary>
        /// BuildAgentMacForWorkspace.
        /// </summary>
        /// <param name="buildAgentForWorkspaceDto">buildAgentForWorkspaceDto.</param>
        /// <returns>List AgentInstallDto.</returns>
        List<AgentInstallDto> BuildAgentMacForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto);

        /// <summary>
        /// BuildAgentCentosForWorkspaceV2.
        /// </summary>
        /// <param name="buildAgentForWorkspaceDto">buildAgentForWorkspaceDto.</param>
        /// <returns>List AgentInstallDto.</returns>
        List<AgentInstallDto> BuildAgentCentosForWorkspaceV2(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto);
    }
}
