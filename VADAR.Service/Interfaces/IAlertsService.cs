// <copyright file="IAlertsService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Alerts Service.
    /// </summary>
    public interface IAlertsService
    {
        /// <summary>
        /// Add Email To Alerts.
        /// </summary>
        /// <param name="alertsRequestDto">alertsRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>True: Success, False: fail.</returns>
        Task<bool> AddEmailToAlerts(AlertsRequestDto alertsRequestDto, string currentUserId);

        /// <summary>
        /// RemoveEmailFromAlerts.
        /// </summary>
        /// <param name="alertsRequestDto">alertsRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>True: Success, False: fail.</returns>
        Task<bool> RemoveEmailFromAlerts(AlertsRequestDto alertsRequestDto, string currentUserId);

        /// <summary>
        /// AddChannelsToAlerts.
        /// </summary>
        /// <param name="alertsRequestDto">alertsRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AddChannelsToAlerts(MultiChannelAlertsRequestDto alertsRequestDto, string currentUserId);

        /// <summary>
        /// AlertSetting.
        /// </summary>
        /// <param name="alertSettingRequest">alertSettingRequest.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AlertSetting(AlertSettingRequestDto alertSettingRequest, string currentUserId);

        /// <summary>
        /// GetAlertsSetting.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<NotificationSettingViewDto>> GetAlertsSetting(int workspaceId, string currentUserId);

        /// <summary>
        /// ListChannelsAlerts.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<WorkspaceNotificationsDto>> ListChannelsAlerts(int workspaceId, string currentUserId);

        /// <summary>
        /// CheckCondition.
        /// </summary>
        /// <param name="alertSettingRequest">alertSettingRequest.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CheckCondition(AlertSettingRequestDto alertSettingRequest, string currentUserId);

        /// <summary>
        /// DeleleCondition.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="name">name.</param>
        /// <param name="conditionId">conditionId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleleCondition(int workspaceId, string name, int conditionId, string currentUserId);
    }
}
