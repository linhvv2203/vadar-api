// <copyright file="Constants.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;

namespace VADAR.Helpers.Const
{
    /// <summary>
    /// Constants for system.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Workspace Claims.
        /// </summary>
        public class WorkspaceClaims
        {
            /// <summary>
            /// Chat Id Telegram.
            /// </summary>
            public const string ChatIdTelegram = "ChatIdTelegram";
        }

        /// <summary>
        /// Notification channels.
        /// </summary>
        public class NotificationChannels
        {
            /// <summary>
            /// Security.
            /// </summary>
            public const string Security = "New Security Event";

            /// <summary>
            /// Performance.
            /// </summary>
            public const string Performance = "New Performance Event";
        }

        /// <summary>
        /// User Role Constants.
        /// </summary>
        public class UserRoles
        {
            /// <summary>
            /// Gets admin.
            /// </summary>
            public static Guid Admin => new Guid("b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6");

            /// <summary>
            /// Gets user.
            /// </summary>
            public static Guid User => new Guid("52f69311-ac69-4959-a6a5-5cd0f81ccbc4");
        }

        /// <summary>
        /// Security Tabs Constants.
        /// </summary>
        public class SecurityTabs
        {
            /// <summary>
            /// MITRE ATT CK.
            /// </summary>
            public const string MITREATTCK = "mitreattck";

            /// <summary>
            /// PCI DSS.
            /// </summary>
            public const string PCIDSS = "pcidss";

            /// <summary>
            /// Security Events.
            /// </summary>
            public const string SecurityEvents = "se";

            /// <summary>
            /// Security Integrity Monitoring.
            /// </summary>
            public const string SecurityIntegrityMonitoring = "sim";

            /// <summary>
            /// Security Vulnerabilities.
            /// </summary>
            public const string SecurityVulnerabilities = "sv";
        }

        /// <summary>
        /// Severity Name Constants.
        /// </summary>
        public class SeverityNameConstants
        {
            /// <summary>
            /// Low.
            /// </summary>
            public const string Low = "low";

            /// <summary>
            /// Information.
            /// </summary>
            public const string Info = "info";

            /// <summary>
            /// Medium.
            /// </summary>
            public const string Medium = "medium";

            /// <summary>
            /// High.
            /// </summary>
            public const string High = "high";
        }

        /// <summary>
        /// Agent Os.
        /// </summary>
        public class Os
        {
            /// <summary>
            /// ubuntu os.
            /// </summary>
            public const string Ubuntu = "ubuntu";

            /// <summary>
            /// Centos os.
            /// </summary>
            public const string Centos = "centos";

            /// <summary>
            /// Window os.
            /// </summary>
            public const string Window = "window";

            /// <summary>
            /// Mac os.
            /// </summary>
            public const string Macos = "macos";
        }

        /// <summary>
        /// Language Code.
        /// </summary>
        public class LanguageCodeConstants
        {
            /// <summary>
            /// Vietnamese.
            /// </summary>
            public const string Vietnamese = "vi";

            /// <summary>
            /// English US.
            /// </summary>
            public const string EnglishUs = "en-US";
        }

        /// <summary>
        /// Language Code Client.
        /// </summary>
        public class LanguageCodeClient
        {
            /// <summary>
            /// Vietnamese.
            /// </summary>
            public const string Vietnamese = "vn";

            /// <summary>
            /// English US.
            /// </summary>
            public const string EnglishUs = "en-US";
        }

        /// <summary>
        /// Role Name System.
        /// </summary>
        public class RoleNameSystem
        {
            /// <summary>
            /// Admin Role.
            /// </summary>
            public const string Admin = "Admin";

            /// <summary>
            /// Editor Role.
            /// </summary>
            public const string Editor = "Editor";

            /// <summary>
            /// Reader Role.
            /// </summary>
            public const string Reader = "Reader";
        }

        /// <summary>
        /// Prefix Cache Redis.
        /// </summary>
        public class PrefixCache
        {
            /// <summary>
            /// Prefix User.
            /// </summary>
            public const string PrefixUser = "auth_";

            /// <summary>
            /// Prefix User Chat Service.
            /// </summary>
            public const string PrefixUserChat = "auth_chat_";
        }

        /// <summary>
        /// Notification Type.
        /// </summary>
        public class NotificationTypeConstants
        {
            /// <summary>
            /// Report.
            /// </summary>
            public const string Report = "VAD_REPORT";

            /// <summary>
            /// License.
            /// </summary>
            public const string License = "VAD_LICENSE";

            /// <summary>
            /// ZBX.
            /// </summary>
            public const string ZBX = "ZBX";
        }

        /// <summary>
        /// Category Constants class.
        /// </summary>
        public class CategoriesConstants
        {
            /// <summary>
            /// Warning.
            /// </summary>
            public const int Warning = 5;

            /// <summary>
            /// Tin Tuc.
            /// </summary>
            public const int TinTuc = 6;

            /// <summary>
            /// News And Media.
            /// </summary>
            public const int NewsAndMedia = 101;

            /// <summary>
            /// Tin tức và truyền thông.
            /// </summary>
            public const int TinTucVaTruyenThong = 102;
        }
    }
}
