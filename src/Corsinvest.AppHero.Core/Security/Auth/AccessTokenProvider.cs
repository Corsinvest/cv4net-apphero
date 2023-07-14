using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Corsinvest.AppHero.Core.Security.Auth;

public class AccessTokenProvider 
{
    private readonly string _tokenKey = nameof(_tokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IIdentityService _identityService;

    public AccessTokenProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _identityService = identityService;
    }

    public async Task GenerateJwt(ApplicationUser applicationUser)
        => await _localStorage.SetAsync(_tokenKey, await _identityService.GenerateJwtAsync(applicationUser));

    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            var token = await _localStorage.GetAsync<string>(_tokenKey);
            if (token.Success && !string.IsNullOrEmpty(token.Value))
            {
                var principal = await _identityService.GetClaimsPrincipal(token.Value);
                if (principal?.Identity?.IsAuthenticated ?? false) { return principal!; }
            }
        }
        catch (CryptographicException) { await RemoveAuthDataFromStorage(); }
        catch (Exception) { return new ClaimsPrincipal(new ClaimsIdentity()); }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    public async Task RemoveAuthDataFromStorage()
    {
        await _localStorage.DeleteAsync(_tokenKey);
        _navigation.NavigateTo("/", true);
    }
}
