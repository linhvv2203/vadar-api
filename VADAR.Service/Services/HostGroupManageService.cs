// <copyright file="HostGroupManageService.cs" company="VSEC">
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
    /// HostGroupManage Service.
    /// </summary>
    public class HostGroupManageService : EntityService<GroupHost>, IHostGroupManageService
    {
        private readonly ICallApiZabbixHelper callApiZabbixHelper;
        private readonly ICallApiWazuhHelper callApiWazuhHelper;
        private readonly IGroupHostUnitOfWork unitOfWork;
        private readonly IHostUnitOfWork hostUnitOfWork;
        private readonly IGroupUnitOfWork groupUnitOfWork;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostGroupManageService"/> class.
        /// </summary>
        /// <param name="callApiZabbixHelper">callApiZabbixHelper.</param>
        /// <param name="callApiWazuhHelper">callApiWazuhHelper.</param>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="hostUnitOfWork">hostUnitOfWork.</param>
        /// <param name="groupUnitOfWork">groupUnitOfWork.</param>
        public HostGroupManageService(
            ICallApiZabbixHelper callApiZabbixHelper,
            ICallApiWazuhHelper callApiWazuhHelper,
            IGroupHostUnitOfWork unitOfWork,
            IHostUnitOfWork hostUnitOfWork,
            IGroupUnitOfWork groupUnitOfWork)
            : base(unitOfWork, unitOfWork.GroupHostRepository)
        {
            this.callApiZabbixHelper = callApiZabbixHelper;
            this.callApiWazuhHelper = callApiWazuhHelper;
            this.unitOfWork = unitOfWork;
            this.hostUnitOfWork = hostUnitOfWork;
            this.groupUnitOfWork = groupUnitOfWork;
        }

        /// <inheritdoc/>
        public async Task<ResultHostGroupDto> AddHostGroupByNames(string name)
        {
            var result = await this.AddHostGroupByName(name);

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultHostGroupDto> RemoveHostGroupByName(List<int> idGroup)
        {
            var result = new ResultHostGroupDto();
            var responseNameWazuh = await this.callApiZabbixHelper.GetGroupById(idGroup);
            var listWazuh = JsonConvert.DeserializeObject<dynamic>(responseNameWazuh);
            foreach (var item in listWazuh.result)
            {
                var responseWazuh = await this.callApiWazuhHelper.RemoveHostWazuh(item.name.ToString());
                var deseriResponse = JsonConvert.DeserializeObject<dynamic>(responseWazuh);
                if (deseriResponse.data.failed_ids == null)
                {
                    continue;
                }

                result.Data = deseriResponse.data.failed_ids;
                return result;
            }

            var response = await this.callApiZabbixHelper.DeleteGroup(idGroup);
            var data = JsonConvert.DeserializeObject<dynamic>(response);
            result.Data = data;
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> AddHostToGroupById(AddHostToGroupByIdRequestDto addHostToGroupByIdRequestDto, string currentUserId)
        {
            if (addHostToGroupByIdRequestDto is null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(addHostToGroupByIdRequestDto));
            }

            if (addHostToGroupByIdRequestDto.HostIds.Count == 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(addHostToGroupByIdRequestDto.HostIds));
            }

            var group = (await this.groupUnitOfWork.GroupRepository.GetAll()).FirstOrDefault(g => g.Id == addHostToGroupByIdRequestDto.GroupId);
            if (group == null)
            {
                throw new VadarException(ErrorCode.GroupNull, nameof(ErrorCode.GroupNull));
            }

            if (!await this.ValidatePermission(currentUserId, group.WorkspaceId, new[] { (long)EnPermissions.HostSetting, (long)EnPermissions.GroupSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var listHostZabbix = new List<string>();
            var listHostWazuh = new List<string>();
            var check = false;
            foreach (var hostId in addHostToGroupByIdRequestDto.HostIds)
            {
                var host = (await this.hostUnitOfWork.HostRepository.GetAll()).FirstOrDefault(g => g.Id == hostId);
                if (host?.ZabbixRef != null)
                {
                    listHostZabbix.Add(host.ZabbixRef);
                }

                if (host?.WazuhRef != null)
                {
                    listHostWazuh.Add(host.WazuhRef);
                }

                var grouphost = (await this.unitOfWork.GroupHostRepository.GetAll()).FirstOrDefault(g => g.GroupId == addHostToGroupByIdRequestDto.GroupId && g.HostId == hostId);
                if (grouphost != null)
                {
                    continue;
                }

                _ = await this.unitOfWork.GroupHostRepository.Add(new GroupHost
                {
                    GroupId = addHostToGroupByIdRequestDto.GroupId,
                    HostId = hostId,
                });
                check = true;
            }

            if (listHostWazuh.Count > 0)
            {
                foreach (var item in listHostWazuh)
                {
                    var responseWazuh = await this.callApiWazuhHelper.AddAHostToGroupWazuh(item, group.WazuhRef);
                    var dataResponse = JsonConvert.DeserializeObject<dynamic>(responseWazuh);
                    if (dataResponse?.error != 0)
                    {
                        throw new VadarException(ErrorCode.EngineNotWork, nameof(ErrorCode.EngineNotWork));
                    }
                }
            }

            if (listHostZabbix.Count > 0)
            {
                await this.callApiZabbixHelper.AddHostToGroup(listHostZabbix, @group.ZabbixRef);
            }

            if (!check)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(ErrorCode.ArgumentNull));
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveHostFromGroup(List<string> id, string groupId, string currentUserId)
        {
            if (id.Count == 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(id));
            }

            var group = (await this.groupUnitOfWork.GroupRepository.GetAll()).FirstOrDefault(g => g.Id == Guid.Parse(groupId));
            if (group == null)
            {
                throw new VadarException(ErrorCode.GroupNull, nameof(ErrorCode.GroupNull));
            }

            if (!await this.ValidatePermission(currentUserId, group.WorkspaceId, new[] { (long)EnPermissions.HostSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var listHostZabbix = new List<string>();
            var listHostWazuh = new List<string>();
            var check = false;
            foreach (var item in id)
            {
                var host = (await this.hostUnitOfWork.HostRepository.GetAll()).FirstOrDefault(g => g.Id == Guid.Parse(item));
                if (host?.ZabbixRef != null)
                {
                    listHostZabbix.Add(host.ZabbixRef);
                }

                if (host?.WazuhRef != null)
                {
                    listHostWazuh.Add(host.WazuhRef);
                }

                var grouphost = (await this.unitOfWork.GroupHostRepository.GetAll()).FirstOrDefault(g => g.GroupId == Guid.Parse(groupId) && g.HostId == Guid.Parse(item));
                if (grouphost == null)
                {
                    continue;
                }

                await this.unitOfWork.GroupHostRepository.Delete(grouphost);
                check = true;
            }

            if (listHostZabbix.Count > 0)
            {
                await this.callApiWazuhHelper.RemoveHostFromGroupWazuh(listHostWazuh, @group.WazuhRef);
            }

            var listgroupZabbix = new List<string>
            {
                group.ZabbixRef,
            };
            if (listHostZabbix.Any())
            {
                await this.callApiZabbixHelper.RemoveHostFromGroup(listHostZabbix, listgroupZabbix);
            }

            if (!check)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(ErrorCode.ArgumentNull));
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckHostAlreadyExistsInGroup(AddHostToGroupByIdRequestDto addHostToGroupByIdRequestDto)
        {
            var hostCount = (await this.unitOfWork.GroupHostRepository.GetAll())
                .Count(x => x.Group.WorkspaceId == addHostToGroupByIdRequestDto.WorkspaceId
                && x.Host.Id == addHostToGroupByIdRequestDto.HostIds.FirstOrDefault());
            return hostCount > 0;
        }

        private async Task<ResultHostGroupDto> AddHostGroupByName(string name)
        {
            var result = new ResultHostGroupDto();
            if (string.IsNullOrEmpty(name))
            {
                return new ResultHostGroupDto();
            }

            var responseWazuh = await this.callApiWazuhHelper.AddHostWazuh(name);
            var response = await this.callApiZabbixHelper.AddGroup(name);
            var data = JsonConvert.DeserializeObject<dynamic>(response);
            var dataWazuh = JsonConvert.DeserializeObject<dynamic>(responseWazuh);
            if (dataWazuh.error != 0)
            {
                return dataWazuh;
            }

            result.Data = data;
            return result;
        }
    }
}
