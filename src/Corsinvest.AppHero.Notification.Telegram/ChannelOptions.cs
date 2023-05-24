/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Notification;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Corsinvest.AppHero.Notification.Telegram;

public class ChannelOptions : NotificationChannelOptions
{
    [Display(Name = "BOT API Token")]
    [Required]
    public string Token { get; set; } = default!;

    [Display(Name = "Chat ID")]
    [Required]
    public int ChatId { get; set; }

    public override string Type { get; } = "Telegram";

    [JsonIgnore]
    public override string Info => $"ChatId: {ChatId}";

    protected override async Task SendImplAsync(NotificationMessage message)
    {
        var client = new TelegramBotClient(Token);
        var chatId = new ChatId(ChatId);

        await client.SendTextMessageAsync(chatId, message.Subject);
        if (!string.IsNullOrWhiteSpace(message.Body))
        {
            await client.SendTextMessageAsync(chatId, message.Body);
        }

        foreach (var item in message.Attachments)
        {
            await client.SendDocumentAsync(chatId, InputFile.FromStream(item.Stream, item.Name));
        }
    }
}