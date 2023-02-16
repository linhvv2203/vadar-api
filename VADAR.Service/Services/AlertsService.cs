// <copyright file="AlertsService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Alert Service.
    /// </summary>
    public class AlertsService : EntityService<Workspace>, IAlertsService
    {
        private readonly IVadarAlertHelper vadarAlertHelper;
        private readonly IWorkspaceUnitOfWork workspaceUnitOfWork;
        private readonly IStringHelper stringHelper;
        private readonly IMapper mapper;
        private readonly INotificationSettingUnitOfWork notificationSettingUnitOfWork;
        private readonly INotificationSettingConditionUnitOfWork notificationSettingConditionUnitOfWork;
        private readonly IWorkerNotificationUnitOfWork workerNotificationUnitOfWork;
        private readonly ITelegramHelper telegramHelper;
        private readonly IWorkerNotificationService workerNotificationService;
        private readonly ILoggerHelper<AlertsService> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="AlertsService"/> class.
        /// </summary>
        /// <param name="vadarAlertHelper">vadarAlertHelper.</param>
        /// <param name="workspaceUnitOfWork">workspaceUnitOfWork.</param>
        /// <param name="stringHelper">stringHelper.</param>
        /// <param name="mapper">mapper.</param>
        /// <param name="notificationSettingUnitOfWork">notificationSettingUnitOfWork.</param>
        /// <param name="notificationSettingConditionUnitOfWork">notificationSettingConditionUnitOfWork.</param>
        /// <param name="workerNotificationUnitOfWork">workerNotificationUnitOfWork.</param>
        /// <param name="telegramHelper">telegramHelper.</param>
        /// <param name="workerNotificationService">workerNotificationService.</param>
        /// <param name="logger">logger.</param>
        public AlertsService(
            IVadarAlertHelper vadarAlertHelper,
            IWorkspaceUnitOfWork workspaceUnitOfWork,
            IStringHelper stringHelper,
            IMapper mapper,
            INotificationSettingUnitOfWork notificationSettingUnitOfWork,
            INotificationSettingConditionUnitOfWork notificationSettingConditionUnitOfWork,
            IWorkerNotificationUnitOfWork workerNotificationUnitOfWork,
            ITelegramHelper telegramHelper,
            IWorkerNotificationService workerNotificationService,
            ILoggerHelper<AlertsService> logger)
            : base(workspaceUnitOfWork, workspaceUnitOfWork.WorkspaceRepository)
        {
            this.vadarAlertHelper = vadarAlertHelper;
            this.workspaceUnitOfWork = workspaceUnitOfWork;
            this.stringHelper = stringHelper;
            this.mapper = mapper;
            this.notificationSettingUnitOfWork = notificationSettingUnitOfWork;
            this.notificationSettingConditionUnitOfWork = notificationSettingConditionUnitOfWork;
            this.workerNotificationUnitOfWork = workerNotificationUnitOfWork;
            this.telegramHelper = telegramHelper;
            this.workerNotificationService = workerNotificationService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> AddChannelsToAlerts(MultiChannelAlertsRequestDto alertsRequestDto, string currentUserId)
        {
            if (alertsRequestDto is null || alertsRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, alertsRequestDto.WorkspaceId, new[] { (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            // validate list email - slack - telegram - zalo - sms
            var notifications = new List<WorkspaceNotification>();
            foreach (var item in alertsRequestDto.WorkspaceNotifications)
            {
                // email
                if (item.Type == (int)EnNotificationType.Email)
                {
                    foreach (var email in item.Address)
                    {
                        if (string.IsNullOrEmpty(email))
                        {
                            continue;
                        }

                        notifications.Add(new WorkspaceNotification
                        {
                            Address = email,
                            Type = EnNotificationType.Email,
                        });
                        if (!this.stringHelper.IsValidEmail(email))
                        {
                            throw new VadarException(ErrorCode.EmailInvalid);
                        }
                    }
                }

                // slack
                if (item.Type == (int)EnNotificationType.Slack)
                {
                    foreach (var slack in item.Address)
                    {
                        if (string.IsNullOrEmpty(slack))
                        {
                            continue;
                        }

                        notifications.Add(new WorkspaceNotification
                        {
                            Address = slack,
                            Type = EnNotificationType.Slack,
                        });
                        if (!this.stringHelper.IsValidUrl(slack))
                        {
                            throw new VadarException(ErrorCode.SlackUrlInValid);
                        }
                    }
                }

                // telegram
                if (item.Type == (int)EnNotificationType.TeleGram)
                {
                    if (item.Address == null || !item.Address.Any())
                    {
                        await this.DeleteWorkspaceClaims(alertsRequestDto.WorkspaceId);
                    }

                    foreach (var telegram in item.Address)
                    {
                        this.logger.LogInfo($"tele address: {telegram}");
                        if (string.IsNullOrEmpty(telegram))
                        {
                            throw new VadarException(ErrorCode.TeleTokenInValid);
                        }

                        if (!this.stringHelper.IsValidTelegram(telegram))
                        {
                            throw new VadarException(ErrorCode.TeleTokenInValid);
                        }

                        var telegramsDb = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository.GetAll())
                            .Where(x => x.Address.Equals(telegram) && x.WorkspaceId == alertsRequestDto.WorkspaceId);
                        this.logger.LogInfo($"telegrams in Db: {JsonConvert.SerializeObject(telegramsDb)}");
                        if (telegramsDb == null || !telegramsDb.Any())
                        {
                            await this.DeleteWorkspaceClaims(alertsRequestDto.WorkspaceId);

                            // update chat id. Helpers.Const.Constants.WorkspaceClaims.ChatIdTelegram
                            var chatIdsFromTele = await this.telegramHelper.GetChatIds(telegram);
                            if (chatIdsFromTele != null && chatIdsFromTele.Any())
                            {
                                // add chatIds to db.
                                _ = await this.workerNotificationService.AddChatIdsToDatabase(chatIdsFromTele, alertsRequestDto.WorkspaceId);
                            }
                        }

                        notifications.Add(new WorkspaceNotification
                        {
                            Address = telegram,
                            Type = EnNotificationType.TeleGram,
                        });
                    }
                }

                // sms
                if (item.Type == (int)EnNotificationType.Sms)
                {
                    foreach (var sms in item.Address)
                    {
                        if (string.IsNullOrEmpty(sms))
                        {
                            continue;
                        }

                        notifications.Add(new WorkspaceNotification
                        {
                            Address = sms,
                            Type = EnNotificationType.Sms,
                        });
                        if (!this.stringHelper.IsValidPhoneNumber(sms))
                        {
                            throw new VadarException(ErrorCode.PhoneNumberInValid);
                        }
                    }
                }

                // zalo
                if (item.Type == (int)EnNotificationType.Zalo)
                {
                    foreach (var zalo in item.Address)
                    {
                        if (string.IsNullOrEmpty(zalo))
                        {
                            continue;
                        }

                        notifications.Add(new WorkspaceNotification
                        {
                            Address = zalo,
                            Type = EnNotificationType.Zalo,
                        });
                        if (!this.stringHelper.IsValidPhoneNumber(zalo))
                        {
                            throw new VadarException(ErrorCode.PhoneNumberInValid);
                        }
                    }
                }
            }

            var workspace = (await this.workspaceUnitOfWork.WorkspaceRepository.GetAll()).Where(x => x.Id == alertsRequestDto.WorkspaceId).FirstOrDefault();
            if (workspace is null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull);
            }

            // delete Channels from Alerts.
            var workspaceNotifications = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository.GetAll())
                .Where(x => x.WorkspaceId == workspace.Id);
            foreach (var item in workspaceNotifications)
            {
                await this.workspaceUnitOfWork.WorkspaceNotificationRepository.Delete(item);
            }

            // add new channels to Alerts.
            workspace.WorkspaceNotifications = notifications;
            await this.workspaceUnitOfWork.WorkspaceRepository.Edit(workspace);

            return await this.workspaceUnitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> AlertSetting(AlertSettingRequestDto alertSettingRequest, string currentUserId)
        {
            if (alertSettingRequest is null || alertSettingRequest.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (alertSettingRequest.NotificationSettings.NotificationSettingConditions.Count > 0)
            {
                foreach (var item in alertSettingRequest.NotificationSettings.NotificationSettingConditions)
                {
                    if (item.Value < 7)
                    {
                        throw new VadarException(ErrorCode.ConditionIsNotSatisfied);
                    }
                }
            }

            if (!await this.ValidatePermission(currentUserId, alertSettingRequest.WorkspaceId, new[] { (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            // check notificationSetting exist.
            var notificationSetting = (await this.notificationSettingUnitOfWork.NotificationSettingRepository.GetAll())
                .Where(x => x.WorkspaceId == alertSettingRequest.WorkspaceId && x.Name == alertSettingRequest.NotificationSettings.Name).FirstOrDefault();
            if (alertSettingRequest.NotificationSettings != null)
            {
                if (notificationSetting == null)
                {
                    var notificationSettingModel = new NotificationSetting
                    {
                        Name = alertSettingRequest.NotificationSettings.Name,
                        Activate = alertSettingRequest.NotificationSettings.Activate,
                        WorkspaceId = alertSettingRequest.WorkspaceId,
                        NotificationSettingConditions = this.mapper.Map<List<NotificationSettingCondition>>(alertSettingRequest.NotificationSettings.NotificationSettingConditions),
                    };

                    await this.notificationSettingUnitOfWork.NotificationSettingRepository.Add(this.mapper.Map<NotificationSetting>(notificationSettingModel));
                }
                else
                {
                    if (notificationSetting.Activate != alertSettingRequest.NotificationSettings.Activate)
                    {
                        notificationSetting.Activate = alertSettingRequest.NotificationSettings.Activate;
                        await this.notificationSettingUnitOfWork.NotificationSettingRepository.Edit(notificationSetting);
                    }

                    foreach (var item in alertSettingRequest.NotificationSettings.NotificationSettingConditions)
                    {
                        var notificationSettingConditions = (await this.notificationSettingConditionUnitOfWork.NotificationSettingConditionRepository.GetAll())
                                                                    .Where(x => x.NotificationSettingId == notificationSetting.Id && x.NotificationType == (EnNotificationType)item.NotificationType).ToList();
                        var conditionExist = notificationSettingConditions.FirstOrDefault(x => x.Condition == (EnConditionType)item.Condition);
                        if (conditionExist != null)
                        {
                            conditionExist.Condition = (EnConditionType)item.Condition;
                            conditionExist.Value = item.Value;
                            conditionExist.NotificationType = (EnNotificationType)item.NotificationType;
                            await this.notificationSettingConditionUnitOfWork.NotificationSettingConditionRepository.Edit(conditionExist);
                        }
                        else
                        {
                            var newCondition = new NotificationSettingCondition
                            {
                                NotificationSettingId = notificationSetting.Id,
                                Condition = (EnConditionType)item.Condition,
                                Value = item.Value,
                                NotificationType = (EnNotificationType)item.NotificationType,
                            };
                            await this.notificationSettingConditionUnitOfWork.NotificationSettingConditionRepository.Add(newCondition);
                        }
                    }
                }
            }

            return await this.notificationSettingUnitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> AddEmailToAlerts(AlertsRequestDto alertsRequestDto, string currentUserId)
        {
            if (alertsRequestDto is null || alertsRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, alertsRequestDto.WorkspaceId, new[] { (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            // validate list email
            var notifications = new List<WorkspaceNotification>();
            foreach (var email in alertsRequestDto.Emails)
            {
                if (string.IsNullOrEmpty(email))
                {
                    continue;
                }

                notifications.Add(new WorkspaceNotification { Address = email });
                if (!this.stringHelper.IsValidEmail(email))
                {
                    throw new VadarException(ErrorCode.EmailInvalid);
                }
            }

            var workspace = await this.workspaceUnitOfWork.WorkspaceRepository.GetWorkspaceById(alertsRequestDto.WorkspaceId);
            if (workspace is null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull);
            }

            // delete Emails from Alerts.
            var workspaceNotifications = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository.GetAll())
                .Where(x => x.WorkspaceId == workspace.Id);
            foreach (var item in workspaceNotifications)
            {
                await this.workspaceUnitOfWork.WorkspaceNotificationRepository.Delete(item);
            }

            // add new emails to Alerts.
            workspace.WorkspaceNotifications = notifications;
            await this.workspaceUnitOfWork.WorkspaceRepository.Edit(workspace);

            return await this.workspaceUnitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<List<WorkspaceNotificationsDto>> ListChannelsAlerts(int workspaceId, string currentUserId)
        {
            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.EmailNotificationView, (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            if (!(await this.workspaceUnitOfWork.WorkspaceRepository.GetAll()).Any(w => w.Id == workspaceId))
            {
                throw new VadarException(ErrorCode.WorkspaceNull);
            }

            var channels = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository
                .FindBy(x => x.WorkspaceId == workspaceId))
                .AsEnumerable()
                .GroupBy(g => g.Type)
                .Select(s => new WorkspaceNotificationsDto
                {
                    Type = (int)s.Key,
                    Address = s.Select(r => r.Address).ToArray(),
                }).ToList();

            return channels;
        }

        /// <inheritdoc/>
        public async Task<List<NotificationSettingViewDto>> GetAlertsSetting(int workspaceId, string currentUserId)
        {
            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.EmailNotificationView, (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var listNotificationSetting = new List<NotificationSettingViewDto>();
            var alertSetting = (await this.notificationSettingUnitOfWork.NotificationSettingRepository.GetAll())
                .Include(x => x.NotificationSettingConditions)
                .Where(x => x.WorkspaceId == workspaceId).ToList();
            foreach (var item in alertSetting)
            {
                var channelSetting = new NotificationSettingViewDto()
                {
                    Name = item.Name,
                    Activate = item.Activate,
                    NotificationSettingConditions = (ICollection<NotiSettingConditionDto>)this.mapper.Map<List<NotiSettingConditionDto>>(item.NotificationSettingConditions),
                };
                listNotificationSetting.Add(channelSetting);
            }

            return listNotificationSetting;
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveEmailFromAlerts(AlertsRequestDto alertsRequestDto, string currentUserId)
        {
            if (alertsRequestDto is null || alertsRequestDto.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, alertsRequestDto.WorkspaceId, new[] { (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var workspace = await this.workspaceUnitOfWork.WorkspaceRepository.GetWorkspaceById(alertsRequestDto.WorkspaceId);
            if (workspace is null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull);
            }

            // delete Emails from Alerts.
            foreach (var email in alertsRequestDto.Emails)
            {
                var alertsDto = new AlertsDto
                {
                    Email = email,
                    WorkspaceName = workspace.Name,
                };
                await this.vadarAlertHelper.RemoveEmailFromAlerts(alertsDto);
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckCondition(AlertSettingRequestDto alertSettingRequest, string currentUserId)
        {
            if (alertSettingRequest is null || alertSettingRequest.WorkspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, alertSettingRequest.WorkspaceId, new[] { (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var check = true;

            // check notificationSetting exist.
            var notificationSetting = (await this.notificationSettingUnitOfWork.NotificationSettingRepository.GetAll())
                .Where(x => x.WorkspaceId == alertSettingRequest.WorkspaceId && x.Name == alertSettingRequest.NotificationSettings.Name).FirstOrDefault();
            if (notificationSetting != null)
            {
                foreach (var item in alertSettingRequest.NotificationSettings.NotificationSettingConditions)
                {
                    var notificationSettingConditions = (await this.notificationSettingConditionUnitOfWork.NotificationSettingConditionRepository.GetAll())
                             .Any(s => s.NotificationSettingId == notificationSetting.Id && s.NotificationType == (EnNotificationType)item.NotificationType && s.Condition == (EnConditionType)item.Condition);
                    if (notificationSettingConditions == true)
                    {
                        check = false;
                    }
                }
            }

            return check;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleleCondition(int workspaceId, string name, int conditionId, string currentUserId)
        {
            if (workspaceId <= 0 || string.IsNullOrEmpty(name) || conditionId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.EmailNotificationSetting, (long)EnPermissions.FullPermission }, this.workspaceUnitOfWork.RolePermissionRepository, this.workspaceUnitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var notificationSetting = (await this.notificationSettingUnitOfWork.NotificationSettingRepository
                .FindBy(x => x.WorkspaceId == workspaceId && x.Name == name)).FirstOrDefault();
            if (notificationSetting == null)
            {
                throw new VadarException(ErrorCode.NotificationSettingNotExist);
            }

            var condition = (await this.notificationSettingConditionUnitOfWork.NotificationSettingConditionRepository
                .FindBy(x => x.NotificationSettingId == notificationSetting.Id && x.Id == conditionId)).FirstOrDefault();
            if (condition == null)
            {
                throw new VadarException(ErrorCode.ConditionNotExists);
            }

            await this.notificationSettingConditionUnitOfWork.NotificationSettingConditionRepository.Delete(condition);

            return await this.notificationSettingConditionUnitOfWork.Commit() > 0;
        }

        private async Task DeleteWorkspaceClaims(int workspaceId)
        {
            var workspaceClaims = (await this.workerNotificationUnitOfWork.WorkspaceClaimRepository.GetAll())
                            .Where(x => x.WorkspaceId == workspaceId && x.ClaimType == Helpers.Const.Constants.WorkspaceClaims.ChatIdTelegram);
            foreach (var wc in workspaceClaims)
            {
                await this.workerNotificationUnitOfWork.WorkspaceClaimRepository.Delete(wc);
            }
        }
    }
}
