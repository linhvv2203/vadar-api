// <copyright file="IAgentInstallService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// IAgentInstallService Service.
    /// </summary>
    public interface IAgentInstallService
    {
        /// <summary>
        /// Lis GetListAgentIntallByWorkspace.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>List AgentInstallWorkspaceViewDto.</returns>
        Task<IEnumerable<AgentOsDto>> GetListAgentIntallByWorkspace(int workspaceId, string currentUserId);
    }
}
