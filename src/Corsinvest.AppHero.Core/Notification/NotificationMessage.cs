/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Notification;

public class NotificationMessage
{
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Context { get; set; } = default!;
    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;
    public IEnumerable<Attachment> Attachments { get; set; } = Array.Empty<Attachment>();
    public Dictionary<string, object> Data { get; set; } = new()!;
}