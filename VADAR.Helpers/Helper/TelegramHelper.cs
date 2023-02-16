// <copyright file="TelegramHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Telegram Helper.
    /// </summary>
    public class TelegramHelper : ITelegramHelper
    {
        private readonly ILoggerHelper<TelegramHelper> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="TelegramHelper"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        public TelegramHelper(ILoggerHelper<TelegramHelper> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task SendMessage(string token, IEnumerable<long> chatIds, RabbitMQRequestDto requestDto)
        {
            var botClient = new TelegramBotClient(token);
            var title = requestDto.IsSecurity ? "Cảnh báo sự kiện bảo mật VSEC VADAR" : "Cảnh báo về hiệu năng";
            var message = $"<b>{title}</b> \n"
                + $"<b>Tên sự kiện</b> {requestDto.Payload.Description}\n"
                + $"<b>Tên máy chủ</b> {requestDto.Payload.Host}\n"
                + $"<b>Nhật ký</b> {requestDto.Payload.FullLog}\n"
                + $"<b>Mức độ</b> {requestDto.Payload.Level}\n"
                + $"<b>Đã xuất hiện trong 15 phút trước</b> {requestDto.Payload.Count} lần\n"
                + $"<b>Báo cáo chi tiết</b> {requestDto.Payload.Link}\n";
            foreach (var chatId in chatIds)
            {
                var result = await botClient.SendTextMessageAsync(chatId, message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                this.logger.LogInfo($"{nameof(result)}: {result} ---- {nameof(chatId)}: {chatId}");
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<long>> GetChatIds(string token)
        {
            try
            {
                var botClient = new TelegramBotClient(token);
                var updates = await botClient.GetUpdatesAsync();
                if (updates == null || !updates.Any())
                {
                    this.logger.LogInfo($"{nameof(updates)} is null");
                    return null;
                }

                this.logger.LogInfo($"{nameof(updates)} -- {token}: {JsonConvert.SerializeObject(updates.Select(s => s.Message.Chat.Id))}");
                return updates.Select(s => s.Message.Chat.Id);
            }
            catch (Exception)
            {
                throw new VadarException(ErrorCode.TeleTokenInValid);
            }
        }
    }
}
