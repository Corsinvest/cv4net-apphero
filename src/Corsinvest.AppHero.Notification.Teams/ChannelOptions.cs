/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Notification;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace Corsinvest.AppHero.Notification.Teams;

public class ChannelOptions : NotificationChannelOptions
{
    [Required]
    [Display(Name = "Webhook Url")]
    [DataType(DataType.Url)]
    public string WebHookUrl { get; set; } = default!;

    [Required]
    [DataType(DataType.MultilineText)]
    public string Model { get; set; } = """
        {
            "@type": "MessageCard",
            "summary": "{Subject}",
            "themeColor": "{ColorSeverity}",
            "sections": [
            { 
                "activityTitle": "{Subject}",
                "activityImage": "{ImageSeverity}",
                "text": "{Body}",
                "facts": [
                {Data}
                {
                        "name": "Context:",
                        "value": "{Context}"
                }]
            }]
        }
        """;

    public override string Type { get; } = "Teams";

    [JsonIgnore]
    public override string Info => $"{Name}";

    protected override async Task SendImplAsync(NotificationMessage message)
    {
        var data = Model.Replace("{Subject}", message.Subject)
                        .Replace("{ColorSeverity}", message.ColorSeverity)
                        .Replace("{ImageSeverity}", GetSeverityIcon(message.Severity))
                        .Replace("{Body}", message.Body)
                        .Replace("{Context}", message.Context)
                        .Replace("{Data}", message.Data.Select(a => $"{{\"name\": \"{a.Key}\",\"value\": \"{a.Value}\"}},")
                                                               .JoinAsString(""));

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
        var ret = await client.PostAsync(WebHookUrl, content);
        if (!ret.IsSuccessStatusCode) { throw new AppHeroException($"Error: {ret.ReasonPhrase}"); }

        //todo send attachment
    }
}