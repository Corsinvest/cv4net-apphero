/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json;

namespace Corsinvest.AppHero.Core.Notification.Email;

public class ChannelOptions : NotificationChannelOptions
{
    public Credential Credential { get; set; } = new Credential();

    [Required]
    [Display(Name = "Host")]
    public string Host { get; set; } = default!;

    [Required]
    [Display(Name = "Port")]
    public int Port { get; set; } = 25;

    [Display(Name = "Ssl Options")]
    public SecureSocketOptions SslOptions { get; set; } = SecureSocketOptions.Auto;

    [Required]
    [Display(Name = "From Address")]
    [RegularExpression(@"^[\d\w\._\-]+@([\d\w\._\-]+\.)+[\w]+$", ErrorMessage = "Email is invalid")]
    public string FromAddress { get; set; } = default!;

    [Required]
    [Display(Name = "From Display name")]
    public string FromDisplayName { get; set; } = default!;

    [Required]
    [Display(Name = "To Address must be separated with a comma character (\",\")")]
    [RegularExpression(@"^(\s?[^\s,]+@[^\s,]+\.[^\s,]+\s?,)*(\s?[^\s,]+@[^\s,]+\.[^\s,]+)$", ErrorMessage = "Email is invalid")]
    public string ToAddress { get; set; } = default!;

    public override string Type { get; } = "Mail";

    [JsonIgnore]
    public override string Info => $"Host: {Host}:{Port}, SSl: {SslOptions}," +
                                   $"User: {Credential.Username}, From {FromDisplayName}<{FromAddress}>, To: <{ToAddress}>";

    protected override async Task SendImplAsync(NotificationMessage message)
    {
        var email = new MimeMessage
        {
            Subject = message.Subject,
            Sender = new MailboxAddress(FromDisplayName, FromAddress),
        };
        email.From.Add(new MailboxAddress(FromDisplayName, FromAddress));
        email.To.AddRange(ToAddress.Split(",").Select(a => MailboxAddress.Parse(a.Trim())));

        var builder = new BodyBuilder
        {
            HtmlBody = message.Body
        };

        builder.HtmlBody += message.Data.Select(a => $"{a.Key} : {a.Value}").JoinAsString("<br>");

        //Attachments
        foreach (var item in message.Attachments)
        {
            builder.Attachments.Add(item.Name, item.Stream);
        }
        email.Body = builder.ToMessageBody();

        //send message
        using var client = new SmtpClient();
        await client.ConnectAsync(Host, Port, SslOptions);
        if (!string.IsNullOrWhiteSpace(Credential.Username)) { await client.AuthenticateAsync(Credential.Username, Credential.Password); }
        await client.SendAsync(email);
        await client.DisconnectAsync(true);
    }
}
