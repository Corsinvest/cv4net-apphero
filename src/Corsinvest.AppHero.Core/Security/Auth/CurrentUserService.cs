/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Corsinvest.AppHero.Core.Security.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _scopeFactory;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _scopeFactory = scopeFactory;
    }

    public async Task<ApplicationUser?> GetUserAsync()
    {
        if (string.IsNullOrEmpty(UserId))
        {
            using var scope = _scopeFactory.CreateScope();
            await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().FindByIdAsync(UserId);
        }

        return null;
    }

    public ClaimsPrincipal? ClaimsPrincipal => _httpContextAccessor.HttpContext?.User;

    public string UserId
        => IsAuthenticated
            ? ClaimsPrincipal!.FindFirst(ClaimTypes.NameIdentifier)!.Value
            : string.Empty;

    public string UserName
        => IsAuthenticated
            ? ClaimsPrincipal!.FindFirst(ClaimTypes.Name)!.Value
            : "anonymous";

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated is true;
    public string HttpConnectionId => _httpContextAccessor.HttpContext?.Connection.Id!;

    public string IpAddress
    {
        get
        {
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress + "";
            return string.IsNullOrWhiteSpace(ip) ? "unknown" : ip;
        }
    }

    public void SetCulture(string culture, string redirectUri)
    {
        using var scope = _scopeFactory.CreateScope();
        var navigationManager = scope.ServiceProvider.GetRequiredService<NavigationManager>();
        navigationManager.NavigateTo(UrlHelper.SetParameter("/culture",
                                                               new()
                                                               {
                                                                    { "culture", culture},
                                                                    { "redirectUri", redirectUri}
                                                               }),
                                                               true);
    }
}