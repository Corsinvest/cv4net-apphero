/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Models;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Net;

namespace Corsinvest.AppHero.Core.Notification.WebHook;

public class ChannelOptions : NotificationChannelOptions
{
    public Credential Credential { get; set; } = new Credential();

    [Required]
    public HttpMethod HttpMethod { get; set; } = default!;

    public override string Type { get; } = "WebHook";

    [Display(Name = "Webhook Url")]
    [Required]
    public string WebHookUrl { get; set; } = default!;

    [JsonIgnore]
    public override string Info => $"{WebHookUrl}";

    protected override async Task SendImplAsync(NotificationMessage message)
    {
        using var handler = new HttpClientHandler();
        if (!string.IsNullOrEmpty(Credential.Username))
        {
            handler.Credentials = new NetworkCredential(Credential.Username, Credential.Password);
        }
        using var client = new HttpClient(handler);

        var dic = new Dictionary<string, string>()
        {
            ["Subject"] = message.Subject,
            ["Body"] = message.Body,
            ["Context"] = message.Context,
            ["Severity"] = message.Severity.ToString(),
        };
        dic.AddRange(message.Data.ToDictionary(a => $"Data_{a.Key}", a => a.Value.ToString()!));

        var content = new FormUrlEncodedContent(dic);

        var ret = HttpMethod switch
        {
            HttpMethod.Put => await client.PutAsync(WebHookUrl, content),
            HttpMethod.Post or _ => await client.PostAsync(WebHookUrl, content),
        };
        if (!ret.IsSuccessStatusCode) { throw new AppHeroException($"Error: {ret.ReasonPhrase}"); }

        //todo send attachment
    }
}