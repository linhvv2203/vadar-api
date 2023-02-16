// <copyright file="DashboardController.cs" company="VSEC">
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
    /// DashboardController.
    /// </summary>
    // [ApiVersion("1.0")]
    public class DashboardController : BaseController
    {
        private readonly ILoggerHelper<DashboardController> logger;
        private readonly IDashboardService dashboardService;

        /// <summary>
        /// Initialises a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="dashboardService">dashboardService.</param>
        public DashboardController(ILoggerHelper<DashboardController> logger, IDashboardService dashboardService)
        {
            this.logger = logger;
            this.dashboardService = dashboardService;
        }

        /// <summary>
        /// Get Dashboard Summary.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetDashboardSummary/{workspaceId}")]
        public async Task<ApiResponse<SummaryDto>> GetDashboardSummary(int? workspaceId)
        {
            try
            {
                var dataRequest = new HostStatisticRequestDto();
                dataRequest.RequestUserId = this.CurrentUserId;
                dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                dataRequest.ToDate = DateTime.UtcNow;
                dataRequest.WorkSpaceId = workspaceId;
                var result = await this.dashboardService.GetDashboardSummarys(dataRequest);
                return new ApiResponse<SummaryDto>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<SummaryDto>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Host Problem.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetHostProblem/{workspaceId}")]
        public async Task<ApiResponse<List<HostDto>>> GetHostProblem(int? workspaceId)
        {
            try
            {
                var result = await this.dashboardService.ShowHostProblem(workspaceId);
                return new ApiResponse<List<HostDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<HostDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetTopEventsByLevel")]
        public async Task<ApiResponse<List<TopEventByLevelDto>>> GetTopEventsByLevel([FromQuery] LogSecurityRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetTopEventsByLevel(dataRequest);
                return new ApiResponse<List<TopEventByLevelDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<TopEventByLevelDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetAgentsStatus")]
        public async Task<ApiResponse<List<EventChartReturnDto>>> GetAgentsStatus([FromQuery] LogSecurityRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetAgentsStatus(dataRequest);
                return new ApiResponse<List<EventChartReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetAlertsEvolutionOverTime")]
        public async Task<ApiResponse<List<EventChartReturnDto>>> GetAlertsEvolutionOverTime([FromQuery] LogSecurityRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetAlertsEvolutionOverTime(dataRequest);
                return new ApiResponse<List<EventChartReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetHostStatistics.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetHostStatistics/{workspaceId}")]
        public async Task<ApiResponse<List<HostStatisticDto>>> GetHostStatistics(int? workspaceId)
        {
            try
            {
                var dataRequest = new HostStatisticRequestDto();
                dataRequest.RequestUserId = this.CurrentUserId;
                dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                dataRequest.ToDate = DateTime.UtcNow;
                dataRequest.WorkSpaceId = workspaceId;
                var result = await this.dashboardService.GetHostStatistics(dataRequest);
                return new ApiResponse<List<HostStatisticDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<HostStatisticDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Event Information.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>Report data.</returns>
        [HttpPost]
        [Route("GetSecurityEventByTime")]
        public async Task<ApiResponse<List<EventChartReturnDto>>> GetSecurityEventByTime([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                if (dataRequest.FromDate == null || dataRequest.ToDate == null)
                {
                    dataRequest.FromDate = DateTime.UtcNow.AddMonths(-1);
                    dataRequest.ToDate = DateTime.UtcNow;
                }

                var result = await this.dashboardService.GetSecurityEventByTime(dataRequest);
                return new ApiResponse<List<EventChartReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetVulnerabilitiesSummary.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetVulnerabilitiesSummary")]
        public async Task<ApiResponse<List<VulnerabilitiesSummaryDto>>> GetVulnerabilitiesSummary([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetVulnerabilitiesSummary(dataRequest);
                return new ApiResponse<List<VulnerabilitiesSummaryDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<VulnerabilitiesSummaryDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetEventSummaryIntegrityMonitoring.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetEventSummaryIntegrityMonitoring")]
        public async Task<ApiResponse<List<EventChartDto>>> GetEventSummaryIntegrityMonitoring([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetEventSummaryIntegrityMonitoring(dataRequest);
                return new ApiResponse<List<EventChartDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetAlertsSeverity.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetAlertsSeverity")]
        public async Task<ApiResponse<List<EventChartReturnDto>>> GetAlertsSeverity([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetAlertsSeverity(dataRequest);
                return new ApiResponse<List<EventChartReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetAlertsByActionOverTime.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetAlertsByActionOverTime")]
        public async Task<ApiResponse<List<EventChartReturnDto>>> GetAlertsByActionOverTime([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetAlertsByActionOverTime(dataRequest);
                return new ApiResponse<List<EventChartReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetMostCommonCVEs.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetMostCommonCVEs")]
        public async Task<ApiResponse<List<CVEsEventDto>>> GetMostCommonCVEs([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetMostCommonCVEs(dataRequest);
                return new ApiResponse<List<CVEsEventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<CVEsEventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetMostCommonCWEs.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetMostCommonCWEs")]
        public async Task<ApiResponse<List<CVEsEventDto>>> GetMostCommonCWEs([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetMostCommonCWEs(dataRequest);
                return new ApiResponse<List<CVEsEventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<CVEsEventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetMostAffectedAgents.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetMostAffectedAgents")]
        public async Task<ApiResponse<List<EventDto>>> GetMostAffectedAgents([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetMostAffectedAgents(dataRequest);
                return new ApiResponse<List<EventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetPerformanceEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>Report data.</returns>
        [HttpPost]
        [Route("GetPerformanceEvent")]
        public async Task<ApiResponse<List<EventChartReturnDto>>> GetPerformanceEvent([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                if (dataRequest.FromDate == null || dataRequest.ToDate == null)
                {
                    dataRequest.FromDate = DateTime.UtcNow.AddMonths(-1);
                    dataRequest.ToDate = DateTime.UtcNow;
                }

                dataRequest.WorkSpaceId = dataRequest.WorkSpaceId;
                var result = await this.dashboardService.GetPerformanceEvent(dataRequest);
                return new ApiResponse<List<EventChartReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventChartReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetTop10SecurityEvent.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetTop10SecurityEvent/{workspaceId}")]
        public async Task<ApiResponse<List<EventDto>>> GetTop10SecurityEvent(int? workspaceId)
        {
            try
            {
                var dataRequest = new HostStatisticRequestDto();
                dataRequest.RequestUserId = this.CurrentUserId;
                dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                dataRequest.ToDate = DateTime.UtcNow;
                dataRequest.WorkSpaceId = workspaceId;
                var result = await this.dashboardService.GetTop10SecurityEvent(dataRequest);
                return new ApiResponse<List<EventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetTop10AttackIP.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetTop10AttackIP/{workspaceId}")]
        public async Task<ApiResponse<List<EventDto>>> GetTop10AttackIP(int? workspaceId)
        {
            try
            {
                var dataRequest = new HostStatisticRequestDto();
                dataRequest.RequestUserId = this.CurrentUserId;
                dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                dataRequest.ToDate = DateTime.UtcNow;
                dataRequest.WorkSpaceId = workspaceId;
                var ruleId = 601;
                var result = await this.dashboardService.GetTop10AttackIP(dataRequest, ruleId);
                return new ApiResponse<List<EventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetLast10SecurityEvent.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetLast10SecurityEvent/{workspaceId}")]
        public async Task<ApiResponse<List<SecurityEventReturnDto>>> GetLast10SecurityEvent(int? workspaceId)
        {
            try
            {
                var dataRequest = new HostStatisticRequestDto();
                dataRequest.RequestUserId = this.CurrentUserId;
                dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                dataRequest.ToDate = DateTime.UtcNow;
                dataRequest.WorkSpaceId = workspaceId;
                var result = await this.dashboardService.GetLast10SecurityEvent(dataRequest);
                return new ApiResponse<List<SecurityEventReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<SecurityEventReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetLast10PerformanceEvent.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <returns>Report data.</returns>
        [HttpGet]
        [Route("GetLast10PerformanceEvent/{workspaceId}")]
        public async Task<ApiResponse<List<PerformanceEventReturnDto>>> GetLast10PerformanceEvent(int? workspaceId)
        {
            try
            {
                var dataRequest = new HostStatisticRequestDto();
                dataRequest.RequestUserId = this.CurrentUserId;
                dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                dataRequest.ToDate = DateTime.UtcNow;
                dataRequest.WorkSpaceId = workspaceId;
                var result = await this.dashboardService.GetLast10PerformanceEvent(dataRequest);
                return new ApiResponse<List<PerformanceEventReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<PerformanceEventReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetTop5SecurityEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetTop5SecurityEvent")]
        public async Task<ApiResponse<List<EventDto>>> GetTop5SecurityEvent([FromBody]HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetTop5SecurityEvent(dataRequest);
                return new ApiResponse<List<EventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetTop5AgentIntegrityMonitoring.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetTop5AgentIntegrityMonitoring")]
        public async Task<ApiResponse<List<EventDto>>> GetTop5AgentIntegrityMonitoring([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetTop5AgentIntegrityMonitoring(dataRequest);
                return new ApiResponse<List<EventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetTopRequirementsOverTime.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetTopRequirementsOverTime")]
        public async Task<ApiResponse<List<ChartLineReturnDto>>> GetTopRequirementsOverTime([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetTopRequirementsOverTime(dataRequest);
                return new ApiResponse<List<ChartLineReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<ChartLineReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetTop10AgentsByAlertsNumber.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetTop10AgentsByAlertsNumber")]
        public async Task<ApiResponse<List<EventDto>>> GetTop10AgentsByAlertsNumber([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetTop10AgentsByAlertsNumber(dataRequest);
                return new ApiResponse<List<EventDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<EventDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetPCIDSSRequirements.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetPCIDSSRequirements")]
        public async Task<ApiResponse<List<ChartLineReturnDto>>> GetPCIDSSRequirements([FromBody] HostStatisticRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetPCIDSSRequirements(dataRequest);
                return new ApiResponse<List<ChartLineReturnDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<ChartLineReturnDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetRuleByAgentName.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("GetRuleByAgentName")]
        public async Task<ApiResponse<List<RuleGroupDto>>> GetRuleByAgentName([FromBody] LogSecurityRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GroupRuleByAgentName(dataRequest);
                return new ApiResponse<List<RuleGroupDto>>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<RuleGroupDto>>(ex.HResult);
            }
        }
    }
}
