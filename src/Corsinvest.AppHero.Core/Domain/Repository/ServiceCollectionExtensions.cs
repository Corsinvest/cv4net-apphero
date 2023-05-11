/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Domain.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository<TContext, TEnity>(this IServiceCollection services)
        => services.AddRepository(typeof(TContext), typeof(TEnity));

    public static IServiceCollection AddRepository(this IServiceCollection services, Type dbContext, Type entity)
        => services.AddScoped(typeof(IReadRepository<>).MakeGenericType(new[] { entity }),
                              sp => sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(new[] { entity })))
                   .AddScoped(typeof(IRepository<>).MakeGenericType(new[] { entity }),
                              typeof(BaseDbRepository<,>).MakeGenericType(new[] { dbContext, entity }));

    public static IServiceCollection AddRepositories(this IServiceCollection services, Type dbContext, IEnumerable<Type> entities)
    {
        foreach (var item in entities) { services.AddRepository(dbContext, item); }
        return services;
    }

    public static IServiceCollection AddRepositories<TContext>(this IServiceCollection services, IEnumerable<Type> entities)
        => services.AddRepositories(typeof(TContext), entities);
}
