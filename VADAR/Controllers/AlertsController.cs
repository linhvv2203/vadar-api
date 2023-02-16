// <copyright file="AlertsController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AlertsController"/> class.
    /// </summary>
    public class AlertsController : BaseController
    {
        private readonly ILoggerHelper<AlertsController> logger;
        private readonly IAlertsService alertsService;

        /// <summary>
        /// Initialises a new instance of the <see cref="AlertsController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="alertsService">Alerts Service.</param>
        public AlertsController(ILoggerHelper<AlertsController> logger, IAlertsService alertsService)
        {
            this.logger = logger;
            this.alertsService = alertsService;
        }

        /// <summary>
        /// ListEmailAlerts.
        /// </summary>
        /// <param name="workspaceId ">workspaceId.</param>
        /// <returns>List Email.</returns>
        [HttpGet]
        public async Task<ApiResponse<List<WorkspaceNotificationsDto>>> ListChannelsAlerts(int workspaceId)
        {
            try
            {
                return new ApiResponse<List<WorkspaceNotificationsDto>>(EnApiStatusCode.Success, await this.alertsService.ListChannelsAlerts(workspaceId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<WorkspaceNotificationsDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// GetAlertsSetting.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetAlertsSetting")]
        public async Task<ApiResponse<List<NotificationSettingViewDto>>> GetAlertsSetting(int workspaceId)
        {
            try
            {
                return new ApiResponse<List<NotificationSettingViewDto>>(EnApiStatusCode.Success, await this.alertsService.GetAlertsSetting(workspaceId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<List<NotificationSettingViewDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// AddEmailToAlerts.
        /// </summary>
        /// <param name="alertsRequestDto ">alertsRequestDto.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        public async Task<ApiResponse<bool>> AddEmailToAlerts([FromBody] AlertsRequestDto alertsRequestDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.alertsService.AddEmailToAlerts(alertsRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// AddChannelsToAlerts.
        /// </summary>
        /// <param name="alertsRequestDto">alertsRequestDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("AddChannelsToAlerts")]
        public async Task<ApiResponse<bool>> AddChannelsToAlerts([FromBody] MultiChannelAlertsRequestDto alertsRequestDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.alertsService.AddChannelsToAlerts(alertsRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Alert Setting.
        /// </summary>
        /// <param name="alertSettingRequest">alertSettingRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("AlertSetting")]
        public async Task<ApiResponse<bool>> AlertSetting([FromBody] AlertSettingRequestDto alertSettingRequest)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.alertsService.AlertSetting(alertSettingRequest, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// RemoveEmailFromAlerts.
        /// </summary>
        /// <param name="alertsRequestDto ">alertsRequestDto.</param>
        /// <returns>bool.</returns>
        [HttpDelete]
        public async Task<ApiResponse<bool>> RemoveEmailFromAlerts([FromBody] AlertsRequestDto alertsRequestDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.alertsService.RemoveEmailFromAlerts(alertsRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// DeleleCondition.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="name">name.</param>
        /// <param name="conditionId">conditionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete]
        [Route("DeleleCondition")]
        public async Task<ApiResponse<bool>> DeleleCondition(int workspaceId, string name, int conditionId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.alertsService.DeleleCondition(workspaceId, name, conditionId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// CheckCondition.
        /// </summary>
        /// <param name="alertSettingRequest">alertSettingRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("CheckCondition")]
        public async Task<ApiResponse<bool>> CheckCondition([FromBody] AlertSettingRequestDto alertSettingRequest)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.alertsService.CheckCondition(alertSettingRequest, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
