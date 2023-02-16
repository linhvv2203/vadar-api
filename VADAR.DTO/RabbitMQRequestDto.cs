// <copyright file="RabbitMQRequestDto.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VADAR.DTO
{
    /// <summary>
    /// RabbitMQRequestDto.
    /// </summary>
    public class RabbitMQRequestDto
    {
        /// <summary>
        /// Gets or sets Receiver.
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public SendNotificationContentRequest Payload { get; set; }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsSecurity.
        /// </summary>
        public bool IsSecurity { get; set; }
    }
}
