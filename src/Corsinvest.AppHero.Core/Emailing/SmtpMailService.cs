/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using MailKit.Net.Smtp;
using MimeKit;

namespace Corsinvest.AppHero.Core.Emailing;

public class SmtpMailService : IMailService
{
    private readonly Options _options;
    private readonly ILogger<SmtpMailService> _logger;

    public SmtpMailService(IOptionsSnapshot<Options> options, ILogger<SmtpMailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(MailRequest request)
    {
        try
        {
            var message = new MimeMessage
            {
                Subject = request.Subject,
                Sender = new MailboxAddress(request.DisplayName ?? _options.DisplayName, request.From ?? _options.From)
            };

            message.From.Add(new MailboxAddress(request.DisplayName ?? _options.DisplayName, request.From ?? _options.From));
            message.To.AddRange(request.To.Select(a => MailboxAddress.Parse(a)));

            // Reply To
            if (!string.IsNullOrEmpty(request.ReplyTo))
            {
                message.ReplyTo.Add(new MailboxAddress(request.ReplyToName, request.ReplyTo));
            }

            message.Bcc.AddRange(request.Bcc.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => MailboxAddress.Parse(a.Trim())));
            message.Cc.AddRange(request.Cc.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => MailboxAddress.Parse(a.Trim())));

            // Headers
            request.Headers.ForEach(a => message.Headers.Add(a.Key, a.Value));

            // Content
            var builder = new BodyBuilder
            {
                HtmlBody = request.Body
            };

            // Create the file attachments for this e-mail message
            request.AttachmentData.ForEach(a => builder.Attachments.Add(a.Key, a.Value));

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, _options.SslOptions);
            if (!string.IsNullOrWhiteSpace(_options.Username)) { await client.AuthenticateAsync(_options.Username, _options.Password); }
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}