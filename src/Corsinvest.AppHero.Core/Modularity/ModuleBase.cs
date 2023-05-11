/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Constants;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components;

namespace Corsinvest.AppHero.Core.Modularity;

public abstract class ModuleBase
{
    public string PathData => Path.Combine(ApplicationHelper.PathModules, Class);

    public ModuleLink? Link { get; set; }
    public IEnumerable<ModuleLink> GetFlatLinks() => Link?.GetFlatLinks() ?? new List<ModuleLink>().AsReadOnly();

    public IEnumerable<ModuleMenuItem> MenuItems { get; set; } = Enumerable.Empty<ModuleMenuItem>();

    public string? InfoText { get; set; }

    private string _icon = default!;
    public string Icon
    {
        //if not set use icon link if set
        get => string.IsNullOrWhiteSpace(_icon) && Link != null
                    ? Link.Icon
                    : _icon;

        set => _icon = value;
    }

    public virtual string UrlHelp { get; set; } = ApplicationHelper.HelpUrl;

    private string _slug = default!;
    public string Slug
    {
        //if not set use Class
        get => string.IsNullOrWhiteSpace(_slug) || string.IsNullOrEmpty(_slug)
                    ? Class.Replace(".", "")
                    : _slug;

        set => _slug = value;
    }

    public string Description { get; init; } = default!;
    public ModuleType Type { get; init; } = ModuleType.Application;
    public string Category { get; set; } = IModularityService.GeneralCategoryName;
    public string FullInfo => $"{Class}@{Version}";
    public string Keywords { get; init; } = string.Empty;
    public string Authors { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    public virtual Version Version => GetType().Assembly.GetName().Version!;

    #region Authorization
    protected IEnumerable<Role> Roles { get; set; } = Array.Empty<Role>();
    public string PermissionLinkBaseKey => $"{Class}.{ActionConstants.Link}";
    public string PermissionWidgetBaseKey => $"{Class}.{ActionConstants.Widget}";
    public Permission PermissionEditOptions => new($"{Class}.{ActionConstants.EditOptions}", "Edit Options", UIIcon.Settings.GetName());

    public string RoleAdminKey => $"{Class}.{RoleConstants.AdministratorRole}";
    public string RoleAdminDescription => $"Admin Module {Description}";

    public async Task<ModuleAuthorization> GetAuthorizationAsync(IPermissionService permissionService)
        => await ModuleAuthorization.GetInstanceAsync(permissionService, this);

    public IEnumerable<Permission> GetPermissions() => GetAllRoles().SelectMany(a => a.Permissions).Distinct();

    private IEnumerable<Role> _roles = default!;
    public IEnumerable<Role> GetAllRoles()
    {
        if (_roles == null)
        {
            var roles = new List<Role>();

            //options
            if (HasOptions) { roles.Add(new(RoleAdminKey, RoleAdminDescription, new[] { PermissionEditOptions })); }

            //links
            roles.AddRange(GetFlatLinks().Where(a => !a.Child.Any())
                                         .Select(a => new Role(RoleAdminKey, RoleAdminDescription, new[] { a.Permission })));

            //menu items
            roles.AddRange(MenuItems.Select(a => new Role(RoleAdminKey, RoleAdminDescription, new[] { a.Permission })));

            //widgets
            roles.AddRange(Widgets.Select(a => new Role(RoleAdminKey, RoleAdminDescription, new[] { a.Permission })));

            roles.AddRange(Roles);
            _roles = roles.AsReadOnly();
        }
        return _roles;
    }
    #endregion

    public IEnumerable<ModuleWidget> Widgets { get; init; } = Array.Empty<ModuleWidget>();
    public bool Enabled { get; set; }
    public string BaseUrl => $"{ApplicationHelper.ModuleComponentUrl}/{Slug}";
    public bool ForceLoad => this is IForceLoadModule;

    #region Configure
    public virtual async Task ConfigureServicesAsync(IServiceCollection services, IConfiguration config) => await Task.CompletedTask;
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration config) { }

    public virtual async Task OnPreApplicationInitializationAsync(IHost host) => await Task.CompletedTask;
    public virtual async Task OnApplicationInitializationAsync(IHost host) => await Task.CompletedTask;
    public virtual async Task OnPostApplicationInitializationAsync(IHost host) => await Task.CompletedTask;

    //public virtual async Task ShutdownAsync() => await Task.CompletedTask;
    public virtual async Task RefreshOptionsAsync(IServiceScope scope) => await Task.CompletedTask;


    protected IServiceCollection AddOptions<TType>(IServiceCollection services, IConfiguration config, string? section = null) where TType : class, new()
    {
        Options = new()
        {
            Type = typeof(TType)
        };

        services.AddOptions<TType>(config,
                                   string.IsNullOrWhiteSpace(section)
                                        ? typeof(TType).FullName!
                                        : section);

        return services;
    }

    protected IServiceCollection AddOptions<TType, TRender>(IServiceCollection services, IConfiguration config) where TType : class, new()
    {
        AddOptions<TType>(services, config);
        Options!.Render = typeof(TRender);
        return services;
    }

    public ModuleOptions? Options { get; private set; }
    public bool HasOptions => Options != null;
    #endregion

    public string Class => GetType().FullName!;
    public virtual bool Configurated => true;

    public bool Search(string value)
        => Class.Contains(value, StringComparison.InvariantCultureIgnoreCase)
           || Description.Contains(value, StringComparison.InvariantCultureIgnoreCase)
           || Keywords.Split(",").Any(a => a.Contains(value, StringComparison.InvariantCultureIgnoreCase));

    public void SetRenderIndex<TIndex>() where TIndex : ComponentBase
        => Link!.Render = typeof(TIndex);

    public void SetRenderOptions<TRenderOptions>() where TRenderOptions : ComponentBase
        => Options!.Render = typeof(TRenderOptions);

}