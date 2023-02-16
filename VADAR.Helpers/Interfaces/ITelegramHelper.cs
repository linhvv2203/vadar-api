// <copyright file="ITelegramHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Telegram helper.
    /// </summary>
    public interface ITelegramHelper
    {
        /// <summary>
        /// SendMessage.
        /// </summary>
        /// <param name="token">token.</param>
        /// <param name="chatIds">Chat Ids.</param>
        /// <param name="payload">payload.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendMessage(string token, IEnumerable<long> chatIds, RabbitMQRequestDto payload);

        /// <summary>
        /// Get chatIds.
        /// </summary>
        /// <param name="token">token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<long>> GetChatIds(string token);
    }
}
