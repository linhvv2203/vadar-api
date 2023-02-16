// <copyright file="LogsController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// Logs Controller.
    /// </summary>
    public class LogsController : BaseController
    {
        private readonly ILoggerHelper<LogsController> logger;
        private readonly ILogsService logsService;
        private readonly IDashboardService dashboardService;

        /// <summary>
        /// Initialises a new instance of the <see cref="LogsController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="logsService">logsService.</param>
        /// <param name="dashboardService">dashboardService.</param>
        public LogsController(
            ILoggerHelper<LogsController> logger,
            ILogsService logsService,
            IDashboardService dashboardService)
        {
            this.logger = logger;
            this.logsService = logsService;
            this.dashboardService = dashboardService;
        }

        /// <summary>
        /// Get Logs performance Paging.
        /// </summary>
        /// <param name="logsPerformanceRequest">logsPerformanceRequest.</param>
        /// <returns>LogsPerformanceResultPaging.</returns>
        [HttpPost]
        [Route("GetLogsperformancePaging")]
        public async Task<ApiResponse<LogsPerformanceResultPagingDto>> GetLogsperformancePaging([FromBody] LogsPerformanceRequestDto logsPerformanceRequest)
        {
            try
            {
                logsPerformanceRequest.RequestUserId = this.CurrentUserId;
                return new ApiResponse<LogsPerformanceResultPagingDto>(EnApiStatusCode.Success, await this.logsService.GetLogsPerformancePaging(logsPerformanceRequest));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<LogsPerformanceResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Logs network Paging.
        /// </summary>
        /// <param name="logsNetworkRequestDto">logsNetworkRequestDto.</param>
        /// <returns>LogsPerformanceResultPaging.</returns>
        [HttpPost]
        [Route("GetLogsNetworkPaging")]
        public async Task<ApiResponse<LogsNetworkResultPagingDto>> GetLogsNetworkPaging([FromBody] LogsNetworkRequestDto logsNetworkRequestDto)
        {
            try
            {
                logsNetworkRequestDto.RequestUserId = this.CurrentUserId;

                if (logsNetworkRequestDto.WorkspaceId == 29 || logsNetworkRequestDto.WorkspaceId == 113)
                {
                    var result = new LogsNetworkResultPagingDto
                    {
                        Count = 3,
                        Items = new List<LogsNetworkResultDto>
                        {
                            new LogsNetworkResultDto
                            {
                                Time = "09/30/2020 04:42:26",
                                HostName = "vsec_jsc-ubuntu1804",
                                WorkspaceName = "VSEC_JSC",
                                Action = "Allow",
                                Class = "None",
                                SourceAddress = "192.168.100.128",
                                SourcePort = "43188",
                                DestinationAddress = "20.0.30.213",
                                DestinationPort = "10051",
                                Message = "(latency) packet fastpathed due to latency",
                            },
                            new LogsNetworkResultDto
                            {
                                Time = "09/30/2020 04:42:26",
                                HostName = "vsec_jsc-LINH",
                                WorkspaceName = "VSEC_JSC",
                                Action = "Allow",
                                Class = "None",
                                SourceAddress = "192.168.0.138",
                                SourcePort = "43188",
                                DestinationAddress = "20.0.30.213",
                                DestinationPort = "10054",
                                Message = "(latency) packet fastpathed due to latency",
                            },
                            new LogsNetworkResultDto
                            {
                                Time = "09/30/2020 04:42:26",
                                HostName = "vsec_jsc-ubuntu1804",
                                WorkspaceName = "VSEC_JSC",
                                Action = "Allow",
                                Class = "None",
                                SourceAddress = "192.168.100.128",
                                SourcePort = "43188",
                                DestinationAddress = "20.0.30.213",
                                DestinationPort = "10051",
                                Message = "(latency) packet fastpathed due to latency",
                            },
                        },
                    };

                    return new ApiResponse<LogsNetworkResultPagingDto>(EnApiStatusCode.Success, result);
                }

                return new ApiResponse<LogsNetworkResultPagingDto>(EnApiStatusCode.Success, await this.logsService.GetLogsNetworkPaging(logsNetworkRequestDto));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<LogsNetworkResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetLogSecurity.
        /// </summary>
        /// /// <param name="logSecurityRequest">logsPerformanceRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("GetLogSecurity")]
        public async Task<ApiResponse<LogsSecurityResultPagingDto>> GetLogSecurity([FromBody] LogSecurityRequestDto logSecurityRequest)
        {
            try
            {
                logSecurityRequest.RequestUserId = this.CurrentUserId;
                var result = await this.logsService.GetLogSecurityPaging(logSecurityRequest);
                return new ApiResponse<LogsSecurityResultPagingDto>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<LogsSecurityResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetLogSecuritySummary.
        /// </summary>
        /// /// <param name="logSecurityRequest">logsPerformanceRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("GetLogSecuritySummary")]
        public async Task<ApiResponse<LogsSecuritySummaryResultDto>> GetLogSecuritySummary([FromBody] LogSecurityRequestDto logSecurityRequest)
        {
            try
            {
                logSecurityRequest.RequestUserId = this.CurrentUserId;
                var result = await this.logsService.GetLogSecuritySummary(logSecurityRequest);
                return new ApiResponse<LogsSecuritySummaryResultDto>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<LogsSecuritySummaryResultDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetGroupLogsByCondition.
        /// </summary>
        /// <param name="dataRequest">dataRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("GetGroupLogsByCondition")]
        public async Task<ApiResponse<GroupLogsResultDto>> GetGroupLogsByCondition([FromBody] LogSecurityRequestDto dataRequest)
        {
            try
            {
                dataRequest.RequestUserId = this.CurrentUserId;
                var result = await this.dashboardService.GetGroupLogsByCondition(dataRequest);
                return new ApiResponse<GroupLogsResultDto>(EnApiStatusCode.Success, result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<GroupLogsResultDto>(ex.HResult);
            }
        }
    }
}
