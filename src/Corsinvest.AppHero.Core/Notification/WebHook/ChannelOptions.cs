/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;

namespace Corsinvest.AppHero.Core.Notification.WebHook;

public class ChannelOptions : NotificationChannelOptions
{
    public Credential Credential { get; set; } = new Credential();

    [Required]
    public HttpMethod HttpMethod { get; set; } = HttpMethod.Post;

    [Required]
    public BodyType BodyType { get; set; } = BodyType.FormData;

    public override string Type { get; } = "WebHook";

    [Required]
    [Display(Name = "Webhook Url")]
    public string WebHookUrl { get; set; } = default!;

    [JsonIgnore]
    public override string Info => $"{WebHookUrl}";

    [Required]
    [DataType(DataType.MultilineText)]
    public string Model { get; set; } = """
        {
            "Subject": "{Subject}",
            "ColorSeverity": "{ColorSeverity}",
            "ImageSeverity": "{ImageSeverity}",
            "Body": "{Body}",
            "Context": "{Context}",
            "Data": "{Data}"
        }
        """;

    protected override async Task SendImpAsync(NotificationMessage message)
    {
        using var handler = new HttpClientHandler();
        if (!string.IsNullOrEmpty(Credential.Username))
        {
            handler.Credentials = new NetworkCredential(Credential.Username, Credential.Password);
        }
        using var client = new HttpClient(handler);

        var data = Model.Replace("{Subject}", message.Subject)
                        .Replace("{ColorSeverity}", message.ColorSeverity)
                        .Replace("{ImageSeverity}", GetSeverityIcon(message.Severity))
                        .Replace("{Body}", message.Body)
                        .Replace("{Context}", message.Context)
                        .Replace("{Data}", message.Data.Select(a => $"{{\"name\": \"{a.Key}\",\"value\": \"{a.Value}\"}},")
                                                               .JoinAsString(""));

        var content = BodyType switch
        {
            BodyType.FormData => (ByteArrayContent)new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(data)!),
            BodyType.Text => new StringContent(data, System.Text.Encoding.UTF8),
            BodyType.Json => new StringContent(data, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json),
            _ => throw new ArgumentOutOfRangeException(nameof(BodyType)),
        };

        var ret = HttpMethod switch
        {
            HttpMethod.Put => await client.PutAsync(WebHookUrl, content),
            HttpMethod.Post => await client.PostAsync(WebHookUrl, content),
            _ => throw new ArgumentOutOfRangeException(nameof(HttpMethod)),
        };
        if (!ret.IsSuccessStatusCode) { throw new AppHeroException($"Error: {ret.ReasonPhrase}"); }

        //todo send attachment
    }
}