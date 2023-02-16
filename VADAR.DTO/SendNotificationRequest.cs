// <copyright file="SendNotificationRequest.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace VADAR.DTO
{
    /// <summary>
    /// Send Notification Request.
    /// </summary>
    public class SendNotificationRequest
    {
        private string receiverId;

        /// <summary>
        /// Gets or sets ReceiverId.
        /// </summary>
        public string ReceiverId
        {
            get => this.receiverId ?? this.SecondReceiverId;
            set => this.receiverId = value;
        }

        /// <summary>
        /// Gets or sets second Receiver Id.
        /// </summary>
        [JsonProperty("receiver_id")]
        public string SecondReceiverId { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        public dynamic Message { get; set; }

        /// <summary>
        /// Gets or sets Type: Wazuh, Zabbix, Safesai.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets Access Key.
        /// </summary>
        public string AccessKey { get; set; }
    }
}
