/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHUserMenu
{
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] private ICurrentUserService CurrentUserService { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;

    private ApplicationUser User { get; set; } = default!;
    private List<ModuleLink> Links { get; } = new();

    protected override async Task OnInitializedAsync()
    {
        User = (await UserManager.FindByIdAsync(CurrentUserService.UserId))!;

        //TODO fix with settingis menu user from configuration
        var links = new ModuleLink[]
        {
            ModularityService.Get<Corsinvest.AppHero.Core.Options.Module>()!.Link!,                                //Options
            ModularityService.Modules.Where(a=> typeof(Corsinvest.AppHero.Core.Security.Module).IsAssignableFrom(a.GetType()))
                                     .FirstOrDefault()!
                                     .Link!.Child.ToArray()[2]!    //profile
        };

        foreach (var item in links)
        {
            if (await PermissionService.HasPermissionAsync(item.Permission)) { Links.Add(item); }
        }
    }

    private void Logout() => AuthenticationService.LogoutAsync();
}