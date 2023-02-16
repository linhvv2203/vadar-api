// <copyright file="PolicyService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Policy Service Class.
    /// </summary>
    public class PolicyService : EntityService<Policy>, IPolicyService
    {
        private readonly IPolicyUnitOfWork policyUnitOfWork;
        private readonly IMapper mapper;

        /// <summary>
        /// Initialises a new instance of the <see cref="PolicyService"/> class.
        /// </summary>
        /// <param name="policyUnitOfWork">Policy Unit Of Work.</param>
        /// <param name="mapper">mapper.</param>
        public PolicyService(
             IPolicyUnitOfWork policyUnitOfWork,
             IMapper mapper)
            : base(policyUnitOfWork, policyUnitOfWork.PolicyRepository)
        {
            this.policyUnitOfWork = policyUnitOfWork;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateWhiteIp(CreateWhiteIpDto createWhiteIpDto, string currentUserId)
        {
            if (createWhiteIpDto is null || createWhiteIpDto.WorkspaceId <= 0 || createWhiteIpDto.Ip is null)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, createWhiteIpDto.WorkspaceId, new[] { (long)EnPermissions.WhitelistIpSetting, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var listIps = new List<WhiteIp>();
            foreach (var item in createWhiteIpDto.Ip)
            {
                var whiteIp = (await this.policyUnitOfWork.WhiteListRepository.GetAll()).FirstOrDefault(x => x.Ip == item.Trim().ToLower() && x.WorkspaceId == createWhiteIpDto.WorkspaceId);
                if (whiteIp != null)
                {
                    throw new VadarException(ErrorCode.WhiteIpExists, nameof(ErrorCode.WhiteIpExists));
                }

                var ip = await this.policyUnitOfWork.WhiteListRepository.Add(new WhiteIp
                {
                    Ip = item.Trim().ToLower(),
                    Description = createWhiteIpDto.Description,
                    WorkspaceId = createWhiteIpDto.WorkspaceId,
                    CreatedDate = DateTime.Now,
                });
                listIps.Add(ip);
            }

            return await this.policyUnitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteWhiteIps(IEnumerable<IpDto> ipDtos, string currentUserId)
        {
            var enumerable = ipDtos.ToList();
            if (!enumerable.Any())
            {
                return true;
            }

            var ipList = enumerable.Select(s => s.Ip);
            var ips = (await this.policyUnitOfWork.WhiteListRepository.GetAll()).Where(w =>
                ipList.Any(ip => ip == w.Ip)).ToList();

            foreach (var el in ips)
            {
                if (!await this.ValidatePermission(currentUserId, el.WorkspaceId, new[] { (long)EnPermissions.WhitelistIpSetting, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
                {
                    throw new VadarException(ErrorCode.Forbidden);
                }

                await this.policyUnitOfWork.WhiteListRepository.Delete(el);
            }

            return await this.policyUnitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteWhiteIp(string ip, string currentUserId)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var whiteIp = (await this.policyUnitOfWork.WhiteListRepository.GetAll()).FirstOrDefault(x => x.Ip == ip);
            if (whiteIp == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(currentUserId, whiteIp.WorkspaceId, new[] { (long)EnPermissions.WhitelistIpSetting, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            await this.policyUnitOfWork.WhiteListRepository.Delete(whiteIp);
            return await this.policyUnitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PolicyViewDto>> GetPolicies(int workspaceId, string desciption, string currentUserId)
        {
            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var policies = await this.policyUnitOfWork.PolicyRepository.GetAll();
            if (!string.IsNullOrEmpty(desciption))
            {
                policies = policies.Where(x => x.Description.ToLower().Trim().Contains(desciption.ToLower().Trim()));
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.PolicyView, (long)EnPermissions.PolicySetting, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var result = await policies
                .OrderByDescending(o => o.Id)
                .Select(s => this.mapper.Map<PolicyViewDto>(s))
                .ToListAsync();

            var policiesByWorkspaceId = await (await this.policyUnitOfWork.WorkspacePolicyRepository.GetAll())
                .Where(x => x.WorkspaceId == workspaceId)
                .Select(s => s.Policy).ToListAsync();

            foreach (var t in result.Where(t => policiesByWorkspaceId.Any(x => x.Id == t.Id)))
            {
                t.IsEnable = true;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<PolicyAndWhiteListIpResultDto> GetPoliciesAndWhiteList(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            // Verify machine
            var workspaceHosts = from h in await this.policyUnitOfWork.HostRepository.GetAll()
                                 join hw in await this.policyUnitOfWork.WorkspaceHostRepository.GetAll() on h.Id equals hw.HostId
                                 join w in await this.policyUnitOfWork.WorkspaceRepository.GetAll() on hw.WorkspaceId equals w.Id
                                 where h.MachineId.Trim().ToUpper() == machineId.Trim().ToUpper()
                                 select hw;

            if (!workspaceHosts.Any())
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            // Get workspace policies.
            var policies = from w in await this.policyUnitOfWork.WorkspaceRepository.GetAll()
                           join wp in await this.policyUnitOfWork.WorkspacePolicyRepository.GetAll() on w.Id equals wp.WorkspaceId
                           join p in await this.policyUnitOfWork.PolicyRepository.GetAll() on wp.PolicyId equals p.Id
                           where w.Id == workspaceHosts.First().WorkspaceId
                           select p;

            var ips = from w in await this.policyUnitOfWork.WorkspaceRepository.GetAll()
                      join ip in await this.policyUnitOfWork.WhiteListRepository.GetAll() on w.Id equals ip.WorkspaceId
                      where w.Id == workspaceHosts.First().WorkspaceId
                      select ip;

            return new PolicyAndWhiteListIpResultDto
            {
                Policies = policies.Select(p => this.mapper.Map<PolicyDto>(p)).ToArray(),
                Ips = ips.Select(ip => this.mapper.Map<IpDto>(ip)).ToArray(),
            };
        }

        /// <inheritdoc/>
        public async Task<PolicyResultPagingDto> GetPoliciesPaging(PoliciesPagingRequestDto policiesPagingRequestDto, string currentUserId)
        {
            if (policiesPagingRequestDto is null || policiesPagingRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, policiesPagingRequestDto.WorkspaceId, new[] { (long)EnPermissions.PolicyView, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var policies = await this.policyUnitOfWork.PolicyRepository.GetAll();
            var result = await policies
                .OrderByDescending(o => o.Id)
                .Skip(policiesPagingRequestDto.PageSize * (policiesPagingRequestDto.PageIndex - 1))
                .Take(policiesPagingRequestDto.PageSize)
                .Select(s => this.mapper.Map<PolicyViewDto>(s))
                .ToListAsync();

            var policiesByWorkspaceId = await (await this.policyUnitOfWork.WorkspacePolicyRepository.GetAll())
                .Where(x => x.WorkspaceId == policiesPagingRequestDto.WorkspaceId)
                .Select(s => s.Policy).ToListAsync();

            foreach (var item in result.Where(item => policiesByWorkspaceId.Any(x => x.Id == item.Id)))
            {
                item.IsEnable = true;
            }

            return new PolicyResultPagingDto
            {
                Count = await policies.CountAsync(),
                Items = result,
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IpDto>> GetWhiteIps(int workspaceId, string ip, string currentUserId)
        {
            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.WhitelistIpView, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var whiteIps = (await this.policyUnitOfWork.WhiteListRepository.GetAll())
                .Where(x => x.WorkspaceId == workspaceId);
            if (!string.IsNullOrEmpty(ip))
            {
                whiteIps = whiteIps.Where(x => x.Ip.ToLower().Trim().Contains(ip.ToLower().Trim()));
            }

            var result = await whiteIps
                .OrderByDescending(o => o.Ip)
                .Select(s => this.mapper.Map<IpDto>(s))
                .ToListAsync();

            return result.ToArray();
        }

        /// <inheritdoc/>
        public async Task<WhiteIpResultPagingDto> GetWhiteIpPaging(WhiteIpPagingRequestDto whiteIpPagingRequestDto, string currentUserId)
        {
            if (whiteIpPagingRequestDto is null || whiteIpPagingRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, whiteIpPagingRequestDto.WorkspaceId, new[] { (long)EnPermissions.WhitelistIpView, (long)EnPermissions.WhitelistIpSetting, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var whiteIps = (await this.policyUnitOfWork.WhiteListRepository.GetAll())
               .Where(x => x.WorkspaceId == whiteIpPagingRequestDto.WorkspaceId);

            if (!string.IsNullOrEmpty(whiteIpPagingRequestDto.Ip))
            {
                whiteIps = whiteIps.Where(x => x.Ip.ToLower().Contains(whiteIpPagingRequestDto.Ip.ToLower().Trim()));
            }

            var result = await whiteIps
                .OrderByDescending(o => o.CreatedDate)
                .Skip(whiteIpPagingRequestDto.PageSize * (whiteIpPagingRequestDto.PageIndex - 1))
                .Take(whiteIpPagingRequestDto.PageSize)
                .Select(s => this.mapper.Map<WhiteIpViewDto>(s))
                .ToListAsync();

            return new WhiteIpResultPagingDto
            {
                Count = await whiteIps.CountAsync(),
                Items = result,
            };
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePolicies(UpdatePoliciesRequestDto updatePoliciesRequestDto, string currentUserId)
        {
            if (updatePoliciesRequestDto is null || updatePoliciesRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, updatePoliciesRequestDto.WorkspaceId, new[] { (long)EnPermissions.PolicySetting, (long)EnPermissions.FullPermission }, this.policyUnitOfWork.RolePermissionRepository, this.policyUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            // Delete Policies from Workspace Id.
            var policiesOfWorkspaceOld = (await this.policyUnitOfWork.WorkspacePolicyRepository.GetAll())
                .Where(x => x.WorkspaceId == updatePoliciesRequestDto.WorkspaceId);

            foreach (var item in policiesOfWorkspaceOld)
            {
                await this.policyUnitOfWork.WorkspacePolicyRepository.Delete(item);
            }

            if (!updatePoliciesRequestDto.PolicyIds.Any())
            {
                return await this.policyUnitOfWork.Commit() > 0;
            }

            // insert new policies.
            var policiesNew = await this.policyUnitOfWork.PolicyRepository.GetAll();

            foreach (var item in policiesNew)
            {
                await this.policyUnitOfWork.WorkspacePolicyRepository.Add(new WorkspacePolicy { WorkspaceId = updatePoliciesRequestDto.WorkspaceId, Policy = item });
            }

            return await this.policyUnitOfWork.Commit() > 0;
        }
    }
}
