// <copyright file="IMessageQueueHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// IMessageQueueHelper.
    /// </summary>
    public interface IMessageQueueHelper
    {
        /// <summary>
        /// Send message To RabbitMQ.
        /// </summary>
        /// <param name="payload">payload.</param>
        void SendToRabbitMQ(RabbitMQRequestDto payload);

        /// <summary>
        /// ReceiveRabbitMQ.
        /// </summary>
        void ReceiveRabbitMQ();
    }
}
