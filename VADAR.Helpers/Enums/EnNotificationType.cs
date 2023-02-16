// <copyright file="EnNotificationType.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace VADAR.Helpers.Enums
{
    /// <summary>
    /// EnNotificationType.
    /// </summary>
    public enum EnNotificationType
    {
        /// <summary>
        /// Email.
        /// </summary>
        Email = 0,

        /// <summary>
        /// Slack.
        /// </summary>
        Slack = 1,

        /// <summary>
        /// TeleGram.
        /// </summary>
        TeleGram = 2,

        /// <summary>
        /// Zalo.
        /// </summary>
        Zalo = 3,

        /// <summary>
        /// Sms.
        /// </summary>
        Sms = 4,
    }
}
