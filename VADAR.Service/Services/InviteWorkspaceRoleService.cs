// <copyright file="InviteWorkspaceRoleService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.DTO.EmailTemplateViewModel;
using VADAR.Exceptions;
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
    /// InviteWorkspaceRole Service.
    /// </summary>
    public class InviteWorkspaceRoleService : EntityService<InviteWorkspaceRole>, IInviteWorkspaceRoleService
    {
        private readonly IInviteWorkspaceRoleUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;
        private readonly IRazorViewHelper razorViewHelper;
        private readonly IGrafanaHelper grafanaHelper;
        private readonly IStringHelper stringHelper;
        private readonly IIdentityServerHelper identityServerHelper;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly ITicketService ticketService;

        /// <summary>
        /// Initialises a new instance of the <see cref="InviteWorkspaceRoleService"/> class.
        /// </summary>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="emailSender">email Sender.</param>
        /// <param name="razorViewHelper">razor view helper.</param>
        /// <param name="grafanaHelper">Grafana Helper.</param>
        /// <param name="stringHelper">stringHelper.</param>
        /// <param name="identityServerHelper">identityServerHelper.</param>
        /// <param name="userService">userService.</param>
        /// <param name="ticketService">ticketService.</param>
        /// <param name="mapper">mapper.</param>
        public InviteWorkspaceRoleService(
            IInviteWorkspaceRoleUnitOfWork unitOfWork,
            IEmailSender emailSender,
            IRazorViewHelper razorViewHelper,
            IGrafanaHelper grafanaHelper,
            IStringHelper stringHelper,
            IIdentityServerHelper identityServerHelper,
            IUserService userService,
            ITicketService ticketService,
            IMapper mapper)
            : base(unitOfWork, unitOfWork.InviteWorkspaceRoleRepository)
        {
            this.unitOfWork = unitOfWork;
            this.emailSender = emailSender;
            this.razorViewHelper = razorViewHelper;
            this.grafanaHelper = grafanaHelper;
            this.stringHelper = stringHelper;
            this.identityServerHelper = identityServerHelper;
            this.userService = userService;
            this.mapper = mapper;
            this.ticketService = ticketService;
        }

        /// <inheritdoc/>
        public async Task<InviteWorkspaceRequestDto> CreateInviteForWorkspace(InviteWorkspaceRequestDto inviteWorkspaceRequestDto, string currentUserId, bool isAuth = true)
        {
            if (inviteWorkspaceRequestDto is null || inviteWorkspaceRequestDto.WorkspaceId <= 0
                || inviteWorkspaceRequestDto.Emails.Length <= 0 || inviteWorkspaceRequestDto.WorkspaceRoles.Length <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(ErrorCode.ArgumentNull));
            }

            if (isAuth && !await this.ValidatePermission(currentUserId, inviteWorkspaceRequestDto.WorkspaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var workspaceRolePermissions = await this.unitOfWork.WorkspaceRolePermissionRepository.GetAll();

            if (inviteWorkspaceRequestDto.WorkspaceRoles.Any(item => !workspaceRolePermissions.Any(x => x.WorkspaceRoleId == item)))
            {
                throw new VadarException(ErrorCode.RolePermissionNull, nameof(ErrorCode.RolePermissionNull));
            }

            // validate list email
            if (inviteWorkspaceRequestDto.Emails.Where(email => !string.IsNullOrEmpty(email)).Any(email => !this.stringHelper.IsValidEmail(email)))
            {
                throw new VadarException(ErrorCode.EmailInvalid);
            }

            var listInviteInWorkspace = (await this.unitOfWork.InviteWorkspaceRoleRepository.GetAll())
                .Where(x => x.WorkspaceRole.WorkspaceId == inviteWorkspaceRequestDto.WorkspaceId).ToList();

            var listInviteIsAlreadyHasStatusCancel = new List<InviteWorkspaceRole>();
            foreach (var email in inviteWorkspaceRequestDto.Emails)
            {
                if (listInviteInWorkspace.Any(x => x.InviteTo == email && x.Status != (int)EnInviteWorkspaceRoleStatus.Cancel && x.Status != (int)EnInviteWorkspaceRoleStatus.Reject))
                {
                    throw new VadarException(ErrorCode.EmailExistInvite);
                }

                if (!await this.identityServerHelper.VerifyEmailExisting(email))
                {
                    throw new VadarException(ErrorCode.EmailNotCreatedOnIDserver, $"{nameof(email)} not created on IDserver");
                }

                var emailIsAlready = listInviteInWorkspace.FirstOrDefault(x => x.InviteTo == email);
                if (emailIsAlready != null)
                {
                    listInviteIsAlreadyHasStatusCancel.Add(emailIsAlready);
                }
            }

            var usersExisting = (await this.unitOfWork.UserRepository.GetAll())
                .Where(x => inviteWorkspaceRequestDto.Emails.Any(e => e == x.Email)).ToList();
            var workspaceRoles = (await this.unitOfWork.WorkspaceRoleRepository.GetAll())
                .Include(w => w.Workspace)
                .Where(x => inviteWorkspaceRequestDto.WorkspaceRoles.Any(w => w == x.Id)).ToList();

            var listInvite = new List<InviteWorkspaceRole>();
            var addInviteEmail = inviteWorkspaceRequestDto.Emails.Except(listInviteIsAlreadyHasStatusCancel.Select(x => x.InviteTo)).ToList();
            foreach (var email in addInviteEmail)
            {
                var user = usersExisting.FirstOrDefault(x => x.Email == email);
                var inviteWorkspaceNew = new InviteWorkspaceRole
                {
                    UserId = user?.Id,
                    InviteTo = user == null ? email : user.Email,
                    Status = (int)EnInviteWorkspaceRoleStatus.Pending,
                    CreatedDate = DateTime.Now,
                    CreatedById = currentUserId,
                    ExpriredDate = DateTime.Now.AddDays(7),
                };

                foreach (var item in workspaceRoles)
                {
                    inviteWorkspaceNew.WorkspaceRole = item;
                    var invite = await this.unitOfWork.InviteWorkspaceRoleRepository.Add(inviteWorkspaceNew);
                    listInvite.Add(invite);
                }
            }

            foreach (var inviteIsAlready in listInviteIsAlreadyHasStatusCancel)
            {
                foreach (var item in workspaceRoles)
                {
                    inviteIsAlready.WorkspaceRole = item;
                    inviteIsAlready.Status = (int)EnInviteWorkspaceRoleStatus.Pending;
                    await this.unitOfWork.InviteWorkspaceRoleRepository.Edit(inviteIsAlready);
                    listInvite.Add(inviteIsAlready);
                }
            }

            var userInvite = await this.unitOfWork.UserRepository.GetUserById(currentUserId);
            var result = new InviteWorkspaceRequestDto();
            if (await this.unitOfWork.Commit() <= 0 || !listInvite.Any())
            {
                return result;
            }

            {
                var workspaceRoleName = listInvite.FirstOrDefault()?.WorkspaceRole?.Name;
                var workspaceName = listInvite.FirstOrDefault()?.WorkspaceRole?.Workspace?.Name;
                var emailViewModel = new InviteUserToWorkspaceViewModel
                {
                    InviteCode = string.Join(",", listInvite.Select(x => x.Id)),
                    UserName = userInvite.UserName,
                    WorkspaceRoleName = workspaceRoleName,
                    WorkspaceName = workspaceName,
                };

                var isVietNameLanguage = inviteWorkspaceRequestDto.Language.ToLower() == LanguageCodeClient.Vietnamese.ToLower();
                string emailSubject;
                string templatePath;
                foreach (var item in listInvite)
                {
                    emailViewModel.InviteCode = item.Id.ToString();
                    switch (inviteWorkspaceRequestDto.UserFirstRegister)
                    {
                        case (int)EnUserRegister.NotExist:
                            // user dky lan dau.
                            templatePath = GetTemplatePath(isVietNameLanguage, true);
                            emailSubject = GetTitle(isVietNameLanguage, true);
                            break;
                        case (int)EnUserRegister.Exist:
                            // user da co tk nhung chua co workspace.
                            templatePath = GetTemplatePath(isVietNameLanguage);
                            emailSubject = GetTitle(isVietNameLanguage);
                            break;
                        default:
                            // thu moi tham gia
                            templatePath = GetTemplatePath(isVietNameLanguage);
                            emailSubject = GetTitle(isVietNameLanguage);
                            break;
                    }

                    await this.SendEmail(emailViewModel, item.InviteTo, emailSubject, templatePath);
                }

                result.Emails = listInvite.Select(x => x.InviteTo).ToArray();
                result.WorkspaceRoles = workspaceRoles.Select(x => x.Id).ToArray();
                return result;
            }

            static string GetTitle(bool isVnLang, bool isNewUser = false)
            {
                if (isNewUser)
                {
                    return isVnLang ? "Yêu cầu kích hoạt tài khoản giải pháp giám sát an toàn thông tin VSEC VADAR" : "Requires activation of VSEC VADAR security monitoring solution account";
                }

                return isVnLang ? "Lời mời tham gia Workspace trong giải pháp giám sát An toàn thông tin VSEC VADAR." : "Invitation to join the Workspace in security monitoring solution VSEC VADAR.";
            }

            static string GetTemplatePath(bool isVnLang, bool isNewUser = false)
            {
                if (isNewUser)
                {
                    return isVnLang ? "EmailTemplates/vi-vn/InviteUserToWorkspaceActive" : "EmailTemplates/en-us/InviteUserToWorkspace";
                }

                return isVnLang ? "EmailTemplates/vi-vn/InviteUserToWorkspace" : "EmailTemplates/en-us/InviteUserToWorkspace";
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AcceptRejectInvitation(
            string currentUserId,
            AcceptRejectInvitationDto acceptRejectInvitation)
        {
            if (acceptRejectInvitation == null || acceptRejectInvitation.InvitationId == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (acceptRejectInvitation.Status != (int)EnInviteWorkspaceRoleStatus.Accept &&
                acceptRejectInvitation.Status != (int)EnInviteWorkspaceRoleStatus.Reject)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            // Get workspace Id
            var invitation = (await this.unitOfWork.InviteWorkspaceRoleRepository.FindBy(i => i.Id == acceptRejectInvitation.InvitationId))
                .Include(i => i.WorkspaceRole)
                .ThenInclude(i => i.Workspace)
                .FirstOrDefault();

            if (invitation == null)
            {
                throw new VadarException(ErrorCode.InvalidInvitationId);
            }

            if (invitation.Status != (int)EnInviteWorkspaceRoleStatus.Pending)
            {
                throw new VadarException(ErrorCode.InvitationInvalid);
            }

            invitation.Status = acceptRejectInvitation.Status;
            await this.unitOfWork.InviteWorkspaceRoleRepository.Edit(invitation);

            // Set workspace role to user
            if (acceptRejectInvitation.Status != (int)EnInviteWorkspaceRoleStatus.Accept)
            {
                return await this.unitOfWork.Commit() > 0;
            }

            // Active account from Identity server.
            var receiver = await this.identityServerHelper.ActiveAccount(invitation.InviteTo);
            if (string.IsNullOrEmpty(receiver.Id) || string.IsNullOrEmpty(receiver.Email))
            {
                throw new VadarException(ErrorCode.InvitationInvalid);
            }

            var userCreated = await this.unitOfWork.UserRepository.GetUserById(receiver.Id);
            if (userCreated == null)
            {
                receiver.JoinDate = DateTime.UtcNow;
                receiver.CountryId = 237;
                userCreated = await this.unitOfWork.UserRepository.Add(this.mapper.Map<User>(receiver));
            }

            if (receiver != null && invitation.InviteTo != receiver.Email)
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var oldWorkspaceRole = (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll())
                .Include(wru => wru.WorkspaceRole)
                .ThenInclude(wr => wr.Workspace)
                .FirstOrDefault(wru =>
                wru.UserId == receiver.Id && wru.WorkspaceRoleId == invitation.WorkspaceRoleId);

            // Login the Hive.
            var ticketDto = new TicketDto()
            {
                Method = "POST",
                Index = "login",
                UseCookie = true,
                Services = "Ticket",
            };

            var loginResponse = await this.ticketService.Index(ticketDto);

            // Add user to the Hive.
            if (loginResponse.IsSuccessStatusCode)
            {
                ticketDto.Index = "v1/user";
                ticketDto.Query = JsonConvert.DeserializeObject<dynamic>("{\"login\": \"" + invitation.InviteTo + "\", \"name\":\"" + invitation.InviteTo + "\", \"organisation\":\"" + invitation.WorkspaceRole.Workspace.Name + "\", \"profile\":\"analyst\"}");
                await this.ticketService.Index(ticketDto);
            }

            // Assign grafana permission.
            // var user = await this.unitOfWork.UserRepository.GetUserById(currentUserId);
            await this.grafanaHelper.CreateGrafanaAccount(new GrafanaAccountDto
            {
                Password = Guid.NewGuid().ToString(),
                Email = receiver.Email,
                Name = string.IsNullOrEmpty(receiver.FullName) ? receiver.Email : receiver.FullName,
                Login = receiver.Email,
            });

            var workspace =
                (await this.unitOfWork.WorkspaceRepository.GetAll()).FirstOrDefault(w =>
                    w.WorkspaceRoles.Any(wpr => wpr.Id == invitation.WorkspaceRoleId));
            if (workspace != null && workspace.GrafanaInventoryDashboardId > 0)
            {
                await this.grafanaHelper.AssignPermissionToUserByEmail(receiver.Email, workspace.GrafanaInventoryDashboardId);
            }

            if (workspace != null && workspace.GrafanaPerformanceDashboardId > 0)
            {
                await this.grafanaHelper.AssignPermissionToUserByEmail(receiver.Email, workspace.GrafanaPerformanceDashboardId);
            }

            if (workspace != null && workspace.GrafanaSecurityDashboardId > 0)
            {
                await this.grafanaHelper.AssignPermissionToUserByEmail(receiver.Email, workspace.GrafanaSecurityDashboardId);
            }

            if (oldWorkspaceRole != null)
            {
                return await this.unitOfWork.Commit() > 0;
            }

            await this.unitOfWork.WorkspaceRoleUserRepository.Add(new WorkspaceRoleUser
            {
                UserId = userCreated.Id,
                WorkspaceRoleId = invitation.WorkspaceRoleId,
            });

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MembersByWorkspaceViewDto>> GetMembersByWorkspace(int workspaceId, string emailAddress, string currentUserId)
        {
            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(workspaceId));
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.WorkspacePermissionView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var inviteWorkspaceRoles = (await this.unitOfWork.WorkspaceRoleRepository.GetAll())
                .Include(i => i.WorkspaceRoleUsers)
                .Where(x => x.WorkspaceId.Equals(workspaceId))
                .Select(s => s.InviteWorkspaceRoles)
                .SelectMany(c => c);

            var result = inviteWorkspaceRoles
                .Where(x => x.Status == (int)EnInviteWorkspaceRoleStatus.Accept || x.Status == (int)EnInviteWorkspaceRoleStatus.Pending)
                .Select(s => new MembersByWorkspaceViewDto
                {
                    UserId = s.InvitedUser != null ? s.InvitedUser.Id : string.Empty,
                    UserName = s.InvitedUser != null ? s.InvitedUser.UserName : string.Empty,
                    InviteStatus = s.Status,
                    WorkspaceRoleId = this.unitOfWork.WorkspaceRoleUserRepository.GetAll().Result.Where(r => r.UserId.Equals(s.UserId)).FirstOrDefault(r => r.WorkspaceRole.WorkspaceId == workspaceId) != null
                    ? this.unitOfWork.WorkspaceRoleUserRepository.GetAll().Result.Where(r => r.UserId.Equals(s.UserId)).FirstOrDefault(r => r.WorkspaceRole.WorkspaceId == workspaceId).WorkspaceRoleId : s.WorkspaceRoleId,
                    InviteId = s.Id,
                    UserEmail = s.InvitedUser == null ? s.InviteTo : s.InvitedUser.Email,
                });

            if (!string.IsNullOrEmpty(emailAddress))
            {
                result = result.Where(x => x.UserEmail.Trim().Contains(emailAddress.ToLower().Trim()));
            }

            return await result.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> CancelInvitation(string currentUserId, Guid invitationId)
        {
            if (string.IsNullOrEmpty(currentUserId) || invitationId == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            // Get workspace Id
            var workspaceId =
                (await this.unitOfWork.InviteWorkspaceRoleRepository.GetAll()).Where(i => i.Id == invitationId).Select(i => i.WorkspaceRole.WorkspaceId).FirstOrDefault();

            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var invitation = (await this.unitOfWork.InviteWorkspaceRoleRepository.FindBy(i => i.Id == invitationId))
                .FirstOrDefault();

            if (invitation == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (invitation.Status != (int)EnInviteWorkspaceRoleStatus.Pending)
            {
                throw new VadarException(ErrorCode.InvitationInvalid);
            }

            invitation.Status = (int)EnInviteWorkspaceRoleStatus.Cancel;
            await this.unitOfWork.InviteWorkspaceRoleRepository.Edit(invitation);
            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteInvitation(string currentUserId, Guid invitationId, int workspaceId)
        {
            if (string.IsNullOrEmpty(currentUserId) || invitationId == Guid.Empty || workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var invitation = (await this.unitOfWork.InviteWorkspaceRoleRepository.GetAll())
                .FirstOrDefault(x => x.Id == invitationId && x.WorkspaceRole.WorkspaceId == workspaceId && x.Status == (int)EnInviteWorkspaceRoleStatus.Accept);
            if (invitation == null)
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            await this.unitOfWork.InviteWorkspaceRoleRepository.Delete(invitation);

            var workspaceRoleUsers = (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll())
                .Where(x => x.UserId == invitation.UserId && x.WorkspaceRole.WorkspaceId == workspaceId);

            foreach (var item in await workspaceRoleUsers.ToListAsync())
            {
                await this.unitOfWork.WorkspaceRoleUserRepository.Delete(item);
            }

            // remove user permissions from dashboard
            if (invitation.Status != (int)EnInviteWorkspaceRoleStatus.Accept)
            {
                return await this.unitOfWork.Commit() > 0;
            }

            var workspace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(workspaceId);

            if (workspace.GrafanaInventoryDashboardId > 0)
            {
                await this.grafanaHelper.RemoveDashboardPermission(invitation.InviteTo, workspace.GrafanaInventoryDashboardId);
            }

            if (workspace.GrafanaPerformanceDashboardId > 0)
            {
                await this.grafanaHelper.RemoveDashboardPermission(invitation.InviteTo, workspace.GrafanaPerformanceDashboardId);
            }

            if (workspace.GrafanaSecurityDashboardId > 0)
            {
                await this.grafanaHelper.RemoveDashboardPermission(invitation.InviteTo, workspace.GrafanaSecurityDashboardId);
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> ResendInvitation(string currentUserId, Guid invitationId, string language)
        {
            if (string.IsNullOrEmpty(currentUserId) || invitationId == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var invitation = (await this.unitOfWork.InviteWorkspaceRoleRepository.GetAll()).Include(i => i.WorkspaceRole)
                .ThenInclude(w => w.Workspace)
                .Include(u => u.InvitedUser).FirstOrDefault(x => x.Id == invitationId);
            if (invitation == null)
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            if (!await this.ValidatePermission(currentUserId, invitation.WorkspaceRole?.WorkspaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var ownerInvite = await this.unitOfWork.UserRepository.GetUserById(invitation.CreatedById);
            if (ownerInvite == null)
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            if (invitation.WorkspaceRole == null)
            {
                return false;
            }

            var emailViewModel = new InviteUserToWorkspaceViewModel
            {
                InviteCode = invitation.Id.ToString(),
                UserName = ownerInvite.UserName,
                WorkspaceRoleName = invitation.WorkspaceRole.Name,
                WorkspaceName = invitation.WorkspaceRole.Workspace.Name,
            };

            var isVietNameLanguage = language.ToLower() == "vn";
            var emailSubject = isVietNameLanguage ? "Lời mời tham gia Workspace trong giải pháp giám sát An toàn thông tin VSEC VADAR." : "Invitation to join the Workspace in security monitoring solution VSEC VADAR.";
            var emailContent = await this.razorViewHelper.RenderViewAsString(
                isVietNameLanguage ? "EmailTemplates/vi-vn/InviteUserToWorkspace" : "EmailTemplates/en-us/InviteUserToWorkspace", emailViewModel);
            var emailReceiver = invitation.InvitedUser != null ? invitation.InvitedUser.Email : invitation.InviteTo;
            if (string.IsNullOrEmpty(emailReceiver))
            {
                return false;
            }

            await this.emailSender.SendEmailAsync(emailReceiver, emailSubject, emailContent);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateWorkspaceRoleForUser(WorkspaceRoleUserUpdateRequestDto dto, string currentUserId)
        {
            if (dto is null || dto.WorkspaceRoles.Length <= 0
                || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workspaceRolePermissions = await this.unitOfWork.WorkspaceRolePermissionRepository.GetAll();

            if (dto.WorkspaceRoles.Any(item => !workspaceRolePermissions.Any(x => x.WorkspaceRoleId == item)))
            {
                throw new VadarException(ErrorCode.RolePermissionNull, nameof(ErrorCode.RolePermissionNull));
            }

            var user = (await this.unitOfWork.UserRepository.GetAll()).FirstOrDefault(x => x.Email.Trim().ToLower() == dto.Email.Trim().ToLower());
            if (user == null)
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var roleOfUsers = (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll())
                .Include(i => i.WorkspaceRole)
                .Where(x => x.UserId == user.Id);
            foreach (var item in dto.WorkspaceRoles)
            {
                var workspaceRole = (await this.unitOfWork.WorkspaceRoleRepository.GetAll()).FirstOrDefault(x => x.Id == item);
                var workspaceRoles = (await this.unitOfWork.WorkspaceRoleRepository.FindBy(x => x.WorkspaceId == workspaceRole.WorkspaceId)).ToList();
                foreach (var i in workspaceRoles)
                {
                    var roleUser = await roleOfUsers.Where(x => x.WorkspaceRoleId == i.Id).ToListAsync();
                    foreach (var items in roleUser)
                    {
                        if (!await this.ValidatePermission(currentUserId, items.WorkspaceRole.WorkspaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
                        {
                            throw new VadarException(ErrorCode.Forbidden);
                        }

                        await this.unitOfWork.WorkspaceRoleUserRepository.Delete(items);
                    }
                }
            }

            foreach (var item in dto.WorkspaceRoles)
            {
                await this.unitOfWork.WorkspaceRoleUserRepository.Add(new WorkspaceRoleUser
                {
                    User = user,
                    WorkspaceRoleId = item,
                });
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<string> VerifyInvitation(Guid invitationId, string currentUserId)
        {
            if (invitationId == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var invitation = await this.unitOfWork.InviteWorkspaceRoleRepository.GetInviteWorkspaceRoleById(invitationId);
            if (invitation is null)
            {
                throw new VadarException(ErrorCode.InvalidInvitationId);
            }

            if (invitation.Status == (int)EnInviteWorkspaceRoleStatus.Pending)
            {
                return invitation.InviteTo;
            }
            else
            {
                return string.Empty;
            }
        }

        private async Task SendEmail(InviteUserToWorkspaceViewModel emailViewModel, string emailAddress, string emailSubject, string templatePath)
        {
            var emailContent = await this.razorViewHelper.RenderViewAsString(templatePath, emailViewModel);
            await this.emailSender.SendEmailAsync(emailAddress, emailSubject, emailContent);
        }
    }
}
