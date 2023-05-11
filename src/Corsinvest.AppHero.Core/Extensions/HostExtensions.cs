/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Core.Extensions;

public static class HostExtensions
{
    public static IServiceScope GetScopeFactory(this IHost host) => host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    public static IConfiguration GetConfiguration(this IHost host) => host.Services.GetRequiredService<IConfiguration>();

    public static void DatabaseMigrate<T>(this IHost host) where T : DbContext
    {
        using var context = host.GetScopeFactory().ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}