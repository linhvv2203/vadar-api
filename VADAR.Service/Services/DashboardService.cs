// <copyright file="DashboardService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Const;
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
    public class DashboardService : EntityService<Permission>, IDashboardService
    {
        private readonly IElasticSearchCallApiHelper elasticSearchCallApiHelper;
        private readonly ICallApiZabbixHelper callApiZabbixHelper;
        private readonly ICallApiWazuhHelper callApiWazuhHelper;
        private readonly IDashboardUnitOfWork dashboardUnitOfWork;
        private readonly IWorkspaceHostUnitOfWork workspaceHostUnitOfWork;
        private readonly IStringHelper stringHelper;

        /// <summary>
        /// Initialises a new instance of the <see cref="DashboardService"/> class.
        /// </summary>
        /// <param name="elasticSearchCallApiHelper">elasticSearchCallApiHelper.</param>
        /// <param name="callApiZabbixHelper">callApiZabbixHelper.</param>
        /// <param name="callApiWazuhHelper">callApiWazuhHelper.</param>
        /// <param name="dashboardUnitOfWork">Dashboard Unit Of Work.</param>
        /// <param name="workspaceHostUnitOfWork">workspaceHostUnitOfWork.</param>
        /// <param name="stringHelper">stringHelper.</param>
        public DashboardService(
            IElasticSearchCallApiHelper elasticSearchCallApiHelper,
            ICallApiZabbixHelper callApiZabbixHelper,
            ICallApiWazuhHelper callApiWazuhHelper,
            IDashboardUnitOfWork dashboardUnitOfWork,
            IWorkspaceHostUnitOfWork workspaceHostUnitOfWork,
            IStringHelper stringHelper)
            : base(
            dashboardUnitOfWork,
            dashboardUnitOfWork.PermissionRepository)
        {
            this.elasticSearchCallApiHelper = elasticSearchCallApiHelper;
            this.callApiZabbixHelper = callApiZabbixHelper;
            this.dashboardUnitOfWork = dashboardUnitOfWork;
            this.workspaceHostUnitOfWork = workspaceHostUnitOfWork;
            this.callApiWazuhHelper = callApiWazuhHelper;
            this.stringHelper = stringHelper;
        }

        /// <inheritdoc/>
        public async Task<SummaryDto> GetDashboardSummarys(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            var result = new SummaryDto();
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
                var workspace = (await this.dashboardUnitOfWork.WorkspaceRepository.FindBy(x => x.Id == dataRequest.WorkSpaceId)).FirstOrDefault();

                if (workspace == null)
                {
                    throw new VadarException(ErrorCode.WorkspaceNull);
                }

                result = await this.GetDashboardSummaryEngine(dataRequest.Hosts, workspace.WazuhRef, workspace.ZabbixRef, workspace.Id, dataRequest.RequestUserId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
                result = await this.GetDashboardSummaryEngine(dataRequest.Hosts, null, null, 0, dataRequest.RequestUserId);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<HostStatisticDto>> GetHostStatistics(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);

            var roleIds = (await this.dashboardUnitOfWork.RoleUserRepository.GetAll()).Where(ru => ru.UserId == dataRequest.RequestUserId).Select(ru => ru.RoleId);
            if (!roleIds.Contains(Constants.UserRoles.Admin))
            {
                dataRequest.Level = 7;
            }

            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetHostStatistic(dataRequest);
            var result = this.GetHostStatistic(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<VulnerabilitiesSummaryDto>> GetVulnerabilitiesSummary(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);

            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetAlertsSeverity(dataRequest);

            var result = this.VulnerabilitiesSummary(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartReturnDto>> GetAlertsSeverity(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetAlertsSeverity(dataRequest);
            var result = this.GetAlertsEvolutionOverTime(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartReturnDto>> GetAlertsByActionOverTime(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetAlertsByActionOverTime(dataRequest);
            var result = this.GetAlertsEvolutionOverTime(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartDto>> GetEventSummaryIntegrityMonitoring(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetEventSummaryIntegrityMonitoring(dataRequest);

            var result = this.EventSummaryIntegrityMonitoring(responseString);
            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventDto>> GetTop5SecurityEvent(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetTop5SecurityEvent(dataRequest);
            var result = this.GetTop10AttackIp(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventDto>> GetTop5AgentIntegrityMonitoring(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetTop5AgentIntegrityMonitoring(dataRequest);
            var result = this.GetTop10AttackIp(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartReturnDto>> GetSecurityEventByTime(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (!string.IsNullOrEmpty(dataRequest.HostName))
            {
                var hosts = new List<string>();
                hosts.Add(dataRequest.HostName);
                dataRequest.Hosts = hosts;
            }
            else
            {
                if (dataRequest.WorkSpaceId != 0)
                {
                    dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
                }
                else
                {
                    dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                        .Select(x => x.NameEngine).ToList();
                }
            }

            var listReturn = new List<EventChartReturnDto>();

            if (dataRequest.SearchLevel)
            {
                for (var i = 1; i <= 15; i++)
                {
                    // LEVEL <i>
                    dataRequest.Level = i;
                    var responseStringLevel = await this.elasticSearchCallApiHelper.GetSecurityEventByTime(dataRequest);
                    Debug.Assert(dataRequest.WorkSpaceId != null, "dataRequest.WorkSpaceId != null");
                    var resultLevel = this.GetSecurityEventByTimeAsync(responseStringLevel);
                    if (resultLevel.Any())
                    {
                        listReturn.Add(new EventChartReturnDto()
                        {
                            LevelName = @"Level " + i,
                            Value = resultLevel,
                        });
                    }
                }
            }
            else
            {
                // LEVEL All
                var responseStringLevelAll = await this.elasticSearchCallApiHelper.GetSecurityEventByTime(dataRequest);
                Debug.Assert(dataRequest.WorkSpaceId != null, "dataRequest.WorkSpaceId != null");
                var resultLevelAll = this.GetSecurityEventByTimeAsync(responseStringLevelAll);
                if (resultLevelAll.Any())
                {
                    listReturn.Add(new EventChartReturnDto
                    {
                        LevelName = "All",
                        Value = resultLevelAll,
                    });
                }
            }

            return listReturn;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartReturnDto>> GetPerformanceEvent(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var listReturn = new List<EventChartReturnDto>();

            if (dataRequest.SearchLevel)
            {
                // Severity Warning
                dataRequest.Serverity = EnZabbixSeverity.Warning.ToString();
                var responseStringWarning = await this.elasticSearchCallApiHelper.GetPerformanceEvent(dataRequest);
                Debug.Assert(dataRequest.WorkSpaceId != null, "dataRequest.WorkSpaceId != null");
                var resultLevelWarning = this.GetSecurityEventByTimeAsync(responseStringWarning);
                if (resultLevelWarning.Any())
                {
                    listReturn.Add(new EventChartReturnDto()
                    {
                        LevelName = "Warning",
                        Value = resultLevelWarning,
                    });
                }

                // Severity Warning
                dataRequest.Serverity = EnZabbixSeverity.Average.ToString();
                var responseStringAverage = await this.elasticSearchCallApiHelper.GetPerformanceEvent(dataRequest);
                Debug.Assert(dataRequest.WorkSpaceId != null, "dataRequest.WorkSpaceId != null");
                var resultLevelAverage = this.GetSecurityEventByTimeAsync(responseStringAverage);
                if (resultLevelAverage.Any())
                {
                    listReturn.Add(new EventChartReturnDto()
                    {
                        LevelName = "Average",
                        Value = resultLevelAverage,
                    });
                }
            }
            else
            {
                // Severity Warning
                var responseStringAll = await this.elasticSearchCallApiHelper.GetPerformanceEvent(dataRequest);
                Debug.Assert(dataRequest.WorkSpaceId != null, "dataRequest.WorkSpaceId != null");
                var resultLevelAll = this.GetSecurityEventByTimeAsync(responseStringAll);
                if (resultLevelAll.Any())
                {
                    listReturn.Add(new EventChartReturnDto()
                    {
                        LevelName = "All",
                        Value = resultLevelAll,
                    });
                }
            }

            return listReturn;
        }

        /// <inheritdoc/>
        public async Task<List<EventDto>> GetTop10SecurityEvent(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetTop10SecurityEvent(dataRequest);
            var result = this.GetTop10SecurityEvent(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventDto>> GetTop10AttackIP(HostStatisticRequestDto dataRequest, int ruleId)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetTop10AttackIP(dataRequest, ruleId);
            var result = this.GetTop10AttackIp(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<SecurityEventReturnDto>> GetLast10SecurityEvent(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetLast10SecurityEvent(dataRequest);
            var result = this.GetLast10SecurityEvent(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<TopEventByLevelDto>> GetTopEventsByLevel(LogSecurityRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            if (dataRequest.WorkspaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetTopEventsByLevel(dataRequest);
            var result = this.GetTopEventsByLevel(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<GroupLogsResultDto> GetGroupLogsByCondition(LogSecurityRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            if (dataRequest.WorkspaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetGroupLogsByCondition(dataRequest);
            var result = this.GroupLogsByLevel(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<RuleGroupDto>> GroupRuleByAgentName(LogSecurityRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            if (dataRequest.WorkspaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetRuleGroupByAgentName(dataRequest);
            var result = this.GetRuleGroupByAgentName(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartReturnDto>> GetAlertsEvolutionOverTime(LogSecurityRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            if (dataRequest.WorkspaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetAlertsEvolutionOverTime(dataRequest);
            var result = this.GetAlertsEvolutionOverTime(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventChartReturnDto>> GetAgentsStatus(LogSecurityRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            if (dataRequest.WorkspaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkspaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetAgentsStatus(dataRequest);
            var result = this.GetAlertsEvolutionOverTime(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<PerformanceEventReturnDto>> GetLast10PerformanceEvent(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetLast10PerformanceEvent(dataRequest);
            if (dataRequest.WorkSpaceId == null)
            {
                return new List<PerformanceEventReturnDto>();
            }

            var result = this.GetLast10PerformanceEventAsync(responseString, (int)dataRequest.WorkSpaceId);

            return await result;
        }

        /// <inheritdoc/>
        public async Task<List<CVEsEventDto>> GetMostCommonCVEs(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetMostCommonCVEs(dataRequest);
            var result = this.GetMostCommonCvEs(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<CVEsEventDto>> GetMostCommonCWEs(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetMostCommonCWEs(dataRequest);
            var result = this.GetMostCommonCvEs(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventDto>> GetMostAffectedAgents(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetMostAffectedAgents(dataRequest);
            var result = this.GetTop10AttackIp(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<ChartLineReturnDto>> GetTopRequirementsOverTime(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetPCIDSSRequirements(dataRequest);
            var result = this.GetPcidssRequirements(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<EventDto>> GetTop10AgentsByAlertsNumber(HostStatisticRequestDto dataRequest)
        {
            await this.CheckPermissions(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            if (dataRequest.WorkSpaceId != 0)
            {
                dataRequest.Hosts = await this.GetHostsOfWorkSpace(dataRequest.RequestUserId, dataRequest.WorkSpaceId);
            }
            else
            {
                dataRequest.Hosts = (await this.dashboardUnitOfWork.HostRepository.GetAll())
                    .Select(x => x.NameEngine).ToList();
            }

            var responseString = await this.elasticSearchCallApiHelper.GetTop10AgentsByAlertsNumber(dataRequest);
            var result = this.GetTop10AgentsByAlertsNumber(responseString);

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<ChartLineReturnDto>> GetPCIDSSRequirements(HostStatisticRequestDto dataRequest)
        {
            return await this.GetTopRequirementsOverTime(dataRequest);
        }

        /// <inheritdoc/>
        public async Task<List<HostDto>> ShowHostProblem(int? workspaceId)
        {
            var workspace = (await this.dashboardUnitOfWork.WorkspaceRepository.FindBy(x => x.Id == workspaceId)).FirstOrDefault();
            var listHostProblemCallZabbix = await this.callApiZabbixHelper.GetHostProblem(workspace.ZabbixRef);
            var result = JsonConvert.DeserializeObject<dynamic>(listHostProblemCallZabbix);
            var listHostProblem = new List<HostDto>();
            if (result != null)
            {
                foreach (var item in result?.result)
                {
                    var hostProblem = new HostDto
                    {
                        Name = item?.host,
                    };
                    listHostProblem.Add(hostProblem);
                }

                return listHostProblem;
            }

            return listHostProblem;
        }

        private List<ChartLineReturnDto> GetPcidssRequirements(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return new List<ChartLineReturnDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonString);
            var temps = new List<DataTemp>();
            foreach (var item in data.aggregations["2"].buckets)
            {
                foreach (var el in item["3"].buckets)
                {
                    temps.Add(new DataTemp
                    {
                        Key = el.key,
                        Count = el.doc_count,
                        Datetime = item.key_as_string,
                    });
                }
            }

            var res = temps.GroupBy(g => g.Key)
                .Select(s => new ChartLineReturnDto
                {
                    Name = s.Key,
                    Value = s.Select(x => new EventChartDto { Date = x.Datetime, Value = x.Count }).ToList(),
                });

            return res.ToList();
        }

        private async Task<SummaryDto> GetDashboardSummaryEngine(List<string> hosts, string wazuhRef, string zabbixRef, int workspaceId, string userId)
        {
            var summaryQDto = new SummaryDto { TotalHosts = hosts.Count };

            var listHostExist = new List<EngineRequestDto>();
            var listHostEngine = new List<EngineRequestDto>();
            var hostProblemRequest = new LogsPerformanceRequestDto
            {
                FromDate = DateTime.UtcNow.AddDays(-1),
                ToDate = DateTime.UtcNow,
                Status = "PROBLEM",
                WorkspaceId = workspaceId,
                Hosts = hosts,
                RequestUserId = userId,
            };
            var listHostProblem = await this.elasticSearchCallApiHelper.GetPerformanceLog(hostProblemRequest);
            var amountHostProblem = JsonConvert.DeserializeObject<dynamic>(listHostProblem);
            var listHostDb = new List<string>();
            if (workspaceId > 0)
            {
                listHostDb = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.GetAll()).Where(x => x.WorkspaceId == workspaceId).Select(x => x.Host.NameEngine).ToList();
            }
            else
            {
                listHostDb = (await this.dashboardUnitOfWork.HostRepository.GetAll()).Select(x => x.NameEngine).ToList();
            }

            var responseWazuh = string.Empty;
            if (wazuhRef != null)
            {
                responseWazuh = await this.callApiWazuhHelper.GetAllHostByGroup(wazuhRef);
            }
            else
            {
                responseWazuh = await this.callApiWazuhHelper.GetAllHost();
            }

            var dataWazuh = JsonConvert.DeserializeObject<dynamic>(responseWazuh);
            if (dataWazuh != null)
            {
                foreach (var item in dataWazuh.data.items)
                {
                    if (item.status == "Active")
                    {
                        var requestWazuhActive = new EngineRequestDto
                        {
                            Name = item.name,
                            Type = "Wazuh",
                            Status = (int)EnHostStatus.Online,
                        };
                        listHostEngine.Add(requestWazuhActive);
                    }

                    if (item.status != "Disconnected")
                    {
                        continue;
                    }

                    var requestWazuhDisconnect = new EngineRequestDto
                    {
                        Name = item.name,
                        Type = "Wazuh",
                        Status = (int)EnHostStatus.Offline,
                    };
                    listHostEngine.Add(requestWazuhDisconnect);
                }
            }

            var responseZabbix = string.Empty;

            if (zabbixRef != null)
            {
                responseZabbix = await this.callApiZabbixHelper.GetHostByGroup(zabbixRef);
            }
            else
            {
                responseZabbix = await this.callApiZabbixHelper.GetAllHost();
            }

            var dataZabbix = JsonConvert.DeserializeObject<dynamic>(responseZabbix);
            if (dataZabbix != null)
            {
                foreach (var item in dataZabbix.result)
                {
                    if (item.status == "0")
                    {
                        var requestZabbixActive = new EngineRequestDto
                        {
                            Name = item.name,
                            Type = "Zabbix",
                            Status = (int)EnHostStatus.Online,
                        };
                        listHostEngine.Add(requestZabbixActive);
                    }

                    if (item.status != "1")
                    {
                        continue;
                    }

                    var requestZabbixDisconnect = new EngineRequestDto
                    {
                        Name = item.name,
                        Type = "Zabbix",
                        Status = (int)EnHostStatus.Offline,
                    };
                    listHostEngine.Add(requestZabbixDisconnect);
                }
            }

            foreach (var hostDb in listHostDb)
            {
                var hostEgineDto = new EngineRequestDto
                {
                    Name = hostDb,
                };
                foreach (var hostEngine in listHostEngine.Where(hostEngine => hostDb == hostEngine.Name))
                {
                    hostEgineDto.Status = hostEngine.Status;
                }

                listHostExist.Add(hostEgineDto);
            }

            summaryQDto.UnHealthy = amountHostProblem?.hits.total.value;

            foreach (var item in listHostExist)
            {
                if (item.Status == 1)
                {
                    summaryQDto.Active++;
                }
                else
                {
                    summaryQDto.Disconnect++;
                }
            }

            return summaryQDto;
        }

        private List<HostStatisticDto> GetHostStatistic(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<HostStatisticDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var listHostStatistic = new List<HostStatisticDto>();
            if (data == null)
            {
                return listHostStatistic;
            }

            foreach (var item in data.hits?.hits)
            {
                var hostStatistic = new HostStatisticDto
                {
                    Host = this.stringHelper.GetHostName(item?._source?.agent?.name),
                    Issues = item?._source?.rule?.description,
                };
                listHostStatistic.Add(hostStatistic);
            }

            return listHostStatistic;
        }

        private List<CVEsEventDto> GetMostCommonCvEs(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<CVEsEventDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var cVEs = new List<CVEsEventDto>();

            foreach (var item in data.aggregations["2"].buckets)
            {
                cVEs.Add(new CVEsEventDto
                {
                    Vulnerabilities = item.key,
                    Value = Convert.ToInt32(item.doc_count),
                });
            }

            return cVEs;
        }

        private List<EventChartDto> GetSecurityEventByTimeAsync(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<EventChartDto>();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var listdata = new List<EventChartDto>();
            if (response?.aggregations?.datas?.buckets == null)
            {
                return listdata;
            }

            foreach (var item in response.aggregations.datas.buckets)
            {
                var dataDto = new EventChartDto
                {
                    Date = item.key_as_string,
                    Value = item.doc_count,
                };
                listdata.Add(dataDto);
            }

            return listdata;
        }

        private List<EventDto> GetTop10SecurityEvent(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<EventDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var eventDtos = new List<EventDto>();

            foreach (var item in data.aggregations["2"].buckets)
            {
                eventDtos.Add(new EventDto
                {
                    HostName = item.key,
                    Value = Convert.ToInt32(item.doc_count),
                });
            }

            return eventDtos;
        }

        private List<EventDto> GetTop10AttackIp(string responseString)
        {
            return this.ConvertStringToEventDto(responseString);
        }

        private List<EventDto> GetTop10AgentsByAlertsNumber(string responseString)
        {
            return this.ConvertStringToEventDto(responseString);
        }

        private List<EventDto> ConvertStringToEventDto(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return new List<EventDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonString);
            var eventDtos = new List<EventDto>();

            foreach (var item in data.aggregations["2"].buckets)
            {
                eventDtos.Add(new EventDto
                {
                    HostName = item.key,
                    Value = Convert.ToInt32(item.doc_count),
                });
            }

            return eventDtos;
        }

        private List<VulnerabilitiesSummaryDto> VulnerabilitiesSummary(string responseString)
        {
            var data = new List<VulnerabilitiesSummaryDto>();
            if (string.IsNullOrEmpty(responseString))
            {
                return data;
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            if (response?.aggregations?.datas?.buckets == null)
            {
                return data;
            }

            foreach (var item in response.aggregations.datas.buckets)
            {
                data.Add(new VulnerabilitiesSummaryDto()
                {
                    Key = item.key,
                    Value = item.doc_count,
                });
            }

            return data;
        }

        private List<EventChartDto> EventSummaryIntegrityMonitoring(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<EventChartDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var eventSummary = new List<EventChartDto>();

            foreach (var item in data.aggregations["2"].buckets)
            {
                var time = item.Value<string>("key_as_string") ?? string.Empty;
                DateTime.TryParse(time, out DateTime datetime);
                eventSummary.Add(new EventChartDto()
                {
                    Date = datetime.ToString("MM'/'dd'/'yyyy HH:mm:ss"),
                    Value = Convert.ToInt32(item.doc_count),
                });
            }

            return eventSummary;
        }

        private List<SecurityEventReturnDto> GetLast10SecurityEvent(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<SecurityEventReturnDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var securityEvents = new List<SecurityEventReturnDto>();

            if (data == null)
            {
                return securityEvents;
            }

            foreach (var item in data.hits?.hits)
            {
                securityEvents.Add(new SecurityEventReturnDto
                {
                    Host = this.stringHelper.GetHostName(item?._source?.agent?.name),
                    Timestamp = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                    Description = item?._source?.rule?.description,
                });
            }

            return securityEvents;
        }

        private List<TopEventByLevelDto> GetTopEventsByLevel(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<TopEventByLevelDto>();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logSecurityList = new List<TopEventByLevelDto>();
            if (response?.aggregations?.datas?.buckets == null)
            {
                return logSecurityList;
            }

            foreach (var item in response.aggregations.datas.buckets)
            {
                var dataDto = new TopEventByLevelDto
                {
                    EventName = item?.key,
                    Level = item?.level?.value,
                    Count = item?.doc_count,
                };

                List<string> hostLogs = new List<string>();
                foreach (var host in item?.hosts?.buckets)
                {
                    hostLogs.Add(host?.key.ToString());
                }

                dataDto.Hosts = string.Join(", ", hostLogs);
                logSecurityList.Add(dataDto);
            }

            return logSecurityList;
        }

        private GroupLogsResultDto GroupLogsByLevel(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new GroupLogsResultDto();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logs = new List<TopEventByLevelDto>();

            if (response?.aggregations?.datas?.buckets == null)
            {
                return new GroupLogsResultDto
                {
                    Count = response?.hits?.total?.value,
                    Items = logs,
                };
            }

            foreach (var item in response.aggregations.datas.buckets)
            {
                var dataDto = new TopEventByLevelDto
                {
                    EventName = item?.key,
                    Level = item?.level?.value,
                    Count = item?.doc_count,
                };

                List<string> hostLogs = new List<string>();
                foreach (var host in item?.hosts?.buckets)
                {
                    hostLogs.Add(host?.key.ToString());
                }

                dataDto.Hosts = string.Join(", ", hostLogs);

                logs.Add(dataDto);
            }

            return new GroupLogsResultDto
            {
                Count = response.hits?.total?.value,
                Items = logs,
            };
        }

        private List<RuleGroupDto> GetRuleGroupByAgentName(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<RuleGroupDto>();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var ruleGroups = new List<RuleGroupDto>();

            foreach (var item in response.aggregations["2"].buckets)
            {
                var dataDto = new RuleGroupDto
                {
                    RuleGroup = item?.key,
                    Count = item?.doc_count,
                };

                ruleGroups.Add(dataDto);
            }

            return ruleGroups;
        }

        private List<SecurityEventReturnDto> GetHostsInEvents(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<SecurityEventReturnDto>();
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var securityEvents = new List<SecurityEventReturnDto>();

            if (data == null)
            {
                return securityEvents;
            }

            foreach (var item in data.hits?.hits)
            {
                securityEvents.Add(new SecurityEventReturnDto
                {
                    Host = this.stringHelper.GetHostName(item?._source?.agent?.name),
                    Timestamp = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                    Description = item?._source?.rule?.description,
                });
            }

            return securityEvents;
        }

        private List<EventChartReturnDto> GetAlertsEvolutionOverTime(string responseString)
        {
            var data = new List<EventChartReturnDto>();
            if (string.IsNullOrEmpty(responseString))
            {
                return data;
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            if (response?.aggregations?.datas?.buckets == null)
            {
                return data;
            }

            foreach (var item in response.aggregations.datas.buckets)
            {
                var value = new List<EventChartDto>();

                foreach (var subItem in item.historys.buckets)
                {
                    value.Add(new EventChartDto()
                    {
                        Date = subItem.key_as_string,
                        Value = subItem.doc_count,
                    });
                }

                data.Add(new EventChartReturnDto()
                {
                    LevelName = item.key,
                    Value = value,
                });
            }

            return data;
        }

        private async Task<List<PerformanceEventReturnDto>> GetLast10PerformanceEventAsync(string responseString, int workspaceId)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new List<PerformanceEventReturnDto>();
            }

            var hostsWorkspace = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.FindBy(h => h.WorkspaceId == workspaceId)).Select(x => x.Host.Name).ToList();

            var data = JsonConvert.DeserializeObject<dynamic>(responseString);
            var performanceEvents = new List<PerformanceEventReturnDto>();

            if (data == null)
            {
                return performanceEvents;
            }

            foreach (var item in data.hits?.hits)
            {
                foreach (var hostName in hostsWorkspace)
                {
                    if (item?._source?.event_host1 == hostName)
                    {
                        performanceEvents.Add(new PerformanceEventReturnDto
                        {
                            HostName = item?._source?.event_host1,
                            Time = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                            EventName = item?._source?.trigger_name,
                        });
                    }
                }
            }

            return performanceEvents;
        }

        private async Task<List<string>> GetHostsOfWorkSpace(string requestUserId, int? workspaceId)
        {
            var result = new List<string>();

            // Check role of users.
            var permissions = await this.GetPermissions(
                requestUserId,
                workspaceId,
                this.dashboardUnitOfWork.RolePermissionRepository,
                this.dashboardUnitOfWork.WorkspaceRolePermissionRepository);
            if (permissions.Any(p => p == (int)EnPermissions.AllDashboardsView || p == (int)EnPermissions.FullPermission) && (workspaceId == null || workspaceId <= 0))
            {
                return new List<string>();
            }

            // Get groups of work space.
            var workspaceIds = new List<int>();
            if (workspaceId == null || workspaceId <= 0)
            {
                workspaceIds = (from wru in await this.dashboardUnitOfWork.WorkspaceRoleUserRepository.GetAll()
                                join wr in await this.dashboardUnitOfWork.WorkspaceRoleRepository.GetAll() on wru
                                    .WorkspaceRoleId equals wr.Id
                                where wru.UserId == requestUserId
                                select wr.WorkspaceId).ToList();
            }
            else
            {
                workspaceIds.Add(workspaceId.Value);
            }

            var hostList = (from h in await this.dashboardUnitOfWork.HostRepository.GetAll()
                            join wh in await this.dashboardUnitOfWork.WorkspaceHostRepository.GetAll() on h.Id equals wh.HostId
                            where workspaceIds.Contains(wh.WorkspaceId)
                            select h.NameEngine.Replace(@"\", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty)).ToList();
            result.AddRange(hostList);

            if (result.Any())
            {
                return result;
            }

            result.Add(Guid.NewGuid().ToString());
            return result;
        }

        private async Task CheckPermissions(string currentUserId, int? workspaceId)
        {
            if (string.IsNullOrEmpty(currentUserId) || workspaceId == null || workspaceId < 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.DashboardView, (long)EnPermissions.AllDashboardsView, (long)EnPermissions.FullPermission }, this.dashboardUnitOfWork.RolePermissionRepository, this.dashboardUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }
        }
    }
}
