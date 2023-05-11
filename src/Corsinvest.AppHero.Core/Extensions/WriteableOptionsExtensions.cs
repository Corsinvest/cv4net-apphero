/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Extensions;

public static class WriteableOptionsExtensions
{
    public static IServiceCollection AddOptions<T>(this IServiceCollection services,
                                                   IConfiguration config,
                                                   string section,
                                                   string filename) where T : class, new()
    {
        services.Configure<T>(config.GetSection(section));
        services.AddTransient<IWritableOptionsService<T>>(provider =>
        {
            return new WritableOptionsService<T>(provider.GetService<IOptionsMonitor<T>>()!,
                                                 (IConfigurationRoot)provider.GetService<IConfiguration>()!,
                                                 section,
                                                 filename);
        });

        //create section if not exists
        services.GetWritableOptions<T>().Update(services.GetOptionsSnapshot<T>().Value);

        return services;
    }

    public static IServiceCollection AddOptions<T>(this IServiceCollection services, IConfiguration config) where T : class, new()
        => services.AddOptions<T>(config, typeof(T).FullName!);

    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services,
                                                          IConfiguration config,
                                                          string section) where TOptions : class, new()
        => services.AddOptions<TOptions>(config, section, ApplicationHelper.FileNameOptions);
}