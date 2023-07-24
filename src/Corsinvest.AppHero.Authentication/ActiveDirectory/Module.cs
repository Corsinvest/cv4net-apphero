/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.Authentication.ActiveDirectory;

public class Module : ModuleBase, IForceLoadModule, IAuthentication
{
    private IServiceCollection _services = default!;

    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Autentication,ActiveDirectory";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Authentication Active Directory";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.MicrosoftWindows.GetName(),
            Enabled = false,
        };
    }

    public AuthenticationType AuthenticationType => AuthenticationType.Inline;
    public override bool Configured => _services.GetOptionsSnapshot<Options>().Value.Domains.Any(a => a.Enabled);

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        _services = services;
        AddOptions<Options>(services, config);

        services.AddScoped<IAuthenticationActiveDirectory, AuthenticationActiveDirectory>();
    }
}