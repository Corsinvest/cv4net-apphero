/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Auditing.Persistence.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Corsinvest.AppHero.Auditing;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Persistence";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Auditing";
        Slug = "Auditing";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.PersonSearch.GetName(),
        };

        Roles = new Role[]
        {
            new("","", Permissions.Data.Permissions)
        };
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) 
        => services.AddScoped<AuditableEntitySaveChangesInterceptor>();

    public static class Permissions
    {
        public static PermissionsRead Data { get; } = new($"{typeof(Module).FullName}.{nameof(Data)}");
    }
}