// <copyright file="SlackBotMessagesHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SlackBotMessages;
using SlackBotMessages.Models;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// SlackBotMessagesHelper.
    /// </summary>
    public class SlackBotMessagesHelper : ISlackBotMessagesHelper
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerHelper<SlackBotMessagesHelper> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="SlackBotMessagesHelper"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        /// <param name="logger">logger.</param>
        public SlackBotMessagesHelper(
            IConfiguration configuration,
            ILoggerHelper<SlackBotMessagesHelper> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task SendMessage(SendNotificationContentRequest notification, string webHookUrl, EnNotificationContent notificationContent)
        {
            var client = new SbmClient(webHookUrl);
            var message = this.GetMessage(notification, notificationContent);

            var result = await client.SendAsync(message);
            this.logger.LogInfo($"{nameof(result)}: {result} ---- {nameof(webHookUrl)}: {webHookUrl}");
        }

        private Message GetMessage(SendNotificationContentRequest notification, EnNotificationContent notificationContent)
        {
            var message = new Message()
                   .SetUserWithEmoji("VSEC VADAR", Emoji.Loudspeaker);
            switch (notificationContent)
            {
                case EnNotificationContent.Security:
                    message.AddAttachment(new Attachment()
                    .AddField("Cảnh báo sự kiện bảo mật VSEC VADAR", string.Empty, false)
                    .AddField("Tên sự kiện", notification.Description, false)
                    .AddField("Tên máy chủ", notification.Host, false)
                    .AddField("Nhật ký", notification.FullLog, false)
                    .AddField("Mức độ", notification.Level, false)
                    .AddField("Đã xuất hiện trong 15 phút trước", $"{notification.Count} lần", false)
                    .AddField("Báo cáo chi tiết", notification.Link, false)

                    // .SetThumbUrl("https://codeshare.co.uk/media/1508/paul-seal-profile-2019.jpg?width=500&height=500&mode=crop&anchor=top")
                    .SetColor("#f96332"));
                    break;
                case EnNotificationContent.Performance:
                    message.AddAttachment(new Attachment()
                    .AddField("Cảnh báo về hiệu năng", string.Empty, false)
                    .AddField("Tên sự kiện", notification.Description, false)
                    .AddField("Tên máy chủ", notification.Host, false)
                    .AddField("Nhật ký", notification.FullLog, false)
                    .AddField("Mức độ", notification.Level, false)
                    .AddField("Báo cáo chi tiết", notification.Link, false)
                    .SetColor("#f96332"));
                    break;
            }

            return message;
        }
    }
}
