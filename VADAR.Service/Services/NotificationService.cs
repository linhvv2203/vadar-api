// <copyright file="NotificationService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Const;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;
using static VADAR.Helpers.Const.Constants;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Notification Service.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IRazorViewHelper razorViewHelper;
        private readonly IConfiguration configuration;
        private readonly IHostUnitOfWork unitOfWork;
        private readonly IEmailService emailService;
        private readonly IStringHelper stringHelper;
        private readonly IWorkspaceUnitOfWork workspaceUnitOfWork;
        private readonly IRedisCachingHelper redisCachingHelper;
        private readonly ISlackBotMessagesHelper slackBotMessagesHelper;
        private readonly ITelegramHelper telegramHelper;
        private readonly IMessageQueueHelper messageQueueHelper;
        private readonly ILoggerHelper<NotificationService> logger;
        private int workspaceId;
        private int count;

        /// <summary>
        /// Initialises a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="workspaceUnitOfWork">workspace Unit Of Work.</param>
        /// <param name="emailService">emailService.</param>
        /// <param name="stringHelper">String Helper.</param>
        /// <param name="razorViewHelper">razorViewHelper.</param>
        /// <param name="configuration">configuration.</param>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="redisCachingHelper">redisCachingHelper.</param>
        /// <param name="slackBotMessagesHelper">slackBotMessagesHelper.</param>
        /// <param name="telegramHelper">telegramHelper.</param>
        /// <param name="messageQueueHelper">message Queue Helper.</param>
        /// <param name="logger">logger.</param>
        public NotificationService(
            IRazorViewHelper razorViewHelper,
            IConfiguration configuration,
            IHostUnitOfWork unitOfWork,
            IWorkspaceUnitOfWork workspaceUnitOfWork,
            IEmailService emailService,
            IStringHelper stringHelper,
            IRedisCachingHelper redisCachingHelper,
            ISlackBotMessagesHelper slackBotMessagesHelper,
            ITelegramHelper telegramHelper,
            IMessageQueueHelper messageQueueHelper,
            ILoggerHelper<NotificationService> logger)
        {
            this.razorViewHelper = razorViewHelper;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.stringHelper = stringHelper;
            this.workspaceUnitOfWork = workspaceUnitOfWork;
            this.redisCachingHelper = redisCachingHelper;
            this.slackBotMessagesHelper = slackBotMessagesHelper;
            this.telegramHelper = telegramHelper;
            this.messageQueueHelper = messageQueueHelper;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> SendNotification(SendNotificationRequest sendNotificationRequest)
        {
            if (sendNotificationRequest == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(ErrorCode.ArgumentNull));
            }

            var accessKey = this.configuration["AccessKey"];

            if (string.IsNullOrWhiteSpace(sendNotificationRequest.AccessKey)
               || string.IsNullOrWhiteSpace(accessKey)
               || accessKey.Trim() != sendNotificationRequest.AccessKey.Trim())
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var notiSetting = this.configuration.GetSection("Alert:Type").Get<string>();

            var sendNotiContentRequest = new SendNotificationContentRequest
            {
                Message = sendNotificationRequest.Message,
                ReceiverId = sendNotificationRequest.ReceiverId,
                Type = sendNotificationRequest.Type,
            };

            switch (sendNotificationRequest.Type)
            {
                case Constants.NotificationTypeConstants.Report:
                    {
                        const string emailSubject = "Vadar Report";
                        sendNotiContentRequest.Link = this.configuration["ClientServerUrl"];
                        var emailContent = await this.razorViewHelper.RenderViewAsString("EmailTemplates/vi-vn/SendEmailReportVadar", sendNotiContentRequest);
                        await this.emailService.SendAsync(sendNotificationRequest.ReceiverId, emailSubject, emailContent, true);
                        break;
                    }

                case Constants.NotificationTypeConstants.License:
                    {
                        const string emailSubject = "Vadar Notification";
                        var emailContent = await this.razorViewHelper.RenderViewAsString("EmailTemplates/vi-vn/SendWarningVadar", sendNotiContentRequest);
                        await this.emailService.SendAsync(sendNotificationRequest.ReceiverId, emailSubject, emailContent, true);
                        break;
                    }

                case Constants.NotificationTypeConstants.ZBX:
                    {
                        await this.SendWarningEmail(sendNotiContentRequest, sendNotificationRequest, false);
                        break;
                    }

                default:
                    if (notiSetting == "Email")
                    {
                        await this.SendWarningEmail(sendNotiContentRequest, sendNotificationRequest, true);
                    }

                    break;
            }

            return true;
        }

        private static dynamic GetValueFieldsFromDataDynamic(dynamic data, string name)
        {
            try
            {
                var result = data.Value<string>(name);
                return result;
            }
            catch (RuntimeBinderException)
            {
                return null;
            }
        }

        private async Task SendWarningEmail(
            SendNotificationContentRequest sendNotiContentRequest,
            SendNotificationRequest sendNotificationRequest,
            bool isSecurity)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(sendNotificationRequest.Message.ToString());
            var receiverId = sendNotificationRequest.ReceiverId;
            var redisKey = string.Empty;
            var redisValue = string.Empty;

            if (isSecurity)
            {
                receiverId = data[0]?.agent.id.ToString();
                redisKey = data[0]?.rule.id;
                redisValue = data[0]?.rule.description;
            }
            else
            {
                redisKey = data?.trigger_id;
                redisValue = data?.trigger_name;
            }

            // Get workspace Id from ReceiverId.
            this.workspaceId = (await this.unitOfWork.WorkspaceHostRepository.GetAll())
                .Where(x => isSecurity ? x.Host.WazuhRef == receiverId : x.Host.NameEngine == receiverId)
                .Select(s => s.WorkspaceId)
                .FirstOrDefault();

            if (this.workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            bool isCVE = redisValue.IndexOf("CVE") >= 0;
            if (isCVE)
            {
                var valueCVERedis = await this.redisCachingHelper.GetDataByKey("CVE-" + this.workspaceId.ToString());
                if (string.IsNullOrEmpty(valueCVERedis))
                {
                    // 24 hours
                    _ = this.redisCachingHelper.SetStringData("CVE-" + this.workspaceId.ToString(), redisValue, 86400);
                    if (isSecurity)
                    {
                        this.count = 0;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                var valueRedis = await this.redisCachingHelper.GetDataByKey(redisKey + "-" + this.workspaceId.ToString());
                if (string.IsNullOrEmpty(valueRedis))
                {
                    // 15 minutes
                    _ = this.redisCachingHelper.SetStringData(redisKey + "-" + this.workspaceId.ToString(), redisValue, 900);
                    if (isSecurity)
                    {
                        this.count = await this.GetCountLast15InWazuh(receiverId, redisValue);
                    }
                }
                else
                {
                    return;
                }
            }

            var notificationSettings = (await this.workspaceUnitOfWork.NotificationSettingRepository.GetAll())
                    .Include(c => c.NotificationSettingConditions)
                    .Where(x => x.WorkspaceId == this.workspaceId).ToList();

            // Get email list to send notification.
            var workspaceNotifications = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository.GetAll())
                .Where(x => x.WorkspaceId == this.workspaceId && !string.IsNullOrWhiteSpace(x.Address))
                .OrderByDescending(d => d.Type);

            foreach (var receiver in workspaceNotifications)
            {
                this.AdditionalDataToSendNotificationContentRequest(sendNotiContentRequest, isSecurity, data);
                if (isSecurity)
                {
                    var securitySetting = notificationSettings.FirstOrDefault(x => x.Activate && x.Name == NotificationChannels.Security);
                    if (securitySetting == null || securitySetting.NotificationSettingConditions == null
                        || !this.IsVerirySettingCondition(sendNotiContentRequest.Level, securitySetting, receiver.Type))
                    {
                        continue;
                    }
                }
                else
                {
                    var performanceSetting = notificationSettings.FirstOrDefault(x => x.Activate && x.Name == NotificationChannels.Performance);
                    if (performanceSetting == null)
                    {
                        continue;
                    }
                }

                var rabbitMQRequest = new RabbitMQRequestDto
                {
                    Receiver = receiver.Address,
                    Payload = sendNotiContentRequest,
                    Type = (int)receiver.Type,
                    IsSecurity = isSecurity,
                };

                this.messageQueueHelper.SendToRabbitMQ(rabbitMQRequest);
            }
        }

        private SendNotificationContentRequest AdditionalDataToSendNotificationContentRequest(SendNotificationContentRequest sendNotiContentRequest, bool isSecurity, dynamic data)
        {
            var timeStamp = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
            dynamic dataContent;
            if (isSecurity)
            {
                var isCVE = data[0]?.rule?.description.ToString().IndexOf("CVE") >= 0;
                dataContent = new
                {
                    timestamp = timeStamp,
                    description = isCVE ? "Hệ thống của bạn có lỗ hổng CVE" : data[0]?.rule?.description.ToString(),
                    fullLog = isCVE ? string.Empty : GetValueFieldsFromDataDynamic(data[0], "full_log"),
                    groups = string.Join(", ", data[0]?.rule?.groups).ToString(),
                    host = isCVE ? string.Empty : data[0]?.agent?.name.ToString(),
                    level = isCVE ? string.Empty : data[0]?.rule?.level.ToString(),
                    this.workspaceId,
                    this.count,
                };

                sendNotiContentRequest.Link = this.GetLinkDetail(this.workspaceId, dataContent);
                sendNotiContentRequest.Description = dataContent.description;
                sendNotiContentRequest.Host = dataContent.host;
                sendNotiContentRequest.FullLog = dataContent.fullLog;
                sendNotiContentRequest.Level = dataContent.level;
                sendNotiContentRequest.Count = dataContent.count;
                sendNotiContentRequest.WorkspaceId = this.workspaceId;
            }
            else
            {
                dataContent = new
                {
                    time = timeStamp,
                    description = data.trigger_description.ToString(),
                    eventName = data.event_name1.ToString(),
                    severity = data.trigger_severity.ToString(),
                    hostName = data.event_host1.ToString(),
                    status = data.trigger_status.ToString(),
                    this.workspaceId,
                };

                var link = this.configuration["ClientServerUrl"] + "/performance?params=" +
                           WebUtility.UrlEncode(this.stringHelper.EncodeBase64(JsonConvert.SerializeObject(dataContent))) +
                           "&wp_id=" + this.workspaceId;

                sendNotiContentRequest.Link = link;
                sendNotiContentRequest.Description = dataContent.description;
                sendNotiContentRequest.Host = dataContent.eventName;
                sendNotiContentRequest.FullLog = dataContent.description;
                sendNotiContentRequest.Level = dataContent.severity;
            }

            return sendNotiContentRequest;
        }

        private string GetLinkDetail(int workspaceId, dynamic dataContent)
        {
            return this.configuration["ClientServerUrl"] + "/security?params=" +
                           WebUtility.UrlEncode(this.stringHelper.EncodeBase64(JsonConvert.SerializeObject(dataContent))) +
                           "&wp_id=" + workspaceId;
        }

        private bool IsVerirySettingCondition(string levelInput, Model.Models.NotificationSetting notificationSetting, EnNotificationType type)
        {
            var below = notificationSetting.NotificationSettingConditions.FirstOrDefault(x => x.NotificationType == type && x.Condition == EnConditionType.Below);
            var above = notificationSetting.NotificationSettingConditions.FirstOrDefault(x => x.NotificationType == type && x.Condition == EnConditionType.Above);
            int.TryParse(levelInput, out int eventLevel);
            if(eventLevel == 0)
            {
                eventLevel = 8;
            }

            if (below == null && above == null)
            {
                return false;
            }

            if (below == null && above != null)
            {
                return eventLevel > above.Value;
            }

            if (below != null && above == null)
            {
                return eventLevel < below.Value;
            }

            return eventLevel > above.Value && eventLevel < below.Value;
        }

        private async Task<int> GetCountLast15InWazuh(string host, string name, int minutes = 15)
        {
            var url = $"{this.configuration["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();
            var toDate = DateTime.Now;
            var fromDate = DateTime.Now.AddMinutes(-minutes);

            var body = @"{
                            ""version"": true,
                            ""size"": 0,
                            ""stored_fields"": [
                                ""*""
                            ],
                            ""_source"": {
                                ""excludes"": [
                                ""@timestamp""
                                ]
                            },
                            ""query"": {
                                ""bool"": {
                                    ""filter"": [
                                        {
                                            ""match_phrase"": {
                                                ""agent.id"": """ + host + @"""
                                            }
                                        },
                                        {
                                            ""match_phrase"": {
                                                ""rule.description"": """ + name + @"""
                                            }
                                        },
                                        {
                                            ""range"": {
                                                ""timestamp"": {
                                                ""gte"": """ + fromDate.ToUniversalTime().ToString("o") + @""",
                                                ""lte"": """ + toDate.ToUniversalTime().ToString("o") + @""",
                                                ""format"": ""strict_date_optional_time""
                                                }
                                            }
                                        }
                                    ]
                                }
                            }
                        }";

            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseBody))
            {
                return 0;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseBody);

            if (data == null)
            {
                return 0;
            }

            return data.hits.total.value;
        }
    }
}
