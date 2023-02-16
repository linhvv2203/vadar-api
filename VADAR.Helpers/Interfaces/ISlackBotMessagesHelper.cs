// <copyright file="ISlackBotMessagesHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VADAR.DTO;
using VADAR.Helpers.Enums;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// ISlackBotMessagesHelper.
    /// </summary>
    public interface ISlackBotMessagesHelper
    {
        /// <summary>
        /// SendMessage.
        /// </summary>
        /// <param name="notification">notification.</param>
        /// <param name="webHookUrl">WebHook Url.</param>
        /// <param name="notificationContent">notification Content enum.</param>
        /// <returns>Task.</returns>
        Task SendMessage(SendNotificationContentRequest notification, string webHookUrl, EnNotificationContent notificationContent);
    }
}
