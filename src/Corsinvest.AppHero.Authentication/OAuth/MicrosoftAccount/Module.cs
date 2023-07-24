/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Authentication.OAuth.Controllers;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.UI;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.Authentication.OAuth.MicrosoftAccount;

public class Module : ModuleBase, IForceLoadModule, IAuthentication
{
    private bool _configured;

    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Autentication,MicrosoftAccount";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "OAuth Microsoft Account";

        Link = new ModuleLink(this, Description, OAuthController.MakeUrlChallenge(MicrosoftAccountDefaults.AuthenticationScheme), true)
        {
            Icon = UIIcon.MicrosoftAzure.GetName(),
            Enabled = false
        };
    }

    public AuthenticationType AuthenticationType => AuthenticationType.External;
    public override bool Configured => _configured;
    static bool IsConfigurated(Options options) => !string.IsNullOrEmpty(options.ClientId) && !string.IsNullOrEmpty(options.ClientSecret);

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        AddOptions<Options>(services, config);
        var options = services.GetOptionsSnapshot<Options>().Value;

        // https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app
        _configured = IsConfigurated(options);
        if (_configured)
        {
            services.AddAuthentication().AddMicrosoftAccount(a =>
            {
                a.SignInScheme = IdentityConstants.ExternalScheme;
                a.ClientId = options.ClientId;
                a.ClientSecret = options.ClientSecret;

                options.MapCustomJson(a.ClaimActions);
            });
        }
    }
}