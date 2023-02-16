// <copyright file="ILogsService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Logs Service Interface.
    /// </summary>
    public interface ILogsService
    {
        /// <summary>
        /// Get Logs performance paging.
        /// </summary>
        /// <param name="logsPerfromanceRequest">logsPerfromanceRequest.</param>
        /// <returns>LogsPerformanceResultPaging.</returns>
        Task<LogsPerformanceResultPagingDto> GetLogsPerformancePaging(LogsPerformanceRequestDto logsPerfromanceRequest);

        /// <summary>
        /// Get Logs Security.
        /// </summary>
        /// <param name="logSecurityRequest">logSecurityRequest.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<LogsSecurityResultPagingDto> GetLogSecurityPaging(LogSecurityRequestDto logSecurityRequest);

        /// <summary>
        /// GetLogsNetworkPaging.
        /// </summary>
        /// <param name="logsNetworkRequestDto">logsNetworkRequestDto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<LogsNetworkResultPagingDto> GetLogsNetworkPaging(LogsNetworkRequestDto logsNetworkRequestDto);

        /// <summary>
        /// GetLogSecuritySummary.
        /// </summary>
        /// <param name="logSecurityRequest">logsNetworkRequestDto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<LogsSecuritySummaryResultDto> GetLogSecuritySummary(LogSecurityRequestDto logSecurityRequest);
    }
}
