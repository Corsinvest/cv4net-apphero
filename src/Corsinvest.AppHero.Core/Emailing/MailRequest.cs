/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: bcc

namespace Corsinvest.AppHero.Core.Emailing;

public class MailRequest
{
    public MailRequest(IEnumerable<string> to,
                       string subject,
                       string? body = null,
                       string? from = null,
                       string? displayName = null,
                       string? replyTo = null,
                       string? replyToName = null,
                       IEnumerable<string>? bcc = null,
                       IEnumerable<string>? cc = null,
                       IDictionary<string, byte[]>? attachmentData = null,
                       IDictionary<string, string>? headers = null)
    {
        To = to;
        Subject = subject;
        Body = body;
        From = from;
        DisplayName = displayName;
        ReplyTo = replyTo;
        ReplyToName = replyToName;
        Bcc = bcc ?? new List<string>();
        Cc = cc ?? new List<string>();
        AttachmentData = attachmentData ?? new Dictionary<string, byte[]>();
        Headers = headers ?? new Dictionary<string, string>();
    }

    public IEnumerable<string> To { get; }
    public string Subject { get; }
    public string? Body { get; }
    public string? From { get; }
    public string? DisplayName { get; }
    public string? ReplyTo { get; }
    public string? ReplyToName { get; }
    public IEnumerable<string> Bcc { get; }
    public IEnumerable<string> Cc { get; }
    public IDictionary<string, byte[]> AttachmentData { get; }
    public IDictionary<string, string> Headers { get; }
}
