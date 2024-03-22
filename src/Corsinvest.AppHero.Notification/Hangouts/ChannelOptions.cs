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
using System.Net.Mime;

namespace Corsinvest.AppHero.Notification.Hangouts;

public class ChannelOptions : NotificationChannelOptions
{
    [Required]
    [Display(Name = "Webhook Url")]
    [DataType(DataType.Url)]
    public string WebHookUrl { get; set; } = default!;

    public override string Type { get; } = "Hangouts";

    [JsonIgnore]
    public override string Info => $"{Name}";

    protected override async Task SendImpAsync(NotificationMessage message)
    {
        var data = """
        {
            "text": "{Subject}",
        }
        """;

        data = data.Replace("{Subject}", message.Subject);
        data = data.Replace("{ColorSeverity}", message.ColorSeverity);
        data = data.Replace("{Body}", message.Body);
        data = data.Replace("{Context}", message.Context);
        data = data.Replace("{Data}", message.Data.Select(a => $"{{\"name\": \"{a.Key}\",\"value\": \"{a.Value}\"}},")
                                                          .JoinAsString(""));

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        var content = new StringContent(data, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
        var ret = await client.PostAsync(WebHookUrl, content);
        if (!ret.IsSuccessStatusCode) { throw new AppHeroException($"Error: {ret.ReasonPhrase}"); }

        //todo send attachment
    }
}