/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.Security.Identity;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Identity.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Hosting;
using Corsinvest.AppHero.Core.UI;
using Microsoft.AspNetCore.Http;

namespace Corsinvest.AppHero.Core.Security;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Security,User,Role,Permissions";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Security";
        Slug = "Security";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.AdminSecurity.GetName(),
        };

        Link.Child = new List<ModuleLink>()
        {
            new (Link, "Users", "Users")
            {
                Icon = UIIcon.Users.GetName()
            },
            new (Link, "Roles", "Roles")
            {
                Icon = UIIcon.Roles.GetName()
            },
            new(Link, "Profile", "Profile")
            {
                Icon = UIIcon.Profile.GetName()
            }
        };

        Roles = new Role[]
        {
            new("",
                "",
                Permissions.Role.Data.Permissions
                .Union(new[]
                {
                    Permissions.Role.ViewPermissions,
                    Permissions.Role.ManagePermissions
                })
                .Union(Permissions.User.Data.Permissions)
                .Union(new[]
                {
                    Permissions.User.ManageRoles,
                    Permissions.User.ResetPassword
                }))
        };
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        AddOptions<Identity.Options>(services, config);

        //security validator
        services.AddScoped<IValidator<ApplicationUser>, ApplicationUserValidator>()
                .AddScoped<IValidator<ApplicationRole>, ApplicationRoleValidator>();

        //Authentication
        services.AddTransient<IAuthenticationService, AuthenticationService>()
                .AddScoped<AuthenticationService>()
                .AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationService>());

        services.AddScoped<IPermissionService, PermissionService>();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/auth/user/signin";
            //options.LogoutPath = "/";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(services.GetOptionsSnapshot<Identity.Options>().Value.LoginCookieExpirationHours);
        });

        services.AddAuthentication();

        //Authorization
        //services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthorization(options =>
        {
            //create all policy from permissions
            var modularityService = services.GetRequiredService<IModularityService>();
            foreach (var item in modularityService.Modules.SelectMany(a => a.GetPermissions()))
            {
                options.AddPolicy(item.Key, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, item.Key));
            }
        });
    }

    public override async Task OnPreApplicationInitializationAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        await services.AppHeroPopulateSecurityAsync();
    }

    public override async Task OnApplicationInitializationAsync(IHost host)
    {
        var app = (WebApplication)host;
        app.UseAuthentication();
        app.UseAuthorization();

        await Task.CompletedTask;
    }

    public override async Task OnPostApplicationInitializationAsync(IHost host)
    {
        var app = (WebApplication)host;
        app.MapControllers();
        app.MapRazorPages();

        await Task.CompletedTask;
    }

    public class Permissions
    {
        public class Role
        {
            public static PermissionsCrud Data { get; } = new($"{typeof(Options.Module).FullName}.{nameof(Role)}.{nameof(Data)}");
            public static Permission ManagePermissions { get; } = new($"{Data.Prefix}.{nameof(ManagePermissions)}", "Manage Permissions", UIIcon.ManagePermissions.GetName());
            public static Permission ViewPermissions { get; } = new($"{Data.Prefix}.{nameof(ViewPermissions)}", "View Permissions", UIIcon.ViewPermissions.GetName());
        }

        public class User
        {
            public static PermissionsCrud Data { get; } = new($"{typeof(Options.Module).FullName}.{nameof(User)}.{nameof(Data)}");
            public static Permission ManageRoles { get; } = new($"{Data.Prefix}.{nameof(ManageRoles)}", "ManageRoles", UIIcon.ManageRoles.GetName());
            public static Permission ResetPassword { get; } = new($"{Data.Prefix}.{nameof(ResetPassword)}", "RestPassword", UIIcon.RestPassword.GetName());
        }
    }
}
