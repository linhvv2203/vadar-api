// <copyright file="LogsService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Logs Service.
    /// </summary>
    public class LogsService : EntityService<Group>, ILogsService
    {
        private readonly IElasticSearchCallApiHelper elasticSearchCallApiHelper;
        private readonly ILogUnitOfWork logUnitOfWork;
        private readonly IWorkspaceUnitOfWork workspaceUnitOfWork;
        private readonly IStringHelper stringHelper;

        /// <summary>
        /// Initialises a new instance of the <see cref="LogsService"/> class.
        /// </summary>
        /// <param name="elasticSearchCallApiHelper">elasticSearchCallApiHelper.</param>
        /// <param name="logUnitOfWork">logUnitOfWork.</param>
        /// <param name="workspaceUnitOfWork">workspaceUnitOfWork.</param>
        /// <param name="stringHelper">stringHelper.</param>
        public LogsService(
            IElasticSearchCallApiHelper elasticSearchCallApiHelper,
            ILogUnitOfWork logUnitOfWork,
            IWorkspaceUnitOfWork workspaceUnitOfWork,
            IStringHelper stringHelper)
            : base(logUnitOfWork, logUnitOfWork.GroupRepository)
        {
            this.elasticSearchCallApiHelper = elasticSearchCallApiHelper;
            this.logUnitOfWork = logUnitOfWork;
            this.workspaceUnitOfWork = workspaceUnitOfWork;
            this.stringHelper = stringHelper;
        }

        /// <inheritdoc/>
        public async Task<LogsNetworkResultPagingDto> GetLogsNetworkPaging(LogsNetworkRequestDto logsNetworkRequestDto)
        {
            if (logsNetworkRequestDto is null)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            await this.CheckPermissions(logsNetworkRequestDto.RequestUserId, logsNetworkRequestDto.WorkspaceId);
            logsNetworkRequestDto.Hosts = await this.GetHostsOfWorkSpace(logsNetworkRequestDto.RequestUserId, logsNetworkRequestDto.WorkspaceId, logsNetworkRequestDto.Devices);

            var responseString = await this.elasticSearchCallApiHelper.GetNetworkLog(logsNetworkRequestDto);
            var result = this.GetLogsNetwork(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<LogsPerformanceResultPagingDto> GetLogsPerformancePaging(LogsPerformanceRequestDto logsPerfromanceRequest)
        {
            if (logsPerfromanceRequest is null)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            await this.CheckPermissions(logsPerfromanceRequest.RequestUserId, logsPerfromanceRequest.WorkspaceId);
            logsPerfromanceRequest.Hosts = await this.GetHostsOfWorkSpace(logsPerfromanceRequest.RequestUserId, logsPerfromanceRequest.WorkspaceId, logsPerfromanceRequest.Devices);
            if (!string.IsNullOrEmpty(logsPerfromanceRequest.GroupName))
            {
                var hosts = (await this.logUnitOfWork.GroupHostRepository.GetAll())
                .Where(x => x.Group.Name.Trim().ToLower() == logsPerfromanceRequest.GroupName.Trim().ToLower())
                .Select(s => s.Host.Name).ToList();
                logsPerfromanceRequest.Hosts = logsPerfromanceRequest.Hosts.Count == 0 ? hosts : logsPerfromanceRequest.Hosts.Intersect(hosts).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetPerformanceLog(logsPerfromanceRequest);
            if (logsPerfromanceRequest.WorkspaceId == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull);
            }

            var result = this.GetLogsPerformanceAsync(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<LogsSecurityResultPagingDto> GetLogSecurityPaging(LogSecurityRequestDto logSecurityRequest)
        {
            await this.CheckPermissions(logSecurityRequest.RequestUserId, logSecurityRequest.WorkspaceId);
            var allHostOfWorkspace = new List<Host>();
            if (logSecurityRequest.WorkspaceId != 0)
            {
                allHostOfWorkspace = await this.GetAllHostsOfWorkSpace(logSecurityRequest.RequestUserId, logSecurityRequest.WorkspaceId);
                logSecurityRequest.Hosts = await this.GetHostsOfWorkSpace(logSecurityRequest.RequestUserId, logSecurityRequest.WorkspaceId, logSecurityRequest.Devices);
            }
            else
            {
                allHostOfWorkspace = (await this.logUnitOfWork.HostRepository.GetAll()).ToList();

                if (logSecurityRequest.Devices == null)
                {
                    logSecurityRequest.Hosts = (await this.logUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
                }
                else
                {
                    logSecurityRequest.Hosts = (await this.logUnitOfWork.HostRepository.GetAll())
                        .Where(s => s.Type == (EnTypeOfDevices)logSecurityRequest.Devices)
                        .Select(x => x.NameEngine).ToList();
                }
            }

            var responseString = await this.elasticSearchCallApiHelper.GetLogSecurity(logSecurityRequest, logSecurityRequest.Sort);
            var result = this.GetLogSecurity(responseString, allHostOfWorkspace);

            return result;
        }

        /// <inheritdoc/>
        public async Task<LogsSecuritySummaryResultDto> GetLogSecuritySummary(LogSecurityRequestDto logSecurityRequest)
        {
            await this.CheckPermissions(logSecurityRequest.RequestUserId, logSecurityRequest.WorkspaceId);
            if (logSecurityRequest.WorkspaceId != 0)
            {
                logSecurityRequest.Hosts = await this.GetHostsOfWorkSpace(logSecurityRequest.RequestUserId, logSecurityRequest.WorkspaceId, logSecurityRequest.Devices);
            }
            else
            {
                if (logSecurityRequest.Devices == null)
                {
                    logSecurityRequest.Hosts = (await this.logUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
                }
                else
                {
                    logSecurityRequest.Hosts = (await this.logUnitOfWork.HostRepository.GetAll())
                        .Where(s => s.Type == (EnTypeOfDevices)logSecurityRequest.Devices)
                        .Select(x => x.NameEngine).ToList();
                }
            }

            var logsOverview = await this.elasticSearchCallApiHelper.GetLogSecurity(logSecurityRequest);
            var logsMoreThanLeve9 = await this.elasticSearchCallApiHelper.GetLogSecurityMoreThanLevel9(logSecurityRequest);

            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 0, 0, 0);
            logSecurityRequest.FromDate = date;
            logSecurityRequest.ToDate = DateTime.Now;

            var logsToday = await this.elasticSearchCallApiHelper.GetLogSecurity(logSecurityRequest);

            var result = this.GetLogSecuritySummary(logsOverview, logsToday, logsMoreThanLeve9);

            return result;
        }

        private LogsPerformanceResultPagingDto GetLogsPerformanceAsync(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new LogsPerformanceResultPagingDto();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logPerformanceList = new List<LogsPerformanceResultDto>();

            if (response?.hits?.hits != null)
            {
                foreach (var item in response.hits.hits)
                {
                    if (string.IsNullOrEmpty(item?._source?.event_name1?.ToString()) || string.IsNullOrEmpty(item?._source?.trigger_name?.ToString()))
                    {
                        continue;
                    }

                    logPerformanceList.Add(new LogsPerformanceResultDto
                    {
                        HostName = item?._source?.event_name1,
                        EventName = item?._source?.trigger_name,
                        Time = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                        Severity = item?._source?.trigger_severity,
                        Description = item?._source?.trigger_description,
                        Status = item?._source?.trigger_status,
                    });
                }
            }

            var result = new LogsPerformanceResultPagingDto
            {
                Count = response?.hits.total.value,
                Items = logPerformanceList,
            };

            return result;
        }

        private LogsNetworkResultPagingDto GetLogsNetwork(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new LogsNetworkResultPagingDto();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logList = new List<LogsNetworkResultDto>();
            if (response?.hits?.hits != null)
            {
                foreach (var item in response.hits.hits)
                {
                    logList.Add(new LogsNetworkResultDto
                    {
                        Time = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                        HostName = this.stringHelper.GetHostName(item?._source?.agent?.name),
                        WorkspaceName = string.Join(", ", item?._source?.agent?.group),
                        Action = item?._source?.data?.action,
                        Class = item?._source?.data?.Value<string>("class") ?? string.Empty,
                        SourceAddress = item?._source?.data?.src_addr,
                        SourcePort = item?._source?.data?.src_port,
                        DestinationAddress = item?._source?.data?.dst_addr,
                        DestinationPort = item?._source?.data?.dst_port,
                        Message = item?._source?.data?.msg,
                    });
                }
            }

            var result = new LogsNetworkResultPagingDto
            {
                Count = response?.hits?.total?.value,
                Items = logList,
            };

            return result;
        }

        private LogsSecurityResultPagingDto GetLogSecurity(string responseString, List<Host> allHostOfWorkspace)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new LogsSecurityResultPagingDto();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logSecurityList = new List<LogSecurityDataDto>();
            if (response?.hits?.hits == null)
            {
                return new LogsSecurityResultPagingDto
                {
                    Count = response?.hits?.total?.value,
                    Items = logSecurityList,
                };
            }

            foreach (var item in response.hits.hits)
            {
                if (string.IsNullOrEmpty(item?._source?.agent?.name?.ToString()) || string.IsNullOrEmpty(item?._source?.rule?.description?.ToString()))
                {
                    continue;
                }

                string nameDisplay = allHostOfWorkspace.FirstOrDefault(x => x.NameEngine == this.stringHelper.GetHostName(item?._source?.agent?.name))?.Name;
                if (item?._source?.data != null)
                {
                    var dataDto = new LogSecurityDataDto
                    {
                        Host = this.stringHelper.GetHostName(item?._source?.agent?.name),
                        NameDisplay = nameDisplay,
                        Timestamp = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                        FullLog = item?._source?.full_log,
                        Description = item?._source?.rule?.description,
                        Level = item?._source?.rule?.level,
                        Groups = string.Join(", ", item?._source?.rule?.groups),
                        References = item?._source?.data?.vulnerability?.references.ToObject<string[]>(),
                        Rationale = item?._source?.data?.vulnerability?.rationale,
                        Mitre = item?._source?.rule?.mitre?.technique.ToObject<string[]>(),
                        Title = item?._source?.data?.vulnerability?.title,
                        RawLogs = string.Join(" ", item),
                    };

                    logSecurityList.Add(dataDto);
                }
                else
                {
                    var dataDto = new LogSecurityDataDto
                    {
                        Host = this.stringHelper.GetHostName(item?._source?.agent?.name),
                        NameDisplay = nameDisplay,
                        Timestamp = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                        FullLog = item?._source?.full_log,
                        Description = item?._source?.rule?.description,
                        Level = item?._source?.rule?.level,
                        Groups = string.Join(", ", item?._source?.rule?.groups),
                        RawLogs = string.Join(" ", item),
                    };

                    logSecurityList.Add(dataDto);
                }
            }

            return new LogsSecurityResultPagingDto
            {
                Count = response.hits?.total?.value,
                Items = logSecurityList,
            };
        }

        private LogsSecuritySummaryResultDto GetLogSecuritySummary(string logsOverview, string logsToday, string logsMoreThanLeve9)
        {
            if (string.IsNullOrEmpty(logsOverview))
            {
                return new LogsSecuritySummaryResultDto();
            }

            var responseLogsOverview = JsonConvert.DeserializeObject<dynamic>(logsOverview);
            var responseLogsToday = JsonConvert.DeserializeObject<dynamic>(logsToday);
            var logsAboveLevel = JsonConvert.DeserializeObject<dynamic>(logsMoreThanLeve9);

            return new LogsSecuritySummaryResultDto
            {
                LogsOverview = responseLogsOverview?.hits?.total?.value,
                LogsToday = responseLogsToday?.hits?.total?.value,
                LogsAboveLevel9 = logsAboveLevel?.hits?.total?.value,
            };
        }

        private async Task<List<string>> GetHostsOfWorkSpace(string requestUserId, int? workspaceId, int? type)
        {
            var result = new List<string>();

            // Check role of users.
            var permissions = await this.GetPermissions(
                requestUserId,
                workspaceId,
                this.logUnitOfWork.RolePermissionRepository,
                this.logUnitOfWork.WorkspaceRolePermissionRepository);
            if (permissions.Any(p => p == (int)EnPermissions.AllDashboardsView || p == (int)EnPermissions.FullPermission) && (workspaceId == null || workspaceId <= 0))
            {
                return new List<string>();
            }

            // Get groups of work space.
            var workspaceIds = new List<int>();
            if (workspaceId == null || workspaceId <= 0)
            {
                workspaceIds = (from wru in await this.logUnitOfWork.WorkspaceRoleUserRepository.GetAll()
                                join wr in await this.logUnitOfWork.WorkspaceRoleRepository.GetAll() on wru
                                    .WorkspaceRoleId equals wr.Id
                                where wru.UserId == requestUserId
                                select wr.WorkspaceId).ToList();
            }
            else
            {
                workspaceIds.Add(workspaceId.Value);
            }

            var hostList = (from h in await this.logUnitOfWork.HostRepository.GetAll()
                            where type != null ? h.Type == (EnTypeOfDevices)type : true
                            join wh in await this.logUnitOfWork.WorkspaceHostRepository.GetAll() on h.Id equals wh.HostId
                            where workspaceIds.Contains(wh.WorkspaceId)
                            select h.NameEngine).ToList();

            result.AddRange(hostList);

            if (result.Count > 0)
            {
                return result;
            }

            result.Add(Guid.NewGuid().ToString());
            return result;
        }

        private async Task<List<Host>> GetAllHostsOfWorkSpace(string requestUserId, int? workspaceId)
        {
            var result = new List<Host>();

            // Check role of users.
            var permissions = await this.GetPermissions(
                requestUserId,
                workspaceId,
                this.logUnitOfWork.RolePermissionRepository,
                this.logUnitOfWork.WorkspaceRolePermissionRepository);
            if (permissions.Any(p => p == (int)EnPermissions.AllDashboardsView || p == (int)EnPermissions.FullPermission) && (workspaceId == null || workspaceId < 0))
            {
                return new List<Host>();
            }

            // Get groups of work space.
            var workspaceIds = new List<int>();
            if (workspaceId == null || workspaceId <= 0)
            {
                workspaceIds = (from wru in await this.logUnitOfWork.WorkspaceRoleUserRepository.GetAll()
                                join wr in await this.logUnitOfWork.WorkspaceRoleRepository.GetAll() on wru
                                    .WorkspaceRoleId equals wr.Id
                                where wru.UserId == requestUserId
                                select wr.WorkspaceId).ToList();
            }
            else
            {
                workspaceIds.Add(workspaceId.Value);
            }

            var hostList = (from h in await this.logUnitOfWork.HostRepository.GetAll()
                            join wh in await this.logUnitOfWork.WorkspaceHostRepository.GetAll() on h.Id equals wh.HostId
                            where workspaceIds.Contains(wh.WorkspaceId)
                            select h).ToList();

            result.AddRange(hostList);

            if (result.Count > 0)
            {
                return result;
            }

            return result;
        }

        private async Task CheckPermissions(string currentUserId, int? workspaceId)
        {
            if (string.IsNullOrEmpty(currentUserId) || workspaceId == null || workspaceId < 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.LogsView, (long)EnPermissions.AllLogsView, (long)EnPermissions.FullPermission }, this.logUnitOfWork.RolePermissionRepository, this.logUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }
        }
    }
}
