/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Domain.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository<TContext, TEntity>(this IServiceCollection services)
        => services.AddRepository(typeof(TContext), typeof(TEntity));

    public static IServiceCollection AddRepository(this IServiceCollection services, Type dbContext, Type entity)
        => services.AddScoped(typeof(IReadRepository<>).MakeGenericType([entity]),
                              sp => sp.GetRequiredService(typeof(IRepository<>).MakeGenericType([entity])))
                   .AddScoped(typeof(IRepository<>).MakeGenericType([entity]),
                              typeof(BaseDbRepository<,>).MakeGenericType([dbContext, entity]));

    public static IServiceCollection AddRepositories(this IServiceCollection services, Type dbContext, IEnumerable<Type> entities)
    {
        foreach (var item in entities) { services.AddRepository(dbContext, item); }
        return services;
    }

    public static IServiceCollection AddRepositories<TContext>(this IServiceCollection services, IEnumerable<Type> entities)
        => services.AddRepositories(typeof(TContext), entities);
}
