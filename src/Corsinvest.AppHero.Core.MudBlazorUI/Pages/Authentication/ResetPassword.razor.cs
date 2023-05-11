/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Authentication;

[AllowAnonymous]
public partial class ResetPassword
{
    [Parameter] public string Email { get; set; } = default!;
    [Parameter] public string Token { get; set; } = default!;

    [Inject] private IOptionsSnapshot<Core.Security.Identity.Options> IdentityOptions { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;

    private ApplicationUser? User { get; set; }
    private string Password { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        User = await UserManager.FindByEmailAsync(Email);
        if (User == null)
        {
            Logger.LogWarning("{Email} User not found.", Email);
            UINotifier.Show(L["Email not valid!"], UINotifierSeverity.Error);
            return;
        }
    }

    private async Task ChangePasswordAsync()
    {
        var result = await UserManager.ResetPasswordAsync(User!, Token, Password);
        UINotifier.Show(result.Succeeded, L["Change password successfully."], result.ToStringErrors());
        if (result.Succeeded) { NavigationManager.NavigateTo("/"); }
    }
}
