/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity.Packages;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using FluentResults;
using System.Reflection;

namespace Corsinvest.AppHero.Core.Modularity;

public interface IModularityService
{
    public static string AdministrationCategoryName { get; } = "Administration";
    public static string GeneralCategoryName { get; } = "General";
    IEnumerable<ModuleBase> Modules { get; }
    Task<IEnumerable<ModuleAuthorization>> GetAuthorizationsAsync(IPermissionService permissionService);
    ModuleBase? GetByClass(string @class);
    ModuleBase? GetBySlug(string slug);
    ModuleBase? GetByUrl(string url);
    ModuleBase? Get<T>() where T : ModuleBase;
    List<ModuleCategory> Categories { get; }
    IEnumerable<Assembly> Assemblies { get; }
    Task RefreshOptionsAsync(IServiceScope scope);
    Task<IEnumerable<ModuleLink>> GetAuthorizedLinksAsync(IPermissionService permissionService);
    Task<IEnumerable<ModuleMenuItem>> GetAuthorizedMenuItemssAsync(IPermissionService permissionService);
    Task<IResult<IEnumerable<PackageDto>>> GetPackagesAsync(PackagesOptions packagesOptions);
}