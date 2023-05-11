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

    public async Task SendTest()
        => await SendImplAsync(new NotificationMessage
        {
            Subject = "Test message from your app",
            Body = "Perfect!! Your app can!"
        });

    public async Task SendAsync(NotificationMessage mailMessage)
    {
        if (Enabled) { await SendImplAsync(mailMessage); }
    }

    protected abstract Task SendImplAsync(NotificationMessage message);
}