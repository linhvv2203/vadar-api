// <copyright file="SendNotificationContentRequest.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.DTO
{
    /// <summary>
    /// Send Notification Request.
    /// </summary>
    public class SendNotificationContentRequest : SendNotificationRequest
    {
        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets Host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets FullLog.
        /// </summary>
        public string FullLog { get; set; }

        /// <summary>
        /// Gets or sets Level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets Workspace Id.
        /// </summary>
        public int WorkspaceId { get; set; }
    }
}
