/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Modularity.Packages;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using FluentResults;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Reflection;
using System.Text;

namespace Corsinvest.AppHero.Core.Modularity;

public class ModularityService : IModularityService
{
    private readonly ILogger<ModularityService> _logger;

    public ModularityService(IServiceCollection services, IEnumerable<ModuleBase> modules)
    {
        _logger = services.GetRequiredService<ILoggerFactory>().CreateLogger<ModularityService>();
        Modules = modules;
        Initialize();
    }

    private void Initialize()
    {
        SetCategoryIcon(IModularityService.AdministrationCategoryName, UIIcon.SettingsApplications.GetName());
        SetCategoryIcon(IModularityService.GeneralCategoryName, UIIcon.Folder.GetName());
    }

    public IEnumerable<ModuleBase> Modules { get; private set; }

    public ModuleBase? Get<T>() where T : ModuleBase
        => Modules.FirstOrDefault(a => a.GetType() == typeof(T));

    public ModuleBase? GetByClass(string @class) => Modules.FirstOrDefault(a => a.Class == @class);
    public ModuleBase? GetBySlug(string slug) => Modules.FirstOrDefault(a => a.Slug == slug);

    public ModuleBase? GetByUrl(string url)
    {
        ModuleBase? module = null;
        var path = new Uri(url).AbsolutePath;
        if (path.StartsWith(ApplicationHelper.ModuleComponentUrl, StringComparison.InvariantCultureIgnoreCase))
        {
            var data = path.Split("/");
            module = data.Length > 2
                        ? GetBySlug(data[2])
                        : null;

        }
        return module;
    }

    public async Task<IEnumerable<ModuleAuthorization>> GetAuthorizationsAsync(IPermissionService permissionService)
    {
        var ret = new List<ModuleAuthorization>();
        foreach (var module in Modules.IsEnabled())
        {
            ret.Add(await module.GetAuthorizationAsync(permissionService));
        }
        return ret;
    }

    public async Task<IEnumerable<ModuleLink>> GetAuthorizedLinksAsync(IPermissionService permissionService)
        => (await GetAuthorizationsAsync(permissionService)).SelectMany(a => a.Links);

    public async Task<IEnumerable<ModuleMenuItem>> GetAuthorizedMenuItemssAsync(IPermissionService permissionService)
        => (await GetAuthorizationsAsync(permissionService)).SelectMany(a => a.MenuItems);

    public IEnumerable<string> GetCategories() => Modules.Select(a => a.Category).Distinct();

    private readonly Dictionary<string, string> _cateoryIcons = new();
    public string? GetCategoryIcon(string category)
        => _cateoryIcons.TryGetValue(category, out var icon)
                ? icon
                : null;

    public void SetCategoryIcon(string category, string icon)
    {
        if (!_cateoryIcons.TryAdd(category, icon)) { _cateoryIcons[category] = icon; }
    }

    public IEnumerable<Assembly> Assemblies
        => Modules.Select(a => a.GetType().Assembly)
                  .Where(a => a != GetType().Assembly)
                  .Distinct();

    public async Task RefreshOptionsAsync(IServiceScope scope)
    {
        foreach (var module in Modules)
        {
            _logger.LogInformation("Refresh Options module: {FullInfo}", module.FullInfo);
            await module.RefreshOptionsAsync(scope);
        }
    }

    public async Task<IResult<IEnumerable<PackageDto>>> GetPackagesAsync(PackagesOptions packagesOptions)
    {
        var errors = new List<string>();
        var ret = new List<PackageDto>();
        foreach (var source in packagesOptions.Sources)
        {
            foreach (var item in source.Packages)
            {
                try
                {
                    //decode packageid!version  
                    //     The version string is either a simple version or an arithmetic range e.g. 1.0
                    //     --> 1.0 ≤ x (,1.0] --> x ≤ 1.0 (,1.0) --> x < 1.0 [1.0] --> x == 1.0 (1.0,) -->
                    //     1.0 < x (1.0, 2.0) --> 1.0 < x < 2.0 [1.0, 2.0] --> 1.0 ≤ x ≤ 2.0

                    var idAndVer = item.Split("|");
                    var id = idAndVer[0];
                    var version = idAndVer.Length == 2
                                    ? idAndVer[1]
                                    : string.Empty;

                    var result = await SearchAsync(source, id);
                    if (result != null)
                    {
                        var versions = (await result.GetVersionsAsync())
                                        .Select(a => a.Version.Version)
                                        .OrderByDescending(a => a)
                                        .ToList();

                        if (!string.IsNullOrEmpty(version))
                        {
                            var versionRange = NuGet.Versioning.VersionRange.Parse(version);

                            versions = versions.Where(a => a >= versionRange.MinVersion.Version
                                                        && a <= versionRange.MaxVersion.Version)
                                               .ToList();
                        }

                        var data = new PackageDto
                        {
                            Id = result.Identity.Id,
                            IconUrl = result.IconUrl + "",
                            Versions = versions,
                            Feed = $"{source.Name} - {source.Feed}",
                            Owners = result.Owners,
                            DownloadCount = result.DownloadCount ?? 0,
                            IsInstallated = packagesOptions.Packages.Any(a => a.Id == result.Identity.Id
                                                                                && a.IsNuGetPackage
                                                                                && a.Version == result.Identity.Version.Version),
                        };

                        data.CurrentVersion = data.IsInstallated
                                                ? data.Versions.First(a => a == result.Identity.Version.Version)
                                                : data.Versions.First();

                        data.Url = string.IsNullOrWhiteSpace(result.ProjectUrl + "")
                                    ? "#"
                                    : result.ProjectUrl.ToString();

                        ret.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    var message = new StringBuilder();
                    var exTmp = ex;
                    while (exTmp != null)
                    {
                        if (message.ToString().Length > 0) { message.Append("<br/>"); }
                        message.Append(exTmp.Message);
                        exTmp = exTmp.InnerException;
                    }
                    errors.Add(message.ToString());
                    break;
                }
            }
        }

        return Result.Ok(ret.OrderBy(a => a.IsInstallated)).WithErrors(errors);
    }

    private static async Task<IPackageSearchMetadata?> SearchAsync(PackageSourceOptions source, string packageId)
    {
        var sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(source.Feed)
        {
            Credentials = new PackageSourceCredential(source.Feed,
                                                      source.Username,
                                                      source.Password,
                                                      true,
                                                      null)
        });

        var packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();

        var data = await packageSearchResource.SearchAsync(packageId,
                                                           new SearchFilter(includePrerelease: true),
                                                           skip: 0,
                                                           take: 32,
                                                           NuGet.Common.NullLogger.Instance,
                                                           CancellationToken.None);

        return data.FirstOrDefault(a => a.Identity.Id == packageId);
    }
}