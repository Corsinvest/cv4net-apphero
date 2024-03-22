/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core;
using Corsinvest.AppHero.Core.Notification;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Notification.Pushover;

public class ChannelOptions : NotificationChannelOptions
{
    [Required]
    public string AppToken { get; set; } = default!;

    [Required]
    public string UserKey { get; set; } = default!;

    public override string Type { get; } = "Pushover";

    [JsonIgnore]
    public override string Info => $"{Name}";

    protected override async Task SendImpAsync(NotificationMessage message)
    {
        using var client = new HttpClient();
        var ret = await client.PostAsync("https://api.pushover.net/1/messages.json",
                                         new FormUrlEncodedContent(new Dictionary<string, string>
                                         {
                                             ["token"] = AppToken,
                                             ["user"] = UserKey,
                                             ["title"] = message.Subject,
                                             ["message"] = message.Body,
                                         }));
        if (!ret.IsSuccessStatusCode) { throw new AppHeroException($"Error: {ret.ReasonPhrase}"); }

        //todo send attachment
    }
}