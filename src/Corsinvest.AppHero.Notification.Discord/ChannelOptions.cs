/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Notification;
using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Notification.Discord;

public class ChannelOptions : NotificationChannelOptions
{
    [Required]
    public string Token { get; set; } = default!;

    [Required]
    public ulong Id { get; set; }

    [Display(Name = "Username")]
    [Required]
    public string Username { get; set; } = default!;

    [Display(Name = "Avatar Url")]
    public string AvatarUrl { get; set; } = default!;

    [Display(Name = "Text to Speech")]
    public bool IsTTS { get; set; }

    public string SuccessUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/trophy-emoji.png";
    public string InfoUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/information-emoji.png";
    public string WarningUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/warning-emoji.png";
    public string ErrorUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/cross-mark-button-emoji.png";

    public override string Type { get; } = "Discord";

    [JsonIgnore]
    public override string Info => $"{Username}";

    protected override async Task SendImplAsync(NotificationMessage message)
    {
        using var client = new DiscordWebhookClient(Id, Token);


        await client.SendMessageAsync(//text: message.Body,
                                      username: Username,
                                      avatarUrl: AvatarUrl,
                                      isTTS: IsTTS,
                                      embeds: new[]
                                      {
                                            new EmbedBuilder
                                            {
                                                Title = message.Subject,
                                                Description = message.Body,
                                                Timestamp = DateTime.UtcNow,
                                                Color = message.Severity switch
                                                {
                                                    NotificationSeverity.Success => Color.Green,
                                                    NotificationSeverity.Info => Color.Blue,
                                                    NotificationSeverity.Warning => Color.Orange,
                                                    NotificationSeverity.Error => Color.Red,
                                                    _ => Color.Default
                                                },
                                                ThumbnailUrl = message.Severity switch
                                                {
                                                    NotificationSeverity.Success => SuccessUrlIcon,
                                                    NotificationSeverity.Info => InfoUrlIcon,
                                                    NotificationSeverity.Warning => WarningUrlIcon,
                                                    NotificationSeverity.Error => ErrorUrlIcon ,
                                                    _ => string.Empty
                                                },
                                                Author = new EmbedAuthorBuilder().WithName(message.Context)
                                            }.Build()
                                      });

        foreach (var item in message.Attachments)
        {
            await client.SendFileAsync(item.Stream, item.Name, string.Empty);
        }
    }
}