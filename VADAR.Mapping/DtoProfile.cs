// <copyright file="DtoProfile.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using AutoMapper;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Model.Models;

namespace VADAR.Mapping
{
    /// <summary>
    /// Dto mapping profile class.
    /// </summary>
    public class DtoProfile : Profile
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DtoProfile"/> class.
        /// Initializes a new instance of the <see cref="DtoProfile"/> class.
        /// Contructor method.
        /// </summary>
        public DtoProfile()
        {
            this.CreateMap<Workspace, WorkspaceViewModelDto>();
            this.CreateMap<WorkspaceDto, Workspace>();
            this.CreateMap<Workspace, WorkspaceDto>()
                .ForMember(wp => wp.Status, m => m.MapFrom(u => (u.License != null) ? u.License.Status : (int)EnLicenseStatus.InActive))
                .ForMember(wp => wp.EndDate, m => m.MapFrom(u => (u.License != null) ? u.License.EndDate : null));
            this.CreateMap<Permission, PermissionDto>();
            this.CreateMap<PermissionDto, Permission>();
            this.CreateMap<User, UserDto>();
            this.CreateMap<UserDto, User>();
            this.CreateMap<Group, GroupViewModelDto>();
            this.CreateMap<GroupDto, Group>();
            this.CreateMap<WorkspaceRole, WorkspaceRoleDto>();
            this.CreateMap<WorkspaceRoleDto, WorkspaceRole>();
            this.CreateMap<Host, HostViewModelDto>();
            this.CreateMap<HostDto, Host>();
            this.CreateMap<WorkspaceHostDto, WorkspaceHost>();
            this.CreateMap<GroupHostDto, GroupHost>();
            this.CreateMap<PolicyDto, Policy>();
            this.CreateMap<Policy, PolicyDto>();
            this.CreateMap<PolicyViewDto, Policy>();
            this.CreateMap<Policy, PolicyViewDto>();
            this.CreateMap<IpDto, WhiteIp>();
            this.CreateMap<WhiteIp, IpDto>();
            this.CreateMap<WhiteIp, WhiteIpViewDto>();
            this.CreateMap<AgentOs, AgentOsDto>();
            this.CreateMap<AgentInstall, AgentInstallDto>();
            this.CreateMap<AgentInstallDto, AgentInstall>();
            this.CreateMap<LicenseDto, License>();
            this.CreateMap<License, LicenseDto>();
            this.CreateMap<UserClaimDto, UserClaim>();
            this.CreateMap<UserClaim, UserClaimDto>();
            this.CreateMap<NotiSettingConditionDto, NotificationSettingCondition>();
            this.CreateMap<NotificationSettingCondition, NotiSettingConditionDto>();
        }
    }
}
