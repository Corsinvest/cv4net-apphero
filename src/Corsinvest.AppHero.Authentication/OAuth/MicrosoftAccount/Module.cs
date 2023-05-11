/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Corsinvest.AppHero.Authentication.OAuth.Controllers;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Authentication.OAuth.MicrosoftAccount;

public class Module : ModuleBase, IForceLoadModule, IAutenticationConfig, IAutentication
{
    private bool _configurated;

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

    public AutenticationType AutenticationType => AutenticationType.External;
    public override bool Configurated => _configurated;
    static bool IsConfigurated(Options options) => !string.IsNullOrEmpty(options.ClientId) && !string.IsNullOrEmpty(options.ClientSecret);

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        AddOptions<Options>(services, config);
        var options = services.GetOptionsSnapshot<Options>().Value;

        // https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app
        _configurated = IsConfigurated(options);
        if (_configurated)
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