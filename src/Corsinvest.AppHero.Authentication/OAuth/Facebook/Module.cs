/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Authentication.OAuth.Controllers;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Identity;
using Corsinvest.AppHero.Core.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.Authentication.OAuth.Facebook;

public class Module : ModuleBase, IForceLoadModule, IAuthentication
{
    private bool _configured;

    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Autentication,Facebook";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "OAuth Facebook";

        Link = new ModuleLink(this, Description, OAuthController.MakeUrlChallenge(FacebookDefaults.AuthenticationScheme), true)
        {
            Icon = UIIcon.Facebook.GetName(),
            Enabled = false
        };
    }

    public override bool Configured => _configured;
    public AuthenticationType AuthenticationType => AuthenticationType.External;

    static bool IsConfigurated(Options options) => !string.IsNullOrEmpty(options.AppId) && !string.IsNullOrEmpty(options.AppSecret);

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        AddOptions<Options>(services, config);
        var options = services.GetOptionsSnapshot<Options>().Value;

        _configured = IsConfigurated(options);
        if (_configured)
        {
            services.AddAuthentication().AddFacebook(a =>
            {
                a.SignInScheme = IdentityConstants.ExternalScheme;
                a.AppId = options.AppId;
                a.AppSecret = options.AppSecret;

                options.MapCustomJson(a.ClaimActions);
                a.ClaimActions.MapJsonKey(ApplicationClaimTypes.ProfileImageUrl, "picture", "url");
            });
        }
    }
}