/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Corsinvest.AppHero.Core.Security.Auth;

public class AuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NavigationManager _navigationManager;
    private readonly AccessTokenProvider _tokenProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly bool _useToken = false;

    public AuthenticationService(IServiceProvider serviceProvider,
                                 NavigationManager navigationManager,
                                 IHttpContextAccessor httpContextAccessor,
                                 AccessTokenProvider tokenProvider)
    {
        _serviceProvider = serviceProvider;
        _navigationManager = navigationManager;
        _tokenProvider = tokenProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_useToken)
        {
            _httpContextAccessor.HttpContext.User = await _tokenProvider.GetClaimsPrincipal();
            return new AuthenticationState(_httpContextAccessor.HttpContext.User);
        }
        else
        {
            return new AuthenticationState(_httpContextAccessor.HttpContext == null
                                            ? new ClaimsPrincipal()
                                            : _httpContextAccessor.HttpContext.User);
        }
    }

    public async Task<bool> LoginAsync(LoginRequest loginRequest)
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByNameAsync(loginRequest.Username);
        var valid = user != null
                        && user.IsActive
                        && user.EmailConfirmed
                        && user.LockoutEnabled
                        && await userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (valid)
        {
            if (_useToken)
            {
                await _tokenProvider.GenerateJwt(user!);
                _navigationManager.NavigateTo("/", true);
            }
            else
            {
                var key = Guid.NewGuid();
                BlazorCookieLoginMiddleware.Logins[key] = loginRequest;
                _navigationManager.NavigateTo($"/login?key={key}", true);
            }
        }

        //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_httpContextAccessor.HttpContext!.User)));

        return valid;
    }


    public async Task LogoutAsync()
    {
        if (_useToken)
        {
            await _tokenProvider.RemoveAuthDataFromStorage();
            _navigationManager.NavigateTo("/", true);
        }
        else
        {
            _navigationManager.NavigateTo("/logout", true);
        }
        //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_httpContextAccessor.HttpContext!.User)));
    }
}