// <copyright file="IElasticSearchCallApiHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Call Api helper Interface.
    /// </summary>
    public interface IElasticSearchCallApiHelper
    {
        /// <summary>
        /// CreateNotification.
        /// </summary>
        /// <param name="notificationDto">notificationDto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> CreateNotification(NotificationDto notificationDto);

        /// <summary>
        /// Create Notification Error.
        /// </summary>
        /// <param name="body">body.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> CreateNotificationError(string body);

        /// <summary>
        /// GetNetworkLog.
        /// </summary>
        /// <param name="logsNetworkRequestDto">logsNetworkRequestDto.</param>
        /// <returns>response string.</returns>
        Task<string> GetNetworkLog(LogsNetworkRequestDto logsNetworkRequestDto);

        /// <summary>
        /// GetNetworkLog.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <param name="from">level.</param>
        /// <param name="to">to.</param>
        /// <returns>response string.</returns>
        Task<string> GeTotaltSecurityEventBetweenLevel(HostStatisticRequestDto dataRequest, int from = 10, int to = 15);

        /// <summary>
        /// GetPerformanceLog.
        /// </summary>
        /// <param name="logsPerformanceRequest">logsPerformanceRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetPerformanceLog(LogsPerformanceRequestDto logsPerformanceRequest);

        /// <summary>
        /// GetLogSecurity.
        /// </summary>
        /// <param name="logSecurityRequest">logSecurityRequest.</param>
        /// <param name="sort">sort.</param>
        /// <returns>response string.</returns>
        Task<string> GetLogSecurity(LogSecurityRequestDto logSecurityRequest, string sort = "desc");

        /// <summary>
        /// GetLogSecurity.
        /// </summary>
        /// <param name="logSecurityRequest">logSecurityRequest.</param>
        /// <param name="sort">sort.</param>
        /// <returns>response string.</returns>
        Task<string> GetLogSecurityMoreThanLevel9(LogSecurityRequestDto logSecurityRequest, string sort = "");

        /// <summary>
        /// GetDashboardSummary.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetDashboardSummary(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetHostStatistic.
        /// </summary>
        /// /// <param name="dataRequest">logsPerformanceRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetHostStatistic(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetEventInfomation.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetSecurityEventByTime(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetPerformanceEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetPerformanceEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop10SecurityEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetTop10SecurityEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop10AttackIP.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <param name="ruleId">ruleId.</param>
        /// <returns>response string.</returns>
        Task<string> GetTop10AttackIP(HostStatisticRequestDto dataRequest, int ruleId);

        /// <summary>
        /// GetLast10SecurityEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetLast10SecurityEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetLast10PeformanceEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetLast10PerformanceEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetTopEventsByLevel(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetHostsInEvents(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetAlertsEvolutionOverTime(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// Get Top Events By Level.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>response string.</returns>
        Task<string> GetAgentsStatus(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// Get Most Common CVEs.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetMostCommonCVEs(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetVulnerabilitiesSummary.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetVulnerabilitiesSummary(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop5SecurityEvent.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetTop5SecurityEvent(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetMostCommonCWEs.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetMostCommonCWEs(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetMostAffectedAgents.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetMostAffectedAgents(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetAlertsSeverity.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetAlertsSeverity(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetAlertsByActionOverTime.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetAlertsByActionOverTime(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop5AgentIntegrityMonitoring.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetTop5AgentIntegrityMonitoring(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetEventSummaryIntegrityMonitoring.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetEventSummaryIntegrityMonitoring(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetTop10AgentsByAlertsNumber.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetTop10AgentsByAlertsNumber(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetPCIDSSRequirements.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetPCIDSSRequirements(HostStatisticRequestDto dataRequest);

        /// <summary>
        /// GetGroupLogsByCondition.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetGroupLogsByCondition(LogSecurityRequestDto dataRequest);

        /// <summary>
        /// GetRuleGroupByAgentName.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetRuleGroupByAgentName(LogSecurityRequestDto dataRequest);
    }
}
