// <copyright file="GroupService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
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
    /// Group Service.
    /// </summary>
    public class GroupService : EntityService<Group>, IGroupService
    {
        private readonly IGroupUnitOfWork unitOfWork;
        private readonly IWorkspaceUnitOfWork workspaceUnitOfWork;
        private readonly ICallApiZabbixHelper callApiZabbixHelper;
        private readonly ICallApiWazuhHelper callApiWazuhHelper;
        private readonly IMapper mapper;

        /// <summary>
        /// Initialises a new instance of the <see cref="GroupService"/> class.
        /// </summary>
        /// <param name="workspaceUnitOfWork">workspaceUnitOfWork.</param>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="mapper">mapper.</param>
        /// <param name="callApiZabbixHelper">callApiZabbixHelper.</param>
        /// <param name="callApiWazuhHelper">callApiWazuhHelper.</param>
        public GroupService(
            IGroupUnitOfWork unitOfWork,
            IWorkspaceUnitOfWork workspaceUnitOfWork,
            IMapper mapper,
            ICallApiZabbixHelper callApiZabbixHelper,
            ICallApiWazuhHelper callApiWazuhHelper)
            : base(unitOfWork, unitOfWork.GroupRepository)
        {
            Guard.IsNotNull(unitOfWork, nameof(unitOfWork));
            Guard.IsNotNull(mapper, nameof(mapper));
            Guard.IsNotNull(callApiZabbixHelper, nameof(callApiZabbixHelper));
            Guard.IsNotNull(callApiZabbixHelper, nameof(callApiZabbixHelper));

            this.unitOfWork = unitOfWork;
            this.workspaceUnitOfWork = workspaceUnitOfWork;
            this.mapper = mapper;
            this.callApiZabbixHelper = callApiZabbixHelper;
            this.callApiWazuhHelper = callApiWazuhHelper;
        }

        /// <inheritdoc/>
        public async Task<bool> AddGroup(GroupDto groupDto, string currentUserId)
        {
            if (groupDto == null || string.IsNullOrWhiteSpace(groupDto.Name))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(groupDto));
            }

            if (groupDto == null || groupDto.WorkspaceId == 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(groupDto));
            }

            if (!await this.ValidatePermission(currentUserId, groupDto.WorkspaceId, new[] { (long)EnPermissions.GroupSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var workspace = (await this.workspaceUnitOfWork.WorkspaceRepository.GetAll()).FirstOrDefault(g => g.Id == groupDto.WorkspaceId);
            if (workspace == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            var groupNameEngine = workspace.Id + "_" + groupDto.Name.Trim().ToUpper();

            var groupsZabbix = await this.callApiZabbixHelper.FindGroupByName(groupNameEngine);
            if (groupsZabbix != null && !string.IsNullOrEmpty(groupsZabbix.Name))
            {
                throw new VadarException(ErrorCode.GroupExists, nameof(ErrorCode.GroupExists));
            }

            var groupsWazuh = await this.callApiWazuhHelper.GetGroupDetail(groupNameEngine);
            if (groupsWazuh != null && !string.IsNullOrEmpty(groupsWazuh.Name))
            {
                throw new VadarException(ErrorCode.GroupExists, nameof(ErrorCode.GroupExists));
            }

            var resultZabbix = await this.callApiZabbixHelper.AddGroup(groupNameEngine);
            var groupZabbix = JsonConvert.DeserializeObject<dynamic>(resultZabbix);
            string groupIdZabbix = groupZabbix?.result?.groupids[0];

            var resultWazuh = await this.callApiWazuhHelper.CreateAGroup(groupNameEngine);
            var groupWazuh = JsonConvert.DeserializeObject<dynamic>(resultWazuh);
            var groupIdWazuh = string.Empty;
            if (groupWazuh?.error == 0)
            {
                groupIdWazuh = groupNameEngine;
            }

            groupDto.ZabbixRef = groupIdZabbix;
            groupDto.WazuhRef = groupIdWazuh;

            await this.unitOfWork.GroupRepository.Add(this.mapper.Map<Group>(groupDto));
            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteGroup(Guid groupId, string currentUserId)
        {
            var group = await this.unitOfWork.GroupRepository.GetGroupById(groupId);
            if (group == null)
            {
                throw new VadarException(ErrorCode.GroupNull, nameof(ErrorCode.GroupNull));
            }

            if (!await this.ValidatePermission(currentUserId, group.WorkspaceId, new[] { (long)EnPermissions.GroupSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var groupNameEngine = group.WorkspaceId + "_" + group.Name.Trim().ToUpper();

            _ = await this.callApiWazuhHelper.RemoveAGroup(groupNameEngine);
            var zabbixGroup = await this.callApiZabbixHelper.FindGroupByName(groupNameEngine);
            if (zabbixGroup != null && !string.IsNullOrEmpty(zabbixGroup.Name))
            {
                _ = await this.callApiZabbixHelper.DeleteGroup(new List<int> { zabbixGroup.Id });
            }

            await this.unitOfWork.GroupRepository.Delete(group);
            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<GroupResultPagingDto> GetAllGroup(GroupPagingRequestDto groupRequestDto, string currentUserId)
        {
            if (groupRequestDto == null || groupRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(groupRequestDto));
            }

            if (!await this.ValidatePermission(currentUserId, groupRequestDto.WorkspaceId, new[] { (long)EnPermissions.GroupView, (long)EnPermissions.GroupSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var query = await this.unitOfWork.GroupRepository.GetAll();
            query = query.Where(x => x.WorkspaceId == groupRequestDto.WorkspaceId);
            if (!string.IsNullOrEmpty(groupRequestDto.GroupName))
            {
                query = query.Where(q => q.Name.ToUpper().Contains(groupRequestDto.GroupName.ToUpper().Trim()));
            }

            query = query.Include(x => x.GroupHosts);
            var groups = await query
                            .Select(g => new GroupViewModelDto
                            {
                                Id = g.Id,
                                Name = g.Name,
                                HostName = string.Join(", ", g.GroupHosts.Select(x => x.Host.Name)),
                                NumberOfHost = g.GroupHosts.Count,
                            })
                            .OrderByDescending(o => o.Id)
                            .Skip(groupRequestDto.PageSize * (groupRequestDto.PageIndex - 1))
                            .Take(groupRequestDto.PageSize)
                            .ToListAsync();

            return new GroupResultPagingDto
            {
                Count = await query.CountAsync(),
                Items = groups,
            };
        }

        /// <inheritdoc/>
        public async Task<GroupViewModelDto> GetGroupById(Guid groupId, string currentUserId)
        {
            var group = await this.unitOfWork.GroupRepository.GetGroupById(groupId);
            if (group == null)
            {
                throw new VadarException(ErrorCode.GroupNull);
            }

            if (!await this.ValidatePermission(currentUserId, group.WorkspaceId, new[] { (long)EnPermissions.GroupView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            return this.mapper.Map<GroupViewModelDto>(group);
        }

        /// <inheritdoc/>
        public async Task<GroupViewModelDto> GetGroupByName(string groupName, string currentUserId)
        {
            var group = await this.unitOfWork.GroupRepository.GetGroupByName(groupName);
            if (group == null)
            {
                var groupsZabbix = await this.callApiZabbixHelper.FindGroupByName(groupName);
                if (groupsZabbix != null && !string.IsNullOrEmpty(groupsZabbix.Name))
                {
                    var result = new GroupViewModelDto
                    {
                        Name = groupsZabbix.Name,
                    };

                    return result;
                }

                var wazuhGroups = await this.callApiWazuhHelper.GetGroupDetail(groupName);
                if (wazuhGroups != null && !string.IsNullOrEmpty(wazuhGroups.Name))
                {
                    var result = new GroupViewModelDto
                    {
                        Name = wazuhGroups.Name,
                    };

                    return result;
                }

                throw new VadarException(ErrorCode.GroupNull);
            }

            if (!await this.ValidatePermission(currentUserId, group.WorkspaceId, new[] { (long)EnPermissions.GroupView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            return this.mapper.Map<GroupViewModelDto>(group);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateGroup(GroupDto groupDto)
        {
            if (groupDto == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(groupDto));
            }

            if (!await this.ValidatePermission(groupDto.UpdatedById, groupDto.WorkspaceId, new[] { (long)EnPermissions.GroupSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var group = await this.unitOfWork.GroupRepository.GetGroupByName(groupDto.Name);
            if (group == null)
            {
                throw new VadarException(ErrorCode.GroupNull, nameof(ErrorCode.GroupNull));
            }

            _ = await this.callApiZabbixHelper.UpdateGroup(groupDto);

            _ = await this.callApiWazuhHelper.RemoveAGroup(groupDto.Name);
            _ = await this.callApiWazuhHelper.AddHostWazuh(group.Name);

            group.Name = groupDto.Name;
            group.Description = groupDto.Description;
            group.UpdateById = group.UpdateById;
            group.UpdatedDate = group.UpdatedDate;

            await this.unitOfWork.GroupRepository.Edit(group);
            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteGroupByIds(Guid[] groupIds, string currentUserId)
        {
            if (groupIds is null || !groupIds.Any())
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(groupIds));
            }

            var groups = await this.unitOfWork.GroupRepository.GetAll();
            groups = groups.Where(g => groupIds.Any(x => x.Equals(g.Id)));
            if (!groups.Any())
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(ErrorCode.ArgumentInvalid));
            }

            foreach (var group in groups)
            {
                if (!await this.ValidatePermission(currentUserId, group.WorkspaceId, new[] { (long)EnPermissions.GroupSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
                {
                    throw new VadarException(ErrorCode.Forbidden);
                }
            }

            // remove list group in wazuh.
            var groupNames = groups.Select(x => x.WazuhRef).ToArray();
            var responseWazuh = await this.callApiWazuhHelper.RemoveListOfGroups(groupNames);
            var dataResponse = JsonConvert.DeserializeObject<dynamic>(responseWazuh);
            if (dataResponse?.error != 0)
            {
                throw new VadarException(ErrorCode.EngineNotWork, nameof(ErrorCode.EngineNotWork));
            }

            // remove list group in zabbix.
            // var ids = new List<int>();
            // foreach (var item in groups)
            // {
            //    var zabbixGroup = await this.callApiZabbixHelper.FindGroupByName(item.ZabbixRef);
            //    if (zabbixGroup != null && !string.IsNullOrEmpty(zabbixGroup.Name))
            //    {
            //        ids.Add(zabbixGroup.Id);
            //    }
            // }

            // ids = ids.Where(x => x > 0).Distinct().ToList();
            // var response = await this.callApiZabbixHelper.DeleteGroup(ids);
            // var data = JsonConvert.DeserializeObject<dynamic>(response);
            // if (data?.result[0] == null)
            // {
            //    throw new VADARException(ErrorCode.EngineNotWork, nameof(ErrorCode.EngineNotWork));
            // }
            foreach (var item in groups)
            {
                await this.unitOfWork.GroupRepository.Delete(item);
            }

            return await this.unitOfWork.Commit() > 0;
        }
    }
}
