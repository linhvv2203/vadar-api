// <copyright file="AgentInstallController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers.BaseControllers
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AgentInstallController"/> class.
    /// </summary>
    public class AgentInstallController : BaseController
    {
        private readonly ILoggerHelper<AgentInstallController> logger;
        private readonly IAgentInstallService agentInstallService;

        /// <summary>
        /// Initialises a new instance of the <see cref="AgentInstallController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="agentInstallService">agentInstallService.</param>
        public AgentInstallController(
            ILoggerHelper<AgentInstallController> logger,
            IAgentInstallService agentInstallService)
        {
            this.logger = logger;
            this.agentInstallService = agentInstallService;
        }

        /// <summary>
        /// GetAgentInstallByWorkspace.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetAgentInstallByWorkspace/{workspaceId}")]
        public async Task<ApiResponse<IEnumerable<AgentOsDto>>> GetAgentInstallByWorkspace(int workspaceId)
        {
            try
            {
                var results = await this.agentInstallService.GetListAgentIntallByWorkspace(workspaceId, this.CurrentUserId);
                return new ApiResponse<IEnumerable<AgentOsDto>>(EnApiStatusCode.Success, results);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<IEnumerable<AgentOsDto>>(ex.HResult);
            }
        }
    }
}
