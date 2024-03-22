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

    [Required]
    [Display(Name = "Webhook Url")]
    [DataType(DataType.Url)]
    public string WebHookUrl { get; set; } = default!;

    [Required]
    public string IconUrl { get; set; } = default!;

    public override string Type { get; } = "Slack";

    [JsonIgnore]
    public override string Info => $"{Username} - {Channels}";

    protected override async Task SendImpAsync(NotificationMessage message)
    {
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
            Attachments =
            [
                new ()
                {
                    AuthorName = message.Context,
                    Fallback = message.Body,
                    Text = message.Body,
                    AuthorIcon= GetSeverityIcon(message.Severity),
                    Color = message.ColorSeverity,
                }
            ],
        };

        //TODO how to send Data?
        //TODO how to send Attachments?

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