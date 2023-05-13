/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Corsinvest.AppHero.Core.Security.Auth;

public class AuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly NavigationManager _navigationManager;

    public AuthenticationService(IServiceProvider serviceProvider, NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _navigationManager = navigationManager;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_httpContextAccessor.HttpContext == null
                                    ? new System.Security.Claims.ClaimsPrincipal()
                                    : _httpContextAccessor.HttpContext.User));
    }

    public async Task ExecuteLoginAsync(ApplicationUser user, bool rememberMe)
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "Login");
        var data = $"{user.Id}|{token}|{rememberMe}";

        var dataProtectionProvider = _serviceProvider.GetRequiredService<IDataProtectionProvider>();
        var protector = dataProtectionProvider.CreateProtector("Login");
        var protectedData = protector.Protect(data);
        _navigationManager.NavigateTo($"/api/account/login?token=" + protectedData, true);
    }

    public async Task<bool> LoginAsync(LoginRequest loginRequest)
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByNameAsync(loginRequest.Username);
        var ret = user != null
                  && user.IsActive
                  && user.EmailConfirmed
                  && user.LockoutEnabled
                  && await userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (ret) { await ExecuteLoginAsync(user!, loginRequest.RememberMe); }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_httpContextAccessor.HttpContext!.User)));

        return ret;
    }

    public async Task Logout()
    {
        await Task.CompletedTask;
        _navigationManager.NavigateTo("/api/account/logout", true);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_httpContextAccessor.HttpContext!.User)));
    }
}