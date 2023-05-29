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

    public string ColorSeverity => Severity switch
    {
        NotificationSeverity.Success => "#00c853",
        NotificationSeverity.Info => "#2196f3",
        NotificationSeverity.Warning => "#ff9800",
        NotificationSeverity.Error => "#f44336",
        _ => string.Empty
    };

    public IEnumerable<Attachment> Attachments { get; set; } = Array.Empty<Attachment>();
    public Dictionary<string, object> Data { get; set; } = new()!;
}