/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Notification;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Notification;

public partial class NotificationChannelsOptionsSelector<T> where T : INotificationChannelsOptions, new()
{
    [Parameter] public T Options { get; set; } = default!;

    [Inject] private IServiceScopeFactory ServiceScopeFactory { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private List<(string Name, string Icon)> GetAllChannelsNotification()
    {
        using var scope = ServiceScopeFactory.CreateScope();

        var ret = new List<(string Name, string Icon)>();
        foreach (var module in ModularityService.Modules.IsEnabled().Implements<INotification>())
        {
            foreach (var item in NotificationService.GetNotificationChannels(scope, module).Where(a => a.Enabled))
            {
                ret.Add((item.Name, module.ToMBIcon()));
            }
        }

        return ret;
    }
}