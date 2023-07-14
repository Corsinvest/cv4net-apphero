/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Collections.Concurrent;

namespace Corsinvest.AppHero.Core.Security.Auth;

public class BlazorCookieLoginMiddleware
{
    public static IDictionary<Guid, LoginRequest> Logins { get; } = new ConcurrentDictionary<Guid, LoginRequest>();
    private readonly RequestDelegate _next;

    public BlazorCookieLoginMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        if (context.Request.Path == "/culture" && context.Request.Query.ContainsKey("culture")) { SetCulture(context); }
        else if (context.Request.Path == "/logout") { await Logout(context, signInManager); }
        else if (context.Request.Path == "/login" && context.Request.Query.ContainsKey("key")) { await Login(context, signInManager); }
        else { await _next.Invoke(context); }
    }

    private static void SetCulture(HttpContext context)
    {
        SetCulture(context, context.Request.Query["culture"] + "");

        context.Response.Redirect(context.Request.Query.ContainsKey("redirectUri")
                                    ? context.Request.Query["redirectUri"] + ""
                                    : "/");
    }

    private static void SetCulture(HttpContext context, string culture)
        => context.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                           CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));


    private static async Task Logout(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        context.Response.Redirect("/");
    }

    private static async Task Login(HttpContext context, SignInManager<ApplicationUser> signInManager)
    {
        var key = Guid.Parse(context.Request.Query["key"]!);
        var info = Logins[key];
        Logins.Remove(key);

        var result = await signInManager.PasswordSignInAsync(info.Username, info.Password, false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            var user = await signInManager.UserManager.FindByNameAsync(info.Username);

            //culture
            var defaultCulture = string.IsNullOrWhiteSpace(user!.DefaultCulture)
                                            ? "en-US"

                                            //from user
                                            : user.DefaultCulture;

            SetCulture(context, defaultCulture);

            context.Response.Redirect("/");
        }
        //else if (result.RequiresTwoFactor)
        //{
        //    //TODO: redirect to 2FA razor component
        //    context.Response.Redirect("/loginwith2fa/" + key);
        //}
        else
        {
            context.Response.Redirect("/");
        }
    }
}