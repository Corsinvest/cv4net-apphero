/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Entities;
using Corsinvest.AppHero.Core.Domain.Repository;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Notification;

namespace Corsinvest.AppHero.Core.Extensions;

public static class ServiceScopeExtensions
{
    public static ILoggerFactory GetLoggerFactory(this IServiceScope scope) => scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    public static AppOptions GetAppOptions(this IServiceScope scope) => scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;
    public static IModularityService GetModularityService(this IServiceScope scope) => scope.ServiceProvider.GetRequiredService<IModularityService>();
    public static INotificationService GetNotificationService(this IServiceScope scope) => scope.ServiceProvider.GetRequiredService<INotificationService>();

    public static ModuleBase? GetModule<T>(this IServiceScope scope) where T : ModuleBase
        => scope.GetModularityService().Get<T>();

    public static IReadRepository<T> GetReadRepository<T>(this IServiceScope scope) where T : class, IAggregateRoot
        => scope.ServiceProvider.GetRequiredService<IReadRepository<T>>();

    public static IRepository<T> GetRepository<T>(this IServiceScope scope) where T : class, IAggregateRoot
        => scope.ServiceProvider.GetRequiredService<IRepository<T>>();

    public static IOptionsSnapshot<T> GetOptionsSnapshot<T>(this IServiceScope scope) where T : class, new()
        => scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<T>>();
}