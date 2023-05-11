/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Extensions;

public static class ServiceExtensions
{
    public static IWritableOptionsService<T> GetWritableOptions<T>(this IServiceCollection services) where T : class, new()
        => services.GetRequiredService<IWritableOptionsService<T>>();

    public static IOptionsSnapshot<T> GetOptionsSnapshot<T>(this IServiceCollection services) where T : class, new()
        => services.GetRequiredService<IOptionsSnapshot<T>>();

    public static IModularityService GetModularityService(this IServiceCollection services)
        => services.GetRequiredService<IModularityService>();

    public static T GetRequiredService<T>(this IServiceCollection services) where T : notnull
        => services.BuildServiceProvider().GetRequiredService<T>();

    public static T? GetService<T>(this IServiceCollection services) where T : notnull
        => services.BuildServiceProvider().GetService<T>();

    public static IServiceCollection AddAppHeroServices(this IServiceCollection services,
                                                        Type interfaceType,
                                                        ServiceLifetime lifetime,
                                                        ILogger logger)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
                                           .SelectMany(s => s.GetTypes())
                                           .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                                           .Select(t => new
                                           {
                                               Service = t.GetInterfaces().FirstOrDefault(),
                                               Implementation = t
                                           })
                                           .Where(t => t.Service is not null && interfaceType.IsAssignableFrom(t.Service));

        foreach (var item in types)
        {
            logger.LogInformation("Automatic load service {ServiceName} implementation {ImplementationName} lifetime {lifetime}",
                                  item.Service!.Name,
                                  item.Implementation.Name,
                                  lifetime);
            services.AddService(item.Service!, item.Implementation, lifetime);
        }
        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection services,
                                                Type serviceType,
                                                Type implementationType,
                                                ServiceLifetime lifetime)
        => lifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
        };
}
