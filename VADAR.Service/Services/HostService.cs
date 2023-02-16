// <copyright file="HostService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Host Service.
    /// </summary>
    public class HostService : EntityService<Host>, IHostService
    {
        private readonly IHostUnitOfWork unitOfWork;
        private readonly ILicenseUnitOfWork licenseUnitOfWork;
        private readonly IWorkspaceUnitOfWork workspaceUnitOfWork;
        private readonly IWorkspaceHostUnitOfWork workspaceHostUnitOfWork;
        private readonly IAgentInstallUnitOfWork agentInstallUnitOfWork;
        private readonly ICallApiZabbixHelper callapiZabbixhelper;
        private readonly ICallApiWazuhHelper callApiWazuhHelper;
        private readonly IMapper mapper;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostService"/> class.
        /// </summary>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="licenseUnitOfWork">licenseUnitOfWork.</param>
        /// <param name="workspaceUnitOfWork">workspaceUnitOfWork.</param>
        /// <param name="callapiZabbixhelper">callapiZabbixhelper.</param>
        /// <param name="callApiWazuhHelper">callApiWazuhHelper.</param>
        /// <param name="workspaceHostUnitOfWork">workspaceHostUnitOfWork.</param>
        /// <param name="agentInstallUnitOfWork">agentInstallUnitOfWork.</param>
        /// <param name="mapper">mapper.</param>
        public HostService(
            IHostUnitOfWork unitOfWork,
            ILicenseUnitOfWork licenseUnitOfWork,
            IWorkspaceUnitOfWork workspaceUnitOfWork,
            IWorkspaceHostUnitOfWork workspaceHostUnitOfWork,
            IAgentInstallUnitOfWork agentInstallUnitOfWork,
            ICallApiZabbixHelper callapiZabbixhelper,
            ICallApiWazuhHelper callApiWazuhHelper,
            IMapper mapper)
            : base(unitOfWork, unitOfWork.HostRepository)
        {
            Guard.IsNotNull(unitOfWork, nameof(unitOfWork));
            Guard.IsNotNull(licenseUnitOfWork, nameof(licenseUnitOfWork));
            Guard.IsNotNull(agentInstallUnitOfWork, nameof(agentInstallUnitOfWork));
            Guard.IsNotNull(mapper, nameof(mapper));

            this.unitOfWork = unitOfWork;
            this.licenseUnitOfWork = licenseUnitOfWork;
            this.workspaceUnitOfWork = workspaceUnitOfWork;
            this.workspaceHostUnitOfWork = workspaceHostUnitOfWork;
            this.agentInstallUnitOfWork = agentInstallUnitOfWork;
            this.callapiZabbixhelper = callapiZabbixhelper;
            this.callApiWazuhHelper = callApiWazuhHelper;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<bool> AddHost(HostDto hostDto)
        {
            if (hostDto == null || string.IsNullOrWhiteSpace(hostDto.Name))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(hostDto));
            }

            if (hostDto == null || string.IsNullOrWhiteSpace(hostDto.TokenWorkspace))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(hostDto));
            }

            if (hostDto == null || string.IsNullOrWhiteSpace(hostDto.MachineId))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(hostDto));
            }

            var workspace = (await this.workspaceUnitOfWork.WorkspaceRepository.GetAll()).FirstOrDefault(g => g.TokenWorkspace == hostDto.TokenWorkspace.Trim());
            if (workspace == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            // Check License For Workspace.
            var licenseWorkspace = (await this.licenseUnitOfWork.LicenseRepository.FindBy(x => x.Id == workspace.LicenseId)).FirstOrDefault();
            if (licenseWorkspace != null && licenseWorkspace.Status == (int)EnLicenseStatus.Revoked)
            {
                throw new VadarException(ErrorCode.WorksapeNotActive, nameof(ErrorCode.WorksapeNotActive));
            }

            hostDto.CreatedById = workspace.CreatedById;

            hostDto.NameEngine = hostDto.Name;

            // Check host engine add host.
            var host = (await this.unitOfWork.HostRepository.GetAll()).FirstOrDefault(g => g.MachineId == hostDto.MachineId.Trim());
            if (!string.IsNullOrEmpty(host?.ZabbixRef) && !string.IsNullOrWhiteSpace(host?.ZabbixRef) && !string.IsNullOrEmpty(host?.WazuhRef) && !string.IsNullOrWhiteSpace(host?.WazuhRef))
            {
                throw new VadarException(ErrorCode.HostExist, nameof(ErrorCode.HostExist));
            }

            if (host == null)
            {
                // insert.
                var newHost = this.mapper.Map<Host>(hostDto);
                newHost.Id = Guid.NewGuid();

                var workspaceHost = new WorkspaceHost
                {
                    HostId = newHost.Id,
                    Host = newHost,
                    WorkspaceId = workspace.Id,
                };

                await this.workspaceHostUnitOfWork.WorkspaceHostRepository.Add(workspaceHost);
            }
            else
            {
                if (string.IsNullOrEmpty(host.ZabbixRef) || string.IsNullOrWhiteSpace(host.ZabbixRef))
                {
                    host.ZabbixRef = hostDto.ZabbixRef;
                }

                if (string.IsNullOrEmpty(host.WazuhRef) || string.IsNullOrWhiteSpace(host.WazuhRef))
                {
                    host.WazuhRef = hostDto.WazuhRef;
                }

                host.Os = string.IsNullOrEmpty(host.Os) || string.IsNullOrWhiteSpace(host.Os) ? hostDto.Os : host.Os;

                // Update.
                await this.unitOfWork.HostRepository.Edit(host);

                // Update Workspace host.
                // delete host tại workspace cũ .
                var workspaceHost = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.FindBy(x => x.HostId == host.Id)).FirstOrDefault();
                if (workspaceHost != null)
                {
                    await this.workspaceHostUnitOfWork.WorkspaceHostRepository.Delete(workspaceHost);
                }

                // thêm host vào workspace mới .
                var workspaceHostDto = new WorkspaceHostDto
                {
                    HostId = host.Id,
                    WorkspaceId = workspace.Id,
                };
                await this.workspaceHostUnitOfWork.WorkspaceHostRepository.Add(this.mapper.Map<WorkspaceHost>(workspaceHostDto));
            }

            // Create api insert Group Host engineRef Wazuh.
            if (!string.IsNullOrEmpty(hostDto.WazuhRef))
            {
                _ = await this.callApiWazuhHelper.AddAHostToGroupWazuh(hostDto.WazuhRef, workspace.WazuhRef);
            }

            // Create api insert Group Host engineRef Zabbix.
            if (!string.IsNullOrEmpty(hostDto.ZabbixRef))
            {
                var listHostZabbix = new List<string>
                {
                    hostDto.ZabbixRef,
                };
                _ = await this.callapiZabbixhelper.AddHostToGroup(listHostZabbix, workspace.ZabbixRef);
            }

            var result = await this.unitOfWork.Commit() > 0;
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> AddHostEngine(string hostName, string os, string ip)
        {
            await this.unitOfWork.HostRepository.Add(this.mapper.Map<Host>(new HostDto()));
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateHost(HostDto hostDto)
        {
            if (hostDto == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(hostDto));
            }

            if (hostDto.Id == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(hostDto));
            }

            if (!await this.ValidatePermission(hostDto.UpdatedById, hostDto.WorkspaceId, new[] { (long)EnPermissions.HostSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var host = await this.unitOfWork.HostRepository.GetHostById(hostDto.Id);
            if (host == null)
            {
                throw new VadarException(ErrorCode.HostNull, nameof(ErrorCode.HostNull));
            }

            if (!string.IsNullOrEmpty(hostDto.Name))
            {
                host.Name = hostDto.Name;
            }

            if (hostDto.Status >= 0)
            {
                host.Status = hostDto.Status;
            }

            host.Type = (EnTypeOfDevices)hostDto.Type;
            host.Description = hostDto.Description;
            host.UpdateById = host.UpdateById;
            host.UpdatedDate = host.UpdatedDate;

            await this.unitOfWork.HostRepository.Edit(host);

            var listGroupHost = (await this.unitOfWork.GroupHostRepository.GetAll())
                .Include(group => group.Group)
                .Include(groupHost => groupHost.Host)
                .Where(gh => gh.Group.WorkspaceId == hostDto.WorkspaceId)
                .Where(g => g.HostId == host.Id).Select(x => x);

            if (listGroupHost.Any())
            {
                foreach (var groupHost in listGroupHost)
                {
                    var listHostWazuh = new List<string>
                    {
                        groupHost.Host.WazuhRef,
                    };
                    var listHostZabbix = new List<string>
                    {
                        groupHost.Host.ZabbixRef,
                    };

                    // xóa các bản ghi host hiện tại ở các group server - engine.
                    if (host.WazuhRef != null)
                    {
                        var responWazuh = await this.callApiWazuhHelper.RemoveHostFromGroupWazuh(listHostWazuh, groupHost.Group.WazuhRef);
                        var dataWazuh = JsonConvert.DeserializeObject<dynamic>(responWazuh);
                        if (dataWazuh?.error != 0)
                        {
                            throw new VadarException(ErrorCode.EngineNotWork, nameof(ErrorCode.EngineNotWork));
                        }
                    }

                    if (host.ZabbixRef != null)
                    {
                        var listgroupZabbix = new List<string>
                        {
                            groupHost.Group.ZabbixRef,
                        };
                        var response = await this.callapiZabbixhelper.RemoveHostFromGroup(listHostZabbix, listgroupZabbix);
                        var data = JsonConvert.DeserializeObject<dynamic>(response);
                        if (data?.result == null)
                        {
                            throw new VadarException(ErrorCode.EngineNotWork, nameof(ErrorCode.EngineNotWork));
                        }
                    }

                    _ = await this.unitOfWork.GroupHostRepository.Delete(groupHost);
                }
            }

            // thêm các bản ghi host hiện tại ở các group server - engine.
            if (!hostDto.Groups.Any())
            {
                return await this.unitOfWork.Commit() > 0;
            }

            foreach (var item in hostDto.Groups)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                var group = await this.unitOfWork.GroupRepository.GetGroupById(Guid.Parse(item));
                if (host.WazuhRef != null)
                {
                    await this.callApiWazuhHelper.AddAHostToGroupWazuh(host.WazuhRef, @group.WazuhRef);
                }

                if (host.ZabbixRef != null)
                {
                    var listHostZabbix = new List<string>
                    {
                        host.ZabbixRef,
                    };
                    await this.callapiZabbixhelper.AddHostToGroup(listHostZabbix, @group.ZabbixRef);
                }

                var groupHostDto = new GroupHostDto { GroupId = @group.Id, HostId = host.Id };
                _ = await this.unitOfWork.GroupHostRepository.Add(this.mapper.Map<GroupHost>(groupHostDto));
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<HostViewModelDto> GetHostByName(string hostName)
        {
            var host = await this.unitOfWork.HostRepository.GetHostByName(hostName);
            if (host == null)
            {
                throw new VadarException(ErrorCode.HostNull);
            }

            return this.mapper.Map<HostViewModelDto>(host);
        }

        /// <inheritdoc/>
        public async Task<HostResultPagingDto> GetAllHost(HostPagingRequestDto hostPagingRequestDto, string currentUserId)
        {
            if (hostPagingRequestDto == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(hostPagingRequestDto));
            }

            if (hostPagingRequestDto.CheckExist == 1)
            {
                hostPagingRequestDto.PageIndex = 0;
                hostPagingRequestDto.PageSize = 0;
            }

            if (hostPagingRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(hostPagingRequestDto.WorkspaceId));
            }

            if (!await this.ValidatePermission(currentUserId, hostPagingRequestDto.WorkspaceId, new[] { (long)EnPermissions.HostView, (long)EnPermissions.HostSetting, (long)EnPermissions.GroupSetting, (long)EnPermissions.GroupView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var workspace = (await this.workspaceUnitOfWork.WorkspaceRepository.FindBy(w => w.Id == hostPagingRequestDto.WorkspaceId)).FirstOrDefault();
            if (workspace == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var workspaceHosts = (await this.unitOfWork.WorkspaceHostRepository.GetAll())
                .Include(i => i.Host)
                .ThenInclude(g => g.GroupHosts)
                .Where(x => x.WorkspaceId == hostPagingRequestDto.WorkspaceId);

            if (hostPagingRequestDto.GroupId != Guid.Empty)
            {
                var hostsExist = workspaceHosts.Where(x => x.Host.GroupHosts.Any(g => g.GroupId == hostPagingRequestDto.GroupId));

                if (hostPagingRequestDto.CheckExist == 1)
                {
                    var hIds = hostsExist.Select(h => h.HostId).ToList();
                    workspaceHosts = workspaceHosts.Where(h => !hIds.Contains(h.HostId));
                }
                else
                {
                    workspaceHosts = hostsExist;
                }
            }

            if (!string.IsNullOrEmpty(hostPagingRequestDto.HostName))
            {
                workspaceHosts = workspaceHosts.Where(q => q.Host.Name.Trim().ToLower().StartsWith(hostPagingRequestDto.HostName.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(hostPagingRequestDto.CreatedById))
            {
                workspaceHosts = workspaceHosts.Where(q => q.Host.CreatedById == hostPagingRequestDto.CreatedById);
            }

            var hostViews = workspaceHosts.Select(s => this.mapper.Map<HostViewModelDto>(s.Host)).ToList();

            // check status
            await this.CheckHostStatus(workspace.WazuhRef, workspace.ZabbixRef, hostViews);
            var hostIndex = new List<HostViewModelDto>();
            if (hostPagingRequestDto.Status != 0)
            {
                hostViews = hostViews.Where(x => x.Status == hostPagingRequestDto.Status).ToList();
                hostIndex = hostViews;
            }
            else
            {
                hostIndex = workspaceHosts.Select(s => this.mapper.Map<HostViewModelDto>(s.Host)).ToList();
            }

            if (hostPagingRequestDto.CheckExist != 1)
            {
                hostViews = hostViews.OrderByDescending(o => o.Id).Skip(hostPagingRequestDto.PageSize * (hostPagingRequestDto.PageIndex - 1))
                            .Take(hostPagingRequestDto.PageSize)
                            .ToList();
            }

            var agentInstalls = (await this.agentInstallUnitOfWork.AgentInstallRepository.GetAll())
               .Include(i => i.AgentOs)
               .Where(a => a.AgentOs.WorkspaceId == hostPagingRequestDto.WorkspaceId).ToList();

            var result = hostViews
                .Select(h => new HostViewModelDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    NameEngine = h.NameEngine,
                    Os = h.Os,
                    CreatedDate = h.CreatedDate,
                    Description = h.Description,
                    Status = h.Status,
                    Type = h.Type,
                    Ip = h.Ip,
                    GroupName = h.GroupName,
                    GroupId = h.GroupId,
                    WazuhRef = h.WazuhRef,
                    ZabbixRef = h.ZabbixRef,
                    MachineId = h.MachineId,
                    LinkDownloadPerformance = agentInstalls.Where(x => !string.IsNullOrWhiteSpace(h.Os) && h.Os.ToLower().Contains(x.AgentOs.Name.ToLower()) && x.Name.StartsWith("Performance"))
                                                    .Select(s => new LinkDownloadViewDto
                                                    {
                                                        Url = s.LinkDownload,
                                                        AgentName = s.Name,
                                                    }).ToList(),
                    LinkDownloadSecurity = agentInstalls.Where(x => !string.IsNullOrWhiteSpace(h.Os) && h.Os.ToLower().Contains(x.AgentOs.Name.ToLower()) && x.Name.StartsWith("Security"))
                                                    .Select(s => new LinkDownloadViewDto
                                                    {
                                                        Url = s.LinkDownload,
                                                        AgentName = s.Name,
                                                    }).ToList(),
                });

            return new HostResultPagingDto
            {
                Count = hostIndex.Count(),
                Items = result,
            };
        }

        /// <inheritdoc/>
        public async Task<bool> CheckHostStatus(string wazuhRef, string zabbixRef, List<HostViewModelDto> hostViews)
        {
        // var dataZabbix = JsonConvert.DeserializeObject<dynamic>(await this.callapiZabbixhelper.GetHostByGroup(zabbixRef));
            var dataWazuh = JsonConvert.DeserializeObject<dynamic>(await this.callApiWazuhHelper.GetAllHostByGroup(wazuhRef));
            foreach (var hostDb in hostViews)
            {
                // wazuh .
                var hostWazuh = ((IEnumerable)dataWazuh?.data.items) !.Cast<dynamic>()
                    .FirstOrDefault(z => z.name == hostDb.NameEngine);
                if (hostWazuh != null)
                {
                    if (hostWazuh.status == EnWazuhStatus.Active.ToString())
                    {
                        hostDb.Status = (int)EnHostStatus.Online;
                    }

                    if (hostWazuh.status == EnWazuhStatus.Disconnected.ToString())
                    {
                        hostDb.Status = (int)EnHostStatus.Offline;
                    }

                    hostDb.Ip = hostWazuh.ip;
                }

                // zabbix.
                // var hostZabbix = ((IEnumerable)dataZabbix?.result ?? throw new InvalidOperationException()).Cast<dynamic>().FirstOrDefault(z => z.name == hostDb.NameEngine);
                // if (hostZabbix != null)
                // {
                //    if (hostDb.Status == (int)EnHostStatus.Offline)
                //    {
                //        if (hostZabbix.status == EnZabbixStatus.Disconnect)
                //        {
                //            hostDb.Status = (int)EnHostStatus.Offline;
                //        }

                // if (hostZabbix.status == EnZabbixStatus.Enable)
                //        {
                //            hostDb.Status = (int)EnHostStatus.Online;
                //        }
                //    }
                //    else
                //    {
                //        hostDb.Status = (int)EnHostStatus.Online;
                //    }
                // }
                hostDb.GroupName = string.Join(", ", this.unitOfWork.GroupHostRepository.GetAll().Result.Where(x => x.HostId == hostDb.Id).Select(s => s.Group.Name));
                hostDb.GroupId = string.Join(",", this.unitOfWork.GroupHostRepository.GetAll().Result.Where(x => x.HostId == hostDb.Id).Select(s => s.Group.Id));
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<HostResultPagingDto> GetHostNotExistGroup(HostPagingRequestDto hostPagingRequestDto, string currentUserId)
        {
            if (hostPagingRequestDto == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(hostPagingRequestDto));
            }

            if (hostPagingRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(hostPagingRequestDto.WorkspaceId));
            }

            if (!await this.ValidatePermission(currentUserId, hostPagingRequestDto.WorkspaceId, new[] { (long)EnPermissions.HostView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var hosts = (await this.unitOfWork.WorkspaceHostRepository.GetAll())
                .Include(i => i.Host)
                .ThenInclude(g => g.GroupHosts)
                .Where(x => x.WorkspaceId == hostPagingRequestDto.WorkspaceId)
                .Select(s => s.Host).ToList();

            if (hostPagingRequestDto.GroupId != Guid.Empty)
            {
                var hosts1 = hosts.Select(h => h.Id).ToArray();
                var groupHosts = (await this.unitOfWork.GroupHostRepository.GetAll())
                    .Include(t => t.Group)
                    .Include(i => i.Host)
                    .ThenInclude(g => g.GroupHosts)
                    .Where(x => hosts1.Any(h => h == x.HostId));

                hosts = groupHosts.Where(q => q.GroupId != hostPagingRequestDto.GroupId).Select(s => s.Host).ToList();
            }

            if (!string.IsNullOrEmpty(hostPagingRequestDto.HostName))
            {
                hosts = hosts.Where(q => q.Name.Trim().ToLower().StartsWith(hostPagingRequestDto.HostName.Trim().ToLower())).ToList();
            }

            var result = hosts
                .Select(h => new HostViewModelDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Os = h.Os,
                    CreatedDate = h.CreatedDate,
                    Description = h.Description,
                    Status = h.Status,
                    GroupName = h.GroupHosts != null ? string.Join(", ", this.unitOfWork.GroupHostRepository.GetAll().Result.Where(x => x.HostId == h.Id).Select(s => s.Group.Name)) : string.Empty,
                })
                .OrderByDescending(o => o.Id)
                .Skip(hostPagingRequestDto.PageSize * (hostPagingRequestDto.PageIndex - 1))
                .Take(hostPagingRequestDto.PageSize);

            return new HostResultPagingDto
            {
                Count = hosts.Count,
                Items = result,
            };
        }

        /// <inheritdoc/>
        public async Task<HostViewModelDto> GetHostById(Guid hostId)
        {
            var host = await this.unitOfWork.HostRepository.GetHostById(hostId);
            if (host == null)
            {
                throw new VadarException(ErrorCode.HostNull);
            }

            return this.mapper.Map<HostViewModelDto>(host);
        }

        /// <inheritdoc/>
        public async Task<dynamic> GetAllHostEngine(int type, int tokenWorkspace, string module, string currentId)
        {
            var workspace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(tokenWorkspace);
            var groupIdWazuh = workspace.WazuhRef;
            var groupIdZabbix = workspace.ZabbixRef;

            var listJob = new List<dynamic>();
            switch (type)
            {
                case (int)EnEventType.Wazuh:
                    {
                        var responseWazuh = await this.callApiWazuhHelper.GetAllHostByGroup(groupIdWazuh);
                        var dataWazuh = JsonConvert.DeserializeObject<dynamic>(responseWazuh);

                        if (module == "job")
                        {
                            if (dataWazuh != null)
                            {
                                foreach (var item in dataWazuh.data.items)
                                {
                                    listJob.Add(item);
                                }
                            }
                        }
                        else
                        {
                            return dataWazuh?.data.items;
                        }

                        break;
                    }

                case (int)EnEventType.Zabbix:
                    {
                        var resultHostzabbix = await this.callapiZabbixhelper.GetHostByGroup(groupIdZabbix);
                        var hostRabbix = JsonConvert.DeserializeObject<dynamic>(resultHostzabbix);

                        if (module == "job")
                        {
                            if (hostRabbix != null)
                            {
                                foreach (var item in hostRabbix.result)
                                {
                                    listJob.Add(item);
                                }
                            }
                        }
                        else
                        {
                            return hostRabbix?.result;
                        }

                        break;
                    }
            }

            if (module != "job")
            {
                return string.Empty;
            }

            foreach (var item in listJob)
            {
                var hostDto = new HostDto { Status = 1, CreatedDate = DateTime.Now, CreatedById = currentId };
                switch (type)
                {
                    case 1:
                        hostDto.NameEngine = item.name;
                        hostDto.Name = item.name;
                        hostDto.WazuhRef = item.id;
                        hostDto.MachineId = item.id + item.name;
                        hostDto.Os = item.os?.name;
                        break;
                    case 2:
                        hostDto.NameEngine = item.host;
                        hostDto.Name = item.host;
                        hostDto.ZabbixRef = item.hostid;
                        hostDto.MachineId = item.host + item.hostid;
                        hostDto.Os = item.name;
                        break;
                }

                var host = await this.unitOfWork.HostRepository.GetHostByName(hostDto.Name);
                if (host != null)
                {
                    host.ZabbixRef = hostDto.ZabbixRef;
                    host.WazuhRef = hostDto.WazuhRef;
                    host.Name = hostDto.Name;
                    await this.unitOfWork.HostRepository.Edit(host);

                    var workspaceHostDto = new WorkspaceHostDto
                    {
                        HostId = host.Id,
                        WorkspaceId = workspace.Id,
                    };
                    var workspaceHost = (await this.unitOfWork.WorkspaceHostRepository.FindBy(x => x.HostId == host.Id && x.WorkspaceId == workspace.Id)).FirstOrDefault();
                    if (workspaceHost == null)
                    {
                        await this.workspaceHostUnitOfWork.WorkspaceHostRepository.Add(this.mapper.Map<WorkspaceHost>(workspaceHostDto));
                    }
                }
                else
                {
                    var hostInsert = await this.unitOfWork.HostRepository.Add(this.mapper.Map<Host>(hostDto));

                    var workspaceHostDto = new WorkspaceHostDto
                    {
                        HostId = hostInsert.Id,
                        WorkspaceId = workspace.Id,
                    };
                    await this.workspaceHostUnitOfWork.WorkspaceHostRepository.Add(this.mapper.Map<WorkspaceHost>(workspaceHostDto));

                    // await this.unitOfWork.HostRepository.Add(host);
                }
            }

            await this.unitOfWork.Commit();

            return string.Empty;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteHost(Guid hostId, int workspaceId, string currentUserId)
        {
            if (hostId == Guid.Empty || workspaceId <= 0 || string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull, $"{nameof(hostId)} || {nameof(workspaceId)} || {nameof(currentUserId)}");
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.HostSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var host = await this.unitOfWork.HostRepository.GetHostById(hostId);
            if (host == null)
            {
                throw new VadarException(ErrorCode.HostNull, nameof(ErrorCode.HostNull));
            }

            var listHostGroup = (await this.unitOfWork.GroupHostRepository.GetAll()).Where(g => g.HostId == host.Id).Select(x => x);
            if (listHostGroup.Any())
            {
                foreach (var groupHost in listHostGroup)
                {
                    // xóa các bản ghi host hiện tại ở các group server - engine.
                    _ = await this.unitOfWork.GroupHostRepository.Delete(groupHost);
                }
            }

            if (!string.IsNullOrWhiteSpace(host.WazuhRef))
            {
                _ = await this.callApiWazuhHelper.RemoveHostWazuh(host.WazuhRef);
            }

            if (!string.IsNullOrWhiteSpace(host.ZabbixRef))
            {
                _ = await this.callapiZabbixhelper.DeleteHost(new List<string> { host.ZabbixRef });
            }

            await this.unitOfWork.HostRepository.Delete(host);
            return await this.unitOfWork.Commit() > 0;
        }
    }
}
