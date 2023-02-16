using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.NotificationApi.Controllers.BaseControllers;
using VADAR.NotificationApi.Model;
using VADAR.Service.Interfaces;

namespace VADAR.NotificationApi.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly ILoggerHelper<NotificationController> logger;
        private readonly INotificationService notificationService;

        public NotificationController(
            ILoggerHelper<NotificationController> logger,
            INotificationService notificationService)
        {
            this.logger = logger;
            this.notificationService = notificationService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<bool>> SendNotification([FromBody]SendNotificationRequest sendNotificationRequest)
        {
            try
            {
                if (sendNotificationRequest != null)
                {
                    this.logger.LogInfo(Newtonsoft.Json.JsonConvert.SerializeObject(sendNotificationRequest));
                }

                return new ApiResponse<bool>(EnApiStatusCode.Success, await notificationService.SendNotification(sendNotificationRequest));
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace + ((sendNotificationRequest != null) ? '\n' + JsonConvert.SerializeObject(sendNotificationRequest) : string.Empty));
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}