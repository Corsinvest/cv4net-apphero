/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Notification;

public class NotificationService : INotificationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IServiceScopeFactory scopeFactory, ILogger<NotificationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public IEnumerable<NotificationChannelOptions> GetNotificationChannels(IServiceScope scope, ModuleBase module)
    {
        var type = typeof(IOptionsSnapshot<>).MakeGenericType([module.Options!.Type]);
        dynamic options = ((IOptionsSnapshot<object>)scope.ServiceProvider.GetRequiredService(type)).Value;
        return options.Channels;
    }

    public async Task SendAsync(IEnumerable<string> channels, NotificationMessage message)
    {
        using var scope = _scopeFactory.CreateScope();
        var modularityService = scope.ServiceProvider.GetRequiredService<IModularityService>();

        foreach (var item in modularityService.Modules.IsEnabled()
                                                      .Implements<INotification>()
                                                      .SelectMany(a => GetNotificationChannels(scope, a))
                                                      .Where(a => a.Enabled && channels.Contains(a.Name)))
        {
            try
            {
                await item.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SendAsync));
            }
        }
    }
}
