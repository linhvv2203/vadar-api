// <copyright file="WorkspaceService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using static VADAR.Helpers.Const.Constants;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Workspace Service.
    /// </summary>
    public class WorkspaceService : EntityService<Workspace>, IWorkspaceService
    {
        private readonly IWorkspaceUnitOfWork unitOfWork;
        private readonly IWorkspaceHostUnitOfWork workspaceHostUnitOfWork;
        private readonly IUserUnitOfWork userUnitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IAESHelper aEsHelper;
        private readonly ICallApiZabbixHelper callApiZabbixHelper;
        private readonly ICallApiWazuhHelper callApiWazuhHelper;
        private readonly IGrafanaHelper grafanaHelper;
        private readonly IVadarAlertHelper vadarAlertHelper;
        private readonly IServiceBusHelper serviceBusHelper;
        private readonly IConfiguration configuration;
        private readonly IInviteWorkspaceRoleService inviteWorkspaceRoleService;
        private readonly IStringHelper stringHelper;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceService"/> class.
        /// </summary>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="groupUnitOfWork">groupUnitOfWork.</param>
        /// <param name="workspaceHostUnitOfWork">workspaceHostUnitOfWork.</param>
        /// <param name="userUnitOfWork">userUnitOfWork.</param>
        /// <param name="recaptchaHelper">recaptchaHelper.</param>
        /// <param name="userService">userService.</param>
        /// <param name="licenseUnitOfWork">licenseUnitOfWork.</param>
        /// <param name="mapper">mapper.</param>
        /// <param name="aEsHelper">aESHelper.</param>
        /// <param name="callApiZabbixHelper">callApiZabbixHelper.</param>
        /// <param name="callApiWazuhHelper">callApiWazuhHelper.</param>
        /// <param name="grafanaHelper">Grafana Helper.</param>
        /// <param name="vadarAlertHelper">vadarAlertHelper.</param>
        /// <param name="serviceBusHelper">service Bus Helper.</param>
        /// <param name="inviteWorkspaceRoleService">inviteWorkspaceRoleService.</param>
        /// <param name="configuration">configuration.</param>
        /// <param name="stringHelper">stringHelper.</param>
        public WorkspaceService(
            IWorkspaceUnitOfWork unitOfWork,
            IGroupUnitOfWork groupUnitOfWork,
            IWorkspaceHostUnitOfWork workspaceHostUnitOfWork,
            IUserUnitOfWork userUnitOfWork,
            IRecaptchaHelper recaptchaHelper,
            IUserService userService,
            ILicenseUnitOfWork licenseUnitOfWork,
            IMapper mapper,
            IAESHelper aEsHelper,
            ICallApiZabbixHelper callApiZabbixHelper,
            ICallApiWazuhHelper callApiWazuhHelper,
            IGrafanaHelper grafanaHelper,
            IVadarAlertHelper vadarAlertHelper,
            IServiceBusHelper serviceBusHelper,
            IInviteWorkspaceRoleService inviteWorkspaceRoleService,
            IConfiguration configuration,
            IStringHelper stringHelper)
            : base(unitOfWork, unitOfWork.WorkspaceRepository)
        {
            Guard.IsNotNull(unitOfWork, nameof(unitOfWork));
            Guard.IsNotNull(groupUnitOfWork, nameof(groupUnitOfWork));
            Guard.IsNotNull(workspaceHostUnitOfWork, nameof(workspaceHostUnitOfWork));
            Guard.IsNotNull(userUnitOfWork, nameof(userUnitOfWork));
            Guard.IsNotNull(recaptchaHelper, nameof(recaptchaHelper));
            Guard.IsNotNull(userService, nameof(userService));
            Guard.IsNotNull(mapper, nameof(mapper));
            Guard.IsNotNull(callApiZabbixHelper, nameof(callApiZabbixHelper));
            Guard.IsNotNull(callApiZabbixHelper, nameof(callApiZabbixHelper));
            Guard.IsNotNull(vadarAlertHelper, nameof(vadarAlertHelper));
            Guard.IsNotNull(serviceBusHelper, nameof(serviceBusHelper));
            Guard.IsNotNull(licenseUnitOfWork, nameof(licenseUnitOfWork));
            Guard.IsNotNull(inviteWorkspaceRoleService, nameof(inviteWorkspaceRoleService));
            Guard.IsNotNull(stringHelper, nameof(stringHelper));

            this.unitOfWork = unitOfWork;
            this.workspaceHostUnitOfWork = workspaceHostUnitOfWork;
            this.userUnitOfWork = userUnitOfWork;
            this.userService = userService;
            this.mapper = mapper;
            this.aEsHelper = aEsHelper;
            this.callApiZabbixHelper = callApiZabbixHelper;
            this.callApiWazuhHelper = callApiWazuhHelper;
            this.grafanaHelper = grafanaHelper;
            this.vadarAlertHelper = vadarAlertHelper;
            this.serviceBusHelper = serviceBusHelper;
            this.configuration = configuration;
            this.inviteWorkspaceRoleService = inviteWorkspaceRoleService;
            this.stringHelper = stringHelper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WorkspaceDto>> GetAllWorkspaceByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var permissions = await this.GetPermissions(
                userId,
                null,
                this.unitOfWork.RolePermissionRepository,
                this.unitOfWork.WorkspaceRolePermissionRepository);
            if (permissions != null && permissions.Any(p => p == (int)EnPermissions.AllDashboardsView || p == (int)EnPermissions.FullPermission))
            {
                return (await this.unitOfWork.WorkspaceRepository.GetAll()).Include(w => w.License).Select(wr => this.mapper.Map<WorkspaceDto>(wr));
            }

            return (await this.unitOfWork.WorkspaceRoleRepository.GetAll())
                .Include(wr => wr.Workspace)
                .ThenInclude(w => w.License)
                .Where(wr => wr.WorkspaceRoleUsers.Any(ru => ru.UserId == userId))
                .Select(wr => this.mapper.Map<WorkspaceDto>(wr.Workspace));
        }

        /// <inheritdoc/>
        public async Task<bool> AutoCreateWorkspace(RegistrationDto registrationDto)
        {
            if (registrationDto == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(ErrorCode.ArgumentNull));
            }

            var accessKey = this.configuration["AccessKey"];
            if (string.IsNullOrWhiteSpace(registrationDto.AccessKey)
               || string.IsNullOrWhiteSpace(accessKey)
               || accessKey.Trim() != registrationDto.AccessKey.Trim())
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var userExist = (await this.userUnitOfWork.UserRepository.GetAll())
                .FirstOrDefault(x => x.Email == registrationDto.Email);
            string userId;
            var mailInvite = new InviteWorkspaceRequestDto();

            if (userExist != null)
            {
                var isWpAlreadyExist = (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll()).Any(x => x.UserId == userExist.Id);
                var isAlreadyJoinedAWp = (await this.unitOfWork.WorkspaceRoleRepository.GetAll()).Any(x => x.Name == "Admin" && x.CreatedById == registrationDto.UserId);
                if (isWpAlreadyExist && isAlreadyJoinedAWp)
                {
                    throw new VadarException(ErrorCode.UserHasExistedWorkspace, nameof(ErrorCode.UserHasExistedWorkspace));
                }

                // user da co tk nhung chua co workspace.
                mailInvite.UserFirstRegister = (int)EnUserRegister.Exist;
                userId = userExist.Id;
            }
            else
            {
                // user dky lan dau.
                mailInvite.UserFirstRegister = (int)EnUserRegister.NotExist;
                var user = new UserDto
                {
                    UserName = registrationDto.UserName,
                    Email = registrationDto.Email,
                    CountryId = 237,
                    IsProfileUpdated = false,
                    Status = (int)EnUserStatus.Pending,
                    JoinDate = DateTime.Now,
                    Id = registrationDto.UserId,
                };
                var userDb = await this.userService.AddUserIfNotExist(user);
                userId = userDb.Id;
            }

            if (registrationDto.ExistUser)
            {
                mailInvite.UserFirstRegister = (int)EnUserRegister.Exist;
            }

            var specialEmails = new[] { "googlecom", "gmailcom", "yahoocom", "hotmailcom" };
            var isSpecialEmail = specialEmails.Any(s => !string.IsNullOrWhiteSpace(registrationDto.CompanyName)
                                                        && registrationDto.CompanyName.Trim().ToLower().Contains(s));

            var companyName = this.stringHelper.RemoveVietnameseTone(registrationDto.CompanyName);
            if (isSpecialEmail)
            {
                companyName += "-" + this.stringHelper.GenerateRandomString();
            }

            var workspace = new WorkspaceDto
            {
                Name = companyName.Trim().Replace(" ", "_").ToUpper(),
                HostLimit = 50,
                EndDate = DateTime.UtcNow.AddDays(14),
                Status = (int)EnLicenseStatus.Active,
                CreatedById = userId,
                CreatedDate = DateTime.UtcNow,
            };

            if (!(await this.unitOfWork.WorkspaceRepository.GetAll())
                .Any(x => x.Name.ToUpper() == workspace.Name && x.CreatedById == workspace.CreatedById))
            {
                await this.AddWorkspace(workspace, false);
            }

            var worksp = (await this.unitOfWork.WorkspaceRepository.GetAll())
                .Include(i => i.WorkspaceRoles).FirstOrDefault(x => x.Name == workspace.Name);
            var roles = worksp?.WorkspaceRoles.Where(s => s.Name == RoleNameSystem.Admin).Select(s => s.Id).ToArray();

            mailInvite.Emails = new[] { registrationDto.Email };
            mailInvite.WorkspaceId = worksp?.Id ?? 0;
            mailInvite.Language = registrationDto.Language.ToLower() == "vi" ? LanguageCodeClient.Vietnamese : LanguageCodeClient.EnglishUs;
            mailInvite.WorkspaceRoles = roles;

            var invitation = (await this.unitOfWork.InviteWorkspaceRoleRepository.GetAll())
                .FirstOrDefault(x => roles.Any(r => r == x.WorkspaceRoleId) && x.InviteTo == registrationDto.Email);
            if (invitation != null && invitation.Status == (int)EnInviteWorkspaceRoleStatus.Pending)
            {
                throw new VadarException(ErrorCode.EmailExistInvite, nameof(ErrorCode.EmailExistInvite));
            }

            await this.inviteWorkspaceRoleService.CreateInviteForWorkspace(mailInvite, userId, isAuth: false);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> AddWorkspace(WorkspaceDto workspaceDto, bool isSupperAdmin = true)
        {
            this.ValidateWorkspaceDto(workspaceDto);

            var workspaceName = workspaceDto.Name.Trim().ToUpper();
            var workspace = (await this.unitOfWork.WorkspaceRepository.GetAll()).FirstOrDefault(g => g.Name == workspaceDto.Name.Trim());
            if (workspace != null)
            {
                return await this.unitOfWork.Commit() > 0;
            }

            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var token = timestamp + workspaceDto.Name;
            workspaceDto.TokenWorkspace = this.aEsHelper.GetMd5Hash(token);

            var workspacesWazuh = await this.callApiWazuhHelper.GetWorkspaceDetail(workspaceName);
            if (workspacesWazuh != null && !string.IsNullOrEmpty(workspacesWazuh.Name))
            {
                workspaceDto.WazuhRef = workspaceName;
            }
            else
            {
                var resultWazuh = await this.callApiWazuhHelper.CreateAWorkspace(workspaceName);
                var workspaceWazuh = JsonConvert.DeserializeObject<dynamic>(resultWazuh);
                var workspaceIdWazuh = string.Empty;
                if (workspaceWazuh?.error == 0)
                {
                    workspaceIdWazuh = workspaceName;
                }

                workspaceDto.WazuhRef = workspaceIdWazuh;
            }

            var workspaceZabbix = await this.callApiZabbixHelper.FindWorkspaceByName(workspaceName);
            if (workspaceZabbix != null && !string.IsNullOrEmpty(workspaceZabbix.Name))
            {
                workspaceDto.ZabbixRef = workspaceZabbix.Id.ToString();
            }
            else
            {
                var resultZabbix = await this.callApiZabbixHelper.AddWorkspace(workspaceName);
                var workspacesZabbix = JsonConvert.DeserializeObject<dynamic>(resultZabbix);
                string workspaceIdZabbix = workspacesZabbix?.result?.groupids[0];
                workspaceDto.ZabbixRef = workspaceIdZabbix;
            }

            var newWorkspace = this.mapper.Map<Workspace>(workspaceDto);
            newWorkspace = this.SetDefaultWorkSpacePermission(newWorkspace, workspaceDto.CreatedById, isSupperAdmin);
            newWorkspace = this.SetDefaultWorkspaceSettingNotification(newWorkspace);

            // Create new Grafana Folder.
            var folder = await this.grafanaHelper.CreateNewFolder(workspaceName);

            // Create new Grafana Dashboard.
            if (folder != null && folder.Id > 0)
            {
                newWorkspace.GrafanaFolderUID = folder.Uid;

                var dashboard = await this.grafanaHelper.ImportInventoryDashboard(folder.Id, workspaceName + "_Inventory_Dashboard", workspaceDto.WazuhRef);
                if (dashboard != null && dashboard.DashboardId > 0)
                {
                    newWorkspace.GrafanaInventoryDashboardId = dashboard.DashboardId;
                    newWorkspace.GrafanaInventoryDashboardUrl = dashboard.ImportedUrl;
                }

                dashboard = await this.grafanaHelper.ImportPerformanceDashboard(folder.Id, workspaceName + "_Performance_Dashboard", workspaceDto.WazuhRef);
                if (dashboard != null && dashboard.DashboardId > 0)
                {
                    newWorkspace.GrafanaPerformanceDashboardId = dashboard.DashboardId;
                    newWorkspace.GrafanaPerformanceDashboardUrl = dashboard.ImportedUrl;
                }

                dashboard = await this.grafanaHelper.ImportSecurityDashboard(folder.Id, workspaceName + "_Security_Dashboard", workspaceDto.WazuhRef);
                if (dashboard != null && dashboard.DashboardId > 0)
                {
                    newWorkspace.GrafanaSecurityDashboardId = dashboard.DashboardId;
                    newWorkspace.GrafanaSecurityDashboardUrl = dashboard.ImportedUrl;
                }

                var grafanaOrg = await this.grafanaHelper.GetAdminAccountDefaultOrganization();

                if (grafanaOrg != null && grafanaOrg.Id > 0)
                {
                    newWorkspace.GrafanaOrgId = grafanaOrg.Id;
                }
            }

            // Build link download agent.
            var agentOs = this.GetDefaultAgentOs();
            var listAgentInstalls = new List<AgentInstallDto>();
            var buildAgentForWorkspaceDto = new BuildAgentForWorkspaceDto
            {
                Name = workspaceName,
                Token = newWorkspace.TokenWorkspace,
                IsProd = this.configuration.GetSection("Production").Get<bool>(),
            };

            foreach (var os in agentOs)
            {
                buildAgentForWorkspaceDto.Os = os.Name;
                switch (buildAgentForWorkspaceDto.Os)
                {
                    case Os.Ubuntu:
                        buildAgentForWorkspaceDto.Folders = $"agents/{Os.Ubuntu.ToLower()}/{buildAgentForWorkspaceDto.Token}/";
                        listAgentInstalls = this.vadarAlertHelper.BuildAgentUbuntuForWorkspace(buildAgentForWorkspaceDto);

                        // Send to queue.
                        await this.serviceBusHelper.SendMessage(buildAgentForWorkspaceDto);
                        break;
                    case Os.Centos:
                        buildAgentForWorkspaceDto.Folders = $"agents/{Os.Centos.ToLower()}/{buildAgentForWorkspaceDto.Token}/";
                        listAgentInstalls = this.vadarAlertHelper.BuildAgentCentosForWorkspaceV2(buildAgentForWorkspaceDto);

                        // Send to queue.
                        await this.serviceBusHelper.SendMessage(buildAgentForWorkspaceDto);
                        break;
                    case Os.Macos:
                        buildAgentForWorkspaceDto.Folders = $"agents/{Os.Macos.ToLower()}/{buildAgentForWorkspaceDto.Token}/";
                        listAgentInstalls = this.vadarAlertHelper.BuildAgentMacForWorkspace(buildAgentForWorkspaceDto);

                        // Send to queue.
                        await this.serviceBusHelper.SendMessage(buildAgentForWorkspaceDto);
                        break;
                    case Os.Window:
                        buildAgentForWorkspaceDto.Folders = $"agents/{Os.Window.ToLower()}/{buildAgentForWorkspaceDto.Token}/";
                        listAgentInstalls = this.vadarAlertHelper.BuildAgentWindowForWorkspace(buildAgentForWorkspaceDto);

                        // Send to queue.
                        await this.serviceBusHelper.SendMessage(buildAgentForWorkspaceDto);
                        break;
                }

                os.AgentInstalls = this.mapper.Map<List<AgentInstall>>(listAgentInstalls);
                listAgentInstalls.Clear();
            }

            newWorkspace.AgentOs = agentOs;

            newWorkspace.License = new License
            {
                Status = workspaceDto.Status,
                HostLimit = workspaceDto.HostLimit,
                StartDate = DateTime.Now,
                EndDate = workspaceDto.EndDate,
                CreatedDate = DateTime.UtcNow,
                CreatedById = workspaceDto.CreatedById,
            };

            newWorkspace.CreatedById = !string.IsNullOrWhiteSpace(workspaceDto.CreatedById)
                ? workspaceDto.CreatedById
                : newWorkspace.CreatedById;
            newWorkspace.CreatedDate = DateTime.UtcNow;

            await this.unitOfWork.WorkspaceRepository.Add(newWorkspace);

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteWorkspace(int id)
        {
            var workspace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(id);

            var hosts = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.GetAll())
                .Count(x => x.WorkspaceId == id);

            if (hosts > 0)
            {
                throw new VadarException(ErrorCode.PleaseDeleteHost, nameof(ErrorCode.PleaseDeleteHost));
            }

            if (workspace == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            var workspaceRoles = (await this.unitOfWork.WorkspaceRoleRepository.GetAll())
                .Where(wpr => wpr.WorkspaceId == id).Include(wpr => wpr.WorkspaceRolePermissions)
                .Include(wpr => wpr.WorkspaceRoleUsers);
            foreach (var wr in workspaceRoles)
            {
                if (wr.WorkspaceRolePermissions != null)
                {
                    foreach (var p in wr.WorkspaceRolePermissions)
                    {
                        await this.unitOfWork.WorkspaceRolePermissionRepository.Delete(p);
                    }
                }

                if (wr.WorkspaceRoleUsers != null)
                {
                    foreach (var u in wr.WorkspaceRoleUsers)
                    {
                        await this.unitOfWork.WorkspaceRoleUserRepository.Delete(u);
                    }
                }

                await this.unitOfWork.WorkspaceRoleRepository.Delete(wr);
            }

            await this.unitOfWork.WorkspaceRepository.Delete(workspace);

            if (workspace.WazuhRef != null)
            {
                await this.callApiWazuhHelper.RemoveAGroup(workspace.WazuhRef);
            }

            if (workspace.ZabbixRef != null)
            {
                var listGroupZabbix = new List<int>
                {
                    int.Parse(workspace.ZabbixRef),
                };
                await this.callApiZabbixHelper.DeleteGroup(listGroupZabbix);
            }

            // Delete Grafana folder
            await this.grafanaHelper.DeleteFolder(workspace.GrafanaFolderUID);

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<WorkspaceViewModelDto> GetWorkspaceById(int id, string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId) || id <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var permissionIds = Enum.GetValues(typeof(EnPermissions)).Cast<int>().Select(x => (long)x).ToArray();
            if (!await this.ValidatePermission(currentUserId, id, permissionIds, this.workspaceHostUnitOfWork.RolePermissionRepository, this.workspaceHostUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var host = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.GetAll()).Count(h => h.WorkspaceId == id);
            var result = (await this.unitOfWork.WorkspaceRepository.GetAll())
                .Include(i => i.License)
                .Where(x => x.Id == id)
                .Select(w => new WorkspaceViewModelDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    HostLimit = w.License != null ? w.License.HostLimit : 0,
                    HostActual = host,
                    EndDate = w.License != null ? w.License.EndDate : null,
                    Status = w.License != null ? w.License.Status : (int)EnLicenseStatus.InActive,
                    IsCreatedOrganisation = string.IsNullOrEmpty(w.OrgId) ? false : true,
                })
                .FirstOrDefault();

            return result;
        }

        /// <inheritdoc/>
        public async Task<WorkspaceResultPagingDto> GetAllWorkspace(WorkspacePagingRequestDto workspaceRequestDto)
        {
            if (workspaceRequestDto == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(workspaceRequestDto));
            }

            var query = await this.unitOfWork.WorkspaceRepository.GetAll();

            if (!string.IsNullOrEmpty(workspaceRequestDto.WorkspaceName))
            {
                query = query.Where(q => q.Name.Trim().ToLower().Contains(workspaceRequestDto.WorkspaceName.Trim().ToLower()));
            }

            var workspaces = query.Include(x => x.License)
                .OrderByDescending(o => o.Id)
                .Skip(workspaceRequestDto.PageSize * (workspaceRequestDto.PageIndex - 1))
                .Take(workspaceRequestDto.PageSize).Select(w => new WorkspaceViewModelDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    HostLimit = w.License.HostLimit,
                    EndDate = w.License.EndDate,
                    Status = w.License != null ? w.License.Status : (int)EnLicenseStatus.InActive,
                    IsCreatedOrganisation = string.IsNullOrEmpty(w.OrgId) ? false : true,
                });

            return new WorkspaceResultPagingDto
            {
                Count = query.Count(),
                Items = workspaces,
            };
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateWorkspace(WorkspaceDto workspaceDto)
        {
            this.ValidateWorkspaceDto(workspaceDto);

            var workspace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(workspaceDto.Id);
            if (workspace == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            workspace.UpdateById = workspaceDto.UpdateById;
            workspace.UpdatedDate = workspaceDto.UpdatedDate;

            if (workspace.License != null)
            {
                workspace.License.HostLimit = workspaceDto.HostLimit;
                workspace.License.EndDate = workspaceDto.EndDate;
                workspace.License.Status = workspaceDto.Status;
                workspace.License.UpdateById = workspaceDto.UpdateById;
                workspace.License.UpdatedDate = workspaceDto.UpdatedDate;
            }
            else
            {
                workspace.License = new License
                {
                    HostLimit = workspaceDto.HostLimit,
                    EndDate = workspaceDto.EndDate,
                    Status = workspaceDto.Status,
                    UpdateById = workspaceDto.UpdateById,
                    UpdatedDate = workspaceDto.UpdatedDate,
                };
            }

            await this.unitOfWork.WorkspaceRepository.Edit(workspace);
            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckLicensePreInstall(string tokenWorkspace)
        {
            if (tokenWorkspace == null)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(ErrorCode.ArgumentNull));
            }

            var workspace = (await this.unitOfWork.WorkspaceRepository.GetAll())
                .Include(w => w.License)
                .FirstOrDefault(x => x.TokenWorkspace == tokenWorkspace);

            if (workspace == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            var totalHostWorkspace = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.GetAll()).Count(x => x.WorkspaceId == workspace.Id);
            return workspace.License != null && workspace.License.Status == (int)EnLicenseStatus.Active
                                             && workspace.License.EndDate > DateTime.Now && workspace.License.HostLimit > totalHostWorkspace;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteByListId(int[] workspaceIds)
        {
            if (workspaceIds is null || workspaceIds.Length <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNullOrEmpty, nameof(ErrorCode.ArgumentNullOrEmpty));
            }

            var workspaces = (await this.unitOfWork.WorkspaceRepository.GetAll()).Where(x => workspaceIds.Any(w => w == x.Id));
            foreach (var item in workspaceIds)
            {
                if ((await this.workspaceHostUnitOfWork.WorkspaceHostRepository.GetAll())
                    .Any(x => x.WorkspaceId == item))
                {
                    throw new VadarException(ErrorCode.PleaseDeleteHost, nameof(ErrorCode.PleaseDeleteHost));
                }
            }

            foreach (var item in workspaces)
            {
                // Delete Grafana folder
                await this.grafanaHelper.DeleteFolder(item.GrafanaFolderUID);

                await this.unitOfWork.WorkspaceRepository.Delete(item);
            }

            return await this.unitOfWork.Commit() > 0;
        }

        private Workspace SetDefaultWorkSpacePermission(Workspace workspace, string userId, bool isSupperAdmin)
        {
            var readerPermission = new List<WorkspaceRolePermission>
            {
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.HostView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.GroupView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.DashboardView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.LogsView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.EventsView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.EmailNotificationView,
                },
            };

            var editorPermission = new List<WorkspaceRolePermission>
            {
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.PolicySetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.EmailNotificationSetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.WhitelistIpSetting,
                },
            };

            var adminPermission = new List<WorkspaceRolePermission>
            {
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.PolicySetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.EmailNotificationSetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.WhitelistIpSetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.HostView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.GroupView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.DashboardView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.LogsView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.EventsView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.EmailNotificationView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.HostSetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.GroupSetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.WorkspacePermissionView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.WorkspacePermissionSetting,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.PolicyView,
                },
                new WorkspaceRolePermission
                {
                    PermissionId = (int)EnPermissions.WhitelistIpView,
                },
            };

            workspace.WorkspaceRoles = new List<WorkspaceRole>
            {
                new WorkspaceRole
                {
                    Name = RoleNameSystem.Editor,
                    CreatedById = userId,
                    CreatedDate = DateTime.UtcNow,
                    WorkspaceRolePermissions = editorPermission,
                },
                new WorkspaceRole
                {
                    Name = RoleNameSystem.Reader,
                    CreatedById = userId,
                    CreatedDate = DateTime.UtcNow,
                    WorkspaceRolePermissions = readerPermission,
                },
            };

            var workspaceRole = new WorkspaceRole
            {
                Name = RoleNameSystem.Admin,
                CreatedById = userId,
                CreatedDate = DateTime.UtcNow,
                WorkspaceRolePermissions = adminPermission,
            };

            if (isSupperAdmin)
            {
                var adminRoleId = Guid.NewGuid();
                workspaceRole.Id = adminRoleId;
                workspaceRole.WorkspaceRoleUsers = new List<WorkspaceRoleUser>
                {
                        new WorkspaceRoleUser
                        {
                            UserId = userId,
                            WorkspaceRoleId = adminRoleId,
                        },
                };
            }

            workspace.WorkspaceRoles.Add(workspaceRole);
            return workspace;
        }

        private Workspace SetDefaultWorkspaceSettingNotification(Workspace workspace)
        {
            workspace.NotificationSettings = new List<NotificationSetting>
            {
                new NotificationSetting
                {
                    Name = NotificationChannels.Security,
                    Activate = true,
                    NotificationSettingConditions = new List<NotificationSettingCondition>
                    {
                        new NotificationSettingCondition
                        {
                            Condition = EnConditionType.Below,
                            Value = 15,
                        },
                        new NotificationSettingCondition
                        {
                            Condition = EnConditionType.Above,
                            Value = 10,
                        },
                    },
                },
                new NotificationSetting
                {
                    Name = NotificationChannels.Performance,
                    Activate = true,
                },
            };

            return workspace;
        }

        private void ValidateWorkspaceDto(WorkspaceDto workspaceDto)
        {
            if (workspaceDto == null || string.IsNullOrWhiteSpace(workspaceDto.Name))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(workspaceDto));
            }

            if (workspaceDto.Name.Length > 63)
            {
                throw new VadarException(ErrorCode.WorkspaceNameIsTooLong, nameof(ErrorCode.WorkspaceNameIsTooLong));
            }

            if (workspaceDto.HostLimit > 10000)
            {
                throw new VadarException(ErrorCode.HostLimitIsTooLarge, nameof(ErrorCode.HostLimitIsTooLarge));
            }

            if (workspaceDto.EndDate == null)
            {
                throw new VadarException(ErrorCode.ExpiredTimeNull, nameof(ErrorCode.ExpiredTimeNull));
            }

            if (workspaceDto.HostLimit <= 0)
            {
                throw new VadarException(ErrorCode.HostLimitInvalid, nameof(ErrorCode.HostLimitInvalid));
            }

            switch (workspaceDto.Status)
            {
                case (int)EnLicenseStatus.Active when workspaceDto.EndDate.Value.Date <= DateTime.Now.Date:
                    throw new VadarException(ErrorCode.EndDateBiggerCurrentTime);
                case (int)EnLicenseStatus.ExpiredDate when workspaceDto.EndDate.Value.Date > DateTime.Now.Date:
                    throw new VadarException(ErrorCode.EndDateLessCurrentTime);
            }
        }
    }
}
