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
                                                ThumbnailUrl = GetSeverityIcon(message.Severity),
                                                Author = new EmbedAuthorBuilder().WithName(message.Context),
                                                Fields = new( message.Data.Select(a => new EmbedFieldBuilder()
                                                {
                                                    Name = a.Key,
                                                    Value = a.Value
                                                }))
                                            }.Build()
                                      });

        foreach (var item in message.Attachments)
        {
            await client.SendFileAsync(item.Stream, item.Name, string.Empty);
        }
    }
}