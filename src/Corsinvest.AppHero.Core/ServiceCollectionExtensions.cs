/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Modularity.Packages;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Corsinvest.AppHero.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppHero(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddOptions<AppOptions>(config, nameof(AppOptions));
        services.AddOptions<PackagesOptions>(config, nameof(PackagesOptions));

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ServiceCollectionExtensions));

        services.AddDistributedMemoryCache()
                //.AddMvc()
                //.Services
                //.AddControllers().AddApplicationPart(typeof(ServiceCollectionExtensions).Assembly)
                //.Services

                .AddAppHeroModules(config)

                .AddAppHeroServices(typeof(ITransientDependency), ServiceLifetime.Transient, logger)
                .AddAppHeroServices(typeof(IScopedDependency), ServiceLifetime.Scoped, logger)
                .AddAppHeroServices(typeof(ISingletonDependency), ServiceLifetime.Singleton, logger)

                .AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()
                //.AddDetection()
                ;

        return services;
    }

    public static async Task OnPreApplicationInitializationAsync(this WebApplication app)
    {
        //app.UseDetection();

        await app.OnPreApplicationInitializationModulesAsync();
    }

    public static async Task OnApplicationInitializationAsync(this WebApplication app)
    {
        #region Static files
        app.UseStaticFiles();

        var pathFiles = Path.Combine(ApplicationHelper.PathData, "files");
        if (!Directory.Exists(pathFiles)) { Directory.CreateDirectory(pathFiles); }
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(pathFiles),
            RequestPath = new PathString("/files")
        });
        #endregion

        await app.OnApplicationInitializationModulesAsync();
    }

    public static async Task OnPostApplicationInitializationAsync(this WebApplication app) => await app.OnPostApplicationInitializationModulesAsync();
}