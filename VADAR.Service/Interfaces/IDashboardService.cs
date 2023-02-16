// <copyright file="IDashboardService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// IDashboard Service.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Get Dashboard Summarys.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// /// <param name="dataRequest">dataRequest.</param>
        Task<SummaryDto> GetDashboardSummarys(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Host Statistics.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<HostStatisticDto>> GetHostStatistics(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Security Event By Time.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<EventChartReturnDto>> GetSecurityEventByTime(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Performance Event.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<EventChartReturnDto>> GetPerformanceEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Top 10 Security Event.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<EventDto>> GetTop10SecurityEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Top 10 Attack IP.
        /// </summary>/// <param name="dataRequest">dataRequest.</param>
        /// /// <param name="ruleId">ruleId.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<EventDto>> GetTop10AttackIP(HostStatisticRequestDto dataRequest, int ruleId);

        /// <summary>
        /// Get Last 10 Security Event.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<SecurityEventReturnDto>> GetLast10SecurityEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Last 10 Performance Event.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<PerformanceEventReturnDto>> GetLast10PerformanceEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<TopEventByLevelDto>> GetTopEventsByLevel(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// Get Most Common CVEs.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<CVEsEventDto>> GetMostCommonCVEs(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetVulnerabilitiesSummary.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<VulnerabilitiesSummaryDto>> GetVulnerabilitiesSummary(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop5SecurityEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventDto>> GetTop5SecurityEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetMostCommonCWEs.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<CVEsEventDto>> GetMostCommonCWEs(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetMostAffectedAgents.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventDto>> GetMostAffectedAgents(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetAlertsSeverity.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventChartReturnDto>> GetAlertsSeverity(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetAlertsEvolutionOverTime.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventChartReturnDto>> GetAlertsEvolutionOverTime(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// GetAlertsByActionOverTime.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventChartReturnDto>> GetAlertsByActionOverTime(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop5AgentIntegrityMonitoring.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventDto>> GetTop5AgentIntegrityMonitoring(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetEventSummaryIntegrityMonitoring.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventChartDto>> GetEventSummaryIntegrityMonitoring(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTopRequirementsOverTime.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ChartLineReturnDto>> GetTopRequirementsOverTime(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop10AgentsByAlertsNumber.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventDto>> GetTop10AgentsByAlertsNumber(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetPCIDSSRequirements.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ChartLineReturnDto>> GetPCIDSSRequirements(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetPCIDSSRequirements.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<EventChartReturnDto>> GetAgentsStatus(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// ShowHostProblem.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<HostDto>> ShowHostProblem(int? workspaceId);

        /// <summary>
        /// GetGroupLogsByCondition.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<GroupLogsResultDto> GetGroupLogsByCondition(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// GroupRuleByAgentName.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<RuleGroupDto>> GroupRuleByAgentName(LogSecurityRequestDto dataRequest);
    }
}
