/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Corsinvest.AppHero.Core.Security.Auth;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IServiceProvider _serviceProvider;

    public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _signInManager = signInManager;
        _serviceProvider = serviceProvider;
    }

    [AllowAnonymous]
    [HttpGet(nameof(Login))]
    public async Task<IActionResult> Login(string token)
    {
        var appOptions = _serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;
        var protector = DataProtectionProvider.Create(appOptions.Name).CreateProtector("Login");
        var data = protector.Unprotect(token);
        var parts = data.Split('|');
        var user = await _signInManager.UserManager.FindByIdAsync(parts[0]);
        if (user == null) { return Unauthorized(); }

        if (await _signInManager.UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "Login", parts[1]))
        {
            await _signInManager.UserManager.ResetAccessFailedCountAsync(user);
            await _signInManager.SignInAsync(user, bool.Parse(parts[2]));

            //culture
            var defaultCulture = string.IsNullOrWhiteSpace(user.DefaultCulture)
                                            ? "en-US"

                                            //from user
                                            : user.DefaultCulture;

            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(defaultCulture)));
        }

        return Redirect("/");
    }

    [Authorize]
    [HttpGet(nameof(SetCulture))]
    public IActionResult SetCulture(string culture, string redirectUri)
    {
        if (!string.IsNullOrWhiteSpace(culture))
        {
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
        }

        return Redirect(redirectUri);
    }

    [Authorize]
    [HttpGet(nameof(Logout))]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}