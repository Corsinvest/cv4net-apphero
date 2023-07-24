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
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Corsinvest.AppHero.Authentication.OAuth.Google;

public class Module : ModuleBase, IForceLoadModule, IAuthentication
{
    private bool _configured;

    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Autentication,Google";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "OAuth Google";

        Link = new ModuleLink(this, Description, OAuthController.MakeUrlChallenge(GoogleDefaults.AuthenticationScheme), true)
        {
            Icon = UIIcon.Google.GetName(),
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

        //https://github.com/dotnet/aspnetcore/blob/master/src/Security/Authentication/samples/SocialSample/Startup.cs
        //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins
        _configured = IsConfigurated(options);
        if (_configured)
        {
            services.AddAuthentication().AddGoogle(a =>
            {
                a.SignInScheme = IdentityConstants.ExternalScheme;
                a.ClientId = options.ClientId;
                a.ClientSecret = options.ClientSecret;

                options.MapCustomJson(a.ClaimActions);
                a.ClaimActions.MapJsonKey(ApplicationClaimTypes.ProfileImageUrl, "picture", "url");
                a.ClaimActions.MapJsonKey(ClaimTypes.Locality, "locale", "string");
            });
        }
    }
}