/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Notification;

public interface INotificationService : ITransientDependency
{
    Task SendAsync(IEnumerable<string> channels, NotificationMessage message);

    IEnumerable<NotificationChannelOptions> GetNotificationChannels(IServiceScope scope, ModuleBase module);
}