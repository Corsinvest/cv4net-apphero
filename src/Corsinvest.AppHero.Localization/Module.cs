/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Repository;
using Corsinvest.AppHero.Core.Helpers;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Localization.Types.Database.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace Corsinvest.AppHero.Localization;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Localization,Translate,Culture,Language";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Localization";
        Slug = "Localization";
        InfoText = "Localize the text with your native language";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.Translate.GetName(),
        };

        Roles = new Role[]
        {
            new("", "", Permissions.Data.Permissions.Union(new []
            {
                Permissions.Translate
            }))
        };
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        AddOptions<Options>(services, config);
        var options = services.GetOptionsSnapshot<Options>().Value;

        //type
        switch (options.Type)
        {
            case TypeArchive.Json:
                services.AddSingleton<IStringLocalizerFactory, Types.Json.LocalizerFactory>();
                break;

            //case TypeArchive.PortableObject: 
            //    services.AddSingleton<IStringLocalizerFactory, Types.PortableObject.LocalizerFactory>();
            //    services.AddPortableObjectLocalization(options => options.ResourcesPath = settings.ResourcesPath);
            //    break;

            case TypeArchive.DataBase:
                services.AddDbContext<LocalizationDbContext>(optionsAction => optionsAction.UseSqlite(DataBaseHelper.CreateSQLitePath(Path.Combine(PathData, "db.db"))),
                                                             ServiceLifetime.Transient,
                                                             ServiceLifetime.Transient)
                        .AddRepository<LocalizationDbContext, Types.Database.Models.Localization>()
                        .AddSingleton<IStringLocalizerFactory, Types.Database.LocalizerFactory>();
                break;

            case TypeArchive.ResX: break;

            default: throw new IndexOutOfRangeException("options.Type");
        }

        services.Configure<RequestLocalizationOptions>(configureOptions =>
        {
            var supportedCultures = options.SupportedCultures.Select(a => new CultureInfo(a)).ToList();
            if (!supportedCultures.Any()) { throw new InvalidOperationException("SupportedCultures is not configured."); }
            if (string.IsNullOrWhiteSpace(options.DefaultCulture)) { throw new InvalidOperationException("DefaultCulture is not configured."); }

            configureOptions.SupportedCultures = supportedCultures;
            configureOptions.SetDefaultCulture(options.DefaultCulture);
            configureOptions.FallBackToParentUICultures = true;
        })
        .AddLocalization();
    }

    public override async Task OnApplicationInitializationAsync(IHost host)
    {
        var app = (WebApplication)host;
        await Task.CompletedTask;

        var options = host.GetScopeFactory().GetOptionsSnapshot<Options>().Value;
        if (options.Type == TypeArchive.DataBase)
        {
            using var context = app.GetScopeFactory().ServiceProvider.GetRequiredService<LocalizationDbContext>();
            context.Database.Migrate();
        }

        app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

    }

    public static class Permissions
    {
        public static PermissionsCrud Data { get; } = new($"{typeof(Module).FullName}.{nameof(Data)}");
        public static Permission Translate { get; } = new($"{Data.Prefix}.{nameof(Translate)}", "Translate", UIIcon.Translate.GetName(), UIColor.Info);
    }
}