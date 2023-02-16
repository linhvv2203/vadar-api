// <copyright file="NotificationDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace VADAR.DTO
{
    /// <summary>
    /// NotificationDto.
    /// </summary>
    public class NotificationDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets Sender.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets Receiver.
        /// </summary>
        public string[] Receiver { get; set; }

        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public int[] Type { get; set; }

        /// <summary>
        /// Gets or sets HostName.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets Telegram Groups.
        /// </summary>
        public string[] TeleGroups { get; set; }

        /// <summary>
        /// Gets or sets CreatedDate.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
