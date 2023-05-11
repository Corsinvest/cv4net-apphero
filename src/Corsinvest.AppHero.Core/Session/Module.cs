/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Corsinvest.AppHero.Core.Session;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Session";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Application;
        Description = "Sessions";
        Slug = "Sessions";
        InfoText = "Check active user sessions";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.ActiveUser.GetName()
        };


        Roles = new Role[]
        {
            new("",
                "",
                Permissions.Data.Permissions
                .Union(new[]
                {
                    Permissions.Logout
                }))
        };
    }

    public class Permissions
    {
        public static PermissionsRead Data { get; } = new($"{typeof(Module).FullName}.{nameof(Data)}");
        public static Permission Logout { get; } = new($"{Data.Prefix}.{nameof(Logout)}", "Logout", UIIcon.Logout.GetName());
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<CircuitHandler, SessionInfoCircuitHandler>();
        services.AddSingleton(provider => (ISessionsInfoTracker)provider.GetService<CircuitHandler>()!);
    }
}