/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity.Packages;
using Microsoft.Extensions.Hosting;
using Weikio.NugetDownloader;
using Weikio.PluginFramework.Abstractions;
using Weikio.PluginFramework.Catalogs;
using Weikio.PluginFramework.Catalogs.NuGet;

namespace Corsinvest.AppHero.Core.Modularity;

public static class ServiceCollectionExtensions
{
    public static async Task OnPreApplicationInitializationModulesAsync(this IHost host) => await OnApplicationInitializationAsync(host, 0);
    public static async Task OnApplicationInitializationModulesAsync(this IHost host) => await OnApplicationInitializationAsync(host, 1);
    public static async Task OnPostApplicationInitializationModulesAsync(this IHost host) => await OnApplicationInitializationAsync(host, 2);

    private static async Task OnApplicationInitializationAsync(IHost host, int index)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ServiceCollectionExtensions));

        var steps = new[] { "Pre", "Normal", "Post" };

        foreach (var module in services.GetRequiredService<IModularityService>().Modules.OrderBy(a => a.ForceLoad))
        {
            logger.LogInformation("Configure {step} module: {module}", steps[index], module.FullInfo);
            switch (index)
            {
                case 0: await module.OnPreApplicationInitializationAsync(host); break;
                case 1: await module.OnApplicationInitializationAsync(host); break;
                case 2: await module.OnPostApplicationInitializationAsync(host); break;
                default: break;
            }
        }
    }

    public static IServiceCollection AddAppHeroModules(this IServiceCollection services, IConfiguration config)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ServiceCollectionExtensions));

        using (logger.LogTimeOperation(LogLevel.Information, true, "Load Plugins..."))
        {
            var plugins = new List<Plugin>();
            var path = ApplicationHelper.PathData;

            //initialize plugins
            Task.Run(async () =>
            {
                var pathPlugins = Path.Combine(path, "plugins");
                Directory.CreateDirectory(pathPlugins);

                var pathPluginsFolder = Path.Combine(pathPlugins, "folder");
                Directory.CreateDirectory(pathPluginsFolder);

                var pathPluginsNuGet = Path.Combine(pathPlugins, "nuget");
                Directory.CreateDirectory(pathPluginsNuGet);

                //in folder execution
                plugins.AddRange(await LoadPluginFolderAsync(logger, AppContext.BaseDirectory, false));

                //in folder 'plugins'
                foreach (var item in Directory.GetDirectories(pathPluginsFolder))
                {
                    plugins.AddRange(await LoadPluginFolderAsync(logger, item, true));
                }

                //NuGet
                plugins.AddRange(await LoadPluginNuGetAsync(services, logger, pathPluginsNuGet));

            }).Wait();

            using (logger.LogTimeOperation(LogLevel.Information, true, "Initialize Modules.."))
            {
                Initialize(services, config, logger, plugins);
            }
        }
        return services;
    }

    private static async Task<List<Plugin>> LoadPluginNuGetAsync(IServiceCollection services, ILogger logger, string path)
    {
        //load form NuGet
        var packagesOptions = services.GetOptionsSnapshot<PackagesOptions>().Value;
        var plugins = new List<Plugin>();

        foreach (var item in packagesOptions.Packages.Where(a => a.IsNuGetPackage))
        {
            var feed = packagesOptions.Sources.FirstOrDefault(a => a.Packages.Contains(item.Id));
            if (feed != null)
            {
                using (logger.LogTimeOperation(LogLevel.Information,
                                               false,
                                               "Add Plugin from NuGet {name} {version}",
                                               item.Id,
                                               item.Version.ToString()))
                {
                    var plugin = new NugetPackagePluginCatalog(item.Id,
                                                               packageVersion: item.Version.ToString(),
                                                               includePrerelease: true,
                                                               packageFeed: new NuGetFeed(feed.Name, feed.Feed)
                                                               {
                                                                   Username = feed.Username,
                                                                   Password = feed.Password
                                                               },
                                                               packagesFolder: Path.Combine(path, feed.Name, item.Id, item.Version.ToString()),
                                                               configureFinder: finder => finder.Implements<ModuleBase>(),
                                                               options: new NugetPluginCatalogOptions()
                                                               {
                                                                   IncludeSystemFeedsAsSecondary = true
                                                               });

                    try
                    {
                        await plugin.Initialize();
                        plugins.AddRange(plugin.GetPlugins());
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                    }
                }
            }
        }

        return plugins;
    }

    private static async Task<List<Plugin>> LoadPluginFolderAsync(ILogger logger, string folder, bool includeSubfolders)
    {
        using (logger.LogTimeOperation(LogLevel.Information, false, "Add Plugin from Folder {folder}", folder))
        {
            var plugin = new FolderPluginCatalog(folder,
                                                 configureFinder: finder => finder.Implements<ModuleBase>(),
                                                 options: new FolderPluginCatalogOptions
                                                 {
                                                     IncludeSubfolders = includeSubfolders
                                                 });
            try
            {
                await plugin.Initialize();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return plugin.GetPlugins();
        }
    }

    private static bool IsEnabledModule(PackagesOptions packagesOptions, ModuleBase module)
        => packagesOptions.Packages.Any(a => IsEnabledModule(a, module));

    private static bool IsEnabledModule(PackageOptions packageOptions, ModuleBase module)
        => packageOptions.Modules.FirstOrDefault(a => a.Class == module.Class)?.Enabled ?? false;

    private static void Initialize(IServiceCollection services,
                                   IConfiguration config,
                                   ILogger logger,
                                   IEnumerable<Plugin> plugins)
    {
        var writablePackagesOptions = services.GetWritableOptions<PackagesOptions>();
        var packagesOptions = writablePackagesOptions.Value;

        var modules = new List<ModuleBase>();

        var serviceProvider = services.GetRequiredService<IServiceProvider>();

        foreach (var item in plugins)
        {
            //TODO
            services.AddControllers().AddApplicationPart(item.Assembly);

            var module = (ModuleBase)ActivatorUtilities.CreateInstance(serviceProvider, item);
            modules.Add(module);

            logger.LogInformation("Module {FullInfo}", module.FullInfo);

            Directory.CreateDirectory(module.PathData);

            module.Enabled = module.ForceLoad || IsEnabledModule(packagesOptions, module);

            logger.LogDebug("Configure module: {FullInfo}", module.FullInfo);
            module.ConfigureServices(services, config);

            CreateOptions(module, packagesOptions);
        }

        writablePackagesOptions.Update(packagesOptions);

        services.AddSingleton<IModularityService>(new ModularityService(services, modules));
    }

    private static void CreateOptions(ModuleBase module, PackagesOptions packagesOptions)
    {
        if (!module.ForceLoad)
        {
            var assemblyName = module.GetType().Assembly.GetName();

            var package = packagesOptions.Packages.FirstOrDefault(a => a.Id == assemblyName.Name);
            if (package == null)
            {
                package = new()
                {
                    Id = assemblyName.Name!,
                    Version = assemblyName.Version!,
                };
                packagesOptions.Packages.Add(package);
            }

            //find module in package
            var moduleInPackage = package.Modules.FirstOrDefault(a => a.Class == module.Class);
            if (moduleInPackage == null)
            {
                moduleInPackage = new()
                {
                    Class = module.Class,
                    Enabled = module.Enabled
                };
                package.Modules.Add(moduleInPackage);
            }
        }
    }
}
