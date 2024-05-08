/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;

namespace Corsinvest.AppHero.Core.Notification;

public abstract class NotificationChannelOptions
{
    [Required] public string Name { get; set; } = default!;
    [JsonIgnore] public abstract string Type { get; }
    public bool Enabled { get; set; } = true;
    [JsonIgnore] public abstract string Info { get; }

    [Required]
    public string SuccessUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/trophy-emoji.png";

    [Required]
    public string InfoUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/information-emoji.png";

    [Required]
    public string WarningUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/warning-emoji.png";

    [Required]
    public string ErrorUrlIcon { get; set; } = "https://img.icons8.com/emoji/48/cross-mark-button-emoji.png";

    public string GetSeverityIcon(NotificationSeverity notificationSeverity)
        => notificationSeverity switch
        {
            NotificationSeverity.Success => SuccessUrlIcon,
            NotificationSeverity.Info => InfoUrlIcon,
            NotificationSeverity.Warning => WarningUrlIcon,
            NotificationSeverity.Error => ErrorUrlIcon,
            _ => string.Empty
        };

    public async Task SendTestAsync()
            => await SendImpAsync(new NotificationMessage
            {
                Subject = "Test message from your app",
                Body = "Perfect!! Your app can!"
            });

    public async Task SendAsync(NotificationMessage mailMessage)
    {
        if (Enabled) { await SendImpAsync(mailMessage); }
    }

    protected abstract Task SendImpAsync(NotificationMessage message);
}