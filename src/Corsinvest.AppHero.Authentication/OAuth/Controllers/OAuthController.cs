/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: Auth

using Corsinvest.AppHero.Authentication.Helper;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Corsinvest.AppHero.Authentication.OAuth.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
public class OAuthController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IServiceProvider _serviceProvider;

    public OAuthController(SignInManager<ApplicationUser> signInManager, ILogger<OAuthController> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _signInManager = signInManager;
        _serviceProvider = serviceProvider;
    }

    public static string MakeUrlChallenge(string authenticationScheme) => $"/api/OAuth/challenge/{authenticationScheme}";

    [HttpGet("challenge/{provider}")]
    [AllowAnonymous]
    public IActionResult Challenge(string provider)
        => Challenge(new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalSignInAsync)),
            Items =
            {
                ["returnUrl"] = "/" ,
                ["scheme"] = provider ,
            }
        },
        provider);


    [AllowAnonymous]
    [HttpGet("ExternalSignIn")]
    public async Task<IActionResult> ExternalSignInAsync()
    {
        try
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded) { return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.ExternalAuthError)); }

            var externalUser = result.Principal;
            if (externalUser == null) { return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.ExternalAuthError)); }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {claims}", externalClaims);
            }

            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used                
            var userIdClaim = externalUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) { return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.ExternalUnknownUserId)); }

            var externalUserId = userIdClaim.Value;
            var externalProvider = userIdClaim.Issuer;

            //If external login/signin failed
            var userEmail = externalUser.FindFirstValue(ClaimTypes.Email)!;

            //get the user by Email (we are forcing it to be unique)
            var user = await _signInManager.UserManager.FindByEmailAsync(userEmail);
            var autoImportUser = bool.Parse(externalUser.FindFirstValue(nameof(OptionsBase.AutoImportUser)) ?? false.ToString());
            if (user == null && !autoImportUser)
            {
                _logger.LogInformation("External autentication AutoImportUser disabled");
                return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.AutoImportUser));
            }

            if (user?.IsActive is false)
            {
                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                _logger.LogInformation("User not active: {UserName}", user.UserName);
                return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserNotActive));
            }

            //Quick check to sign in
            var externalSignInResult = await _signInManager.ExternalLoginSignInAsync(externalProvider, externalUserId, true);
            if (externalSignInResult.Succeeded)
            {
                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return LocalRedirect("/");
            }
            else
            {
                if (user == null)
                {
                    user = new()
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FirstName = externalUser.FindFirstValue(ClaimTypes.GivenName),
                        LastName = externalUser.FindFirstValue(ClaimTypes.Surname),
                        EmailConfirmed = true,
                        IsActive = true,
                        ProfileImageUrl = externalUser.FindFirstValue(ApplicationClaimTypes.ProfileImageUrl),
                        DefaultCulture = externalUser.FindFirstValue(ClaimTypes.Locality) ?? "en-US",
                    };

                    try
                    {
                        var identityResult = await _signInManager.UserManager.CreateAsync(user);
                        if (identityResult.Succeeded)
                        {
                            var roles = (externalUser.FindFirstValue(nameof(OptionsBase.DefaultRolesNewUser)) + "")
                                            .Split(",").Where(a => !string.IsNullOrEmpty(a));
                            if (roles.Any())
                            {
                                identityResult = await _signInManager.UserManager.AddRolesToUserAsync(user, roles);
                            }
                        }

                        if (!identityResult.Succeeded)
                        {
                            var msg = identityResult.Errors.Select(a => $"{a.Code} - {a.Description}").JoinAsString(Environment.NewLine);
                            return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserCreationFailed, msg));
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserCreationFailed, ex.Message));
                    }
                }
                else
                {
                    //check if the login for this provider exists
                    var userLogins = await _signInManager.UserManager.GetLoginsAsync(user);
                    if (userLogins.Any(a => a.LoginProvider == externalProvider && a.ProviderKey == externalUserId))
                    {
                        //something went wrong, it should get logged in
                        // If lock out activated and the max. amounts of attempts is reached.
                        if (externalSignInResult.IsLockedOut)
                        {
                            _logger.LogInformation("User Locked out: {UserName}", user.UserName);
                            return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserLockedOut));
                        }

                        // If your email is not confirmed but you require it in the settings for login.
                        if (externalSignInResult.IsNotAllowed)
                        {
                            _logger.LogInformation("User not allowed to log in: {UserName}", user.UserName);
                            return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserIsNotAllowed));
                        }

                        return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.Unknown));
                    }
                }

                //All if fine, this user (email) did not try to log in before using this external provider
                //Add external login info
                var addExternalLoginResult = await _signInManager.UserManager.AddLoginAsync(user,
                                                                                       new UserLoginInfo(externalProvider,
                                                                                                               externalUserId,
                                                                                                               externalUser.FindFirstValue(ClaimTypes.Name)));
                if (!addExternalLoginResult.Succeeded)
                {
                    return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.CannotAddExternalLogin));
                }

                //Try to sign in again
                externalSignInResult = await _signInManager.ExternalLoginSignInAsync(externalProvider, externalUserId, true);
                if (externalSignInResult.Succeeded)
                {
                    // delete temporary cookie used during external authentication
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    return LocalRedirect("/");
                }
                else
                {
                    //Something went terrible wrong, user exists, external login info added
                    // If lock out activated and the max. amounts of attempts is reached.
                    if (externalSignInResult.IsLockedOut)
                    {
                        _logger.LogInformation("User Locked out: {UserName}", user.UserName);
                        return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserLockedOut));
                    }

                    // If your email is not confirmed but you require it in the settings for login.
                    if (externalSignInResult.IsNotAllowed)
                    {
                        _logger.LogInformation("User not allowed to log in: {UserName}", user.UserName);
                        return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.UserIsNotAllowed));
                    }

                    return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.Unknown));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.GetBaseException().Message);
            return LocalRedirect(ExternalAuthenticationHelper.MakeUrlError(ExternalAuthError.Unknown));
        }
    }
}