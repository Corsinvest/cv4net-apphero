/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Notification;
using Newtonsoft.Json;
using Slack.Webhooks;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Notification.Slack;

public class ChannelOptions : NotificationChannelOptions
{
    [Display(Name = "Username")]
    [Required]
    public string Username { get; set; } = default!;

    [Display(Name = "Channels must be separated with a comma character (\",\")")]
    [Required]
    public string Channels { get; set; } = default!;

    [Display(Name = "Webhook Url")]
    [Required]
    public string WebHookUrl { get; set; } = default!;

    [Required]
    public string IconUrl { get; set; } = default!;

    [Required]
    public string SuccessUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/trophy-emoji.png";

    [Required]
    public string InfoUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/information-emoji.png";

    [Required]
    public string WarningUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/warning-emoji.png";

    [Required]
    public string ErrorUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/cross-mark-button-emoji.png";

    public override string Type { get; } = "Slack";

    [JsonIgnore]
    public override string Info => $"{Username} - {Channels}";

    protected override async Task SendImplAsync(NotificationMessage message)
    {
        message.Context = "mail";
        message.Severity = NotificationSeverity.Info;

        var slackMessage = new SlackMessage
        {
            IconUrl = new Uri(IconUrl),
            Text = message.Subject,
            IconEmoji = message.Severity switch
            {
                NotificationSeverity.Success => Emoji.Trophy,
                NotificationSeverity.Info => Emoji.InformationSource,
                NotificationSeverity.Warning => Emoji.Warning,
                NotificationSeverity.Error => Emoji.X,
                _ => string.Empty
            },
            Username = Username,
            Attachments = new()
            {
                new ()
                {
                    AuthorName = message.Context,
                    Fallback = message.Body,
                    Text = message.Body,
                    AuthorIcon= message.Severity switch
                    {
                        NotificationSeverity.Success => SuccessUrlIcon,
                        NotificationSeverity.Info => InfoUrlIcon,
                        NotificationSeverity.Warning => WarningUrlIcon,
                        NotificationSeverity.Error => ErrorUrlIcon ,
                        _ => string.Empty
                    },
                    Color = message.Severity switch
                    {
                        NotificationSeverity.Success => "#00c853",
                        NotificationSeverity.Info => "#2196f3",
                        NotificationSeverity.Warning => "#ff9800",
                        NotificationSeverity.Error => "#f44336",
                        _ => string.Empty
                    },
                }
            },
        };

        //foreach (var item in message.Attachments)
        //{
        //slackMessage.Attachments.Add(new SlackAttachment
        //    {

        //        //Fallback = message.Body,
        //        //Text = message.Body,
        //        //Color = "#D00000",
        //    });
        //}

        using var client = new SlackClient(WebHookUrl);
        client.PostToChannels(slackMessage, Channels.Split(','));
        await Task.CompletedTask;
    }
}