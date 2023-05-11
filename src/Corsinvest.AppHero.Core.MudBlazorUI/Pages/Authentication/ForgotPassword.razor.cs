/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Authentication;

[AllowAnonymous]
public partial class ForgotPassword
{
    [Inject] private IIdentityService IdentityService { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;

    private bool ShowWait { get; set; }
    private Data Model { get; set; } = new();

    class Data
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }

    private async Task OnValidForgotPasswordAsync()
    {
        var user = await UserManager.FindByEmailAsync(Model.Email);
        if (user == null)
        {
            Logger.LogWarning("{Email} user not found!", Model.Email);
            UINotifier.Show(L["User not found!!"], UINotifierSeverity.Error);
        }
        else
        {
            ShowWait = true;
            StateHasChanged();

            await IdentityService.SendEmailResetPasswordAsync(Model.Email);

            ShowWait = false;
            StateHasChanged();
            UINotifier.Show(L["Please check your mailbox!."], UINotifierSeverity.Success);
        }
    }
}