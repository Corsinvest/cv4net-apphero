/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Authentication;

[AllowAnonymous]
public partial class ValidateRegistration
{
    [Parameter] public string Email { get; set; } = default!;
    [Parameter] public string Token { get; set; } = default!;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    private ApplicationUser? User { get; set; }

    protected override async Task OnInitializedAsync()
    {
        User = await UserManager.FindByEmailAsync(Email);
        if (User == null)
        {
            Logger.LogWarning("{Email} User not found.", Email);
            UINotifier.Show(L["Email not valid!"], UINotifierSeverity.Error);
            return;
        }
        else
        {
            var result = await UserManager.ConfirmEmailAsync(User, Token);
            UINotifier.Show(result.Succeeded, L["Email confirmed successfully."], result.ToStringErrors());
            if (result.Succeeded) { NavigationManager.NavigateTo("/"); }
        }
    }
}