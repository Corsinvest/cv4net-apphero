/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Components.Forms;

namespace Corsinvest.AppHero.Core.BaseUI.Pages.Authentication;

public partial class DefaultLoginBase : AHComponentBase
{
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] private IUINotifier UINotifier { get; set; } = default!;

    protected LoginRequest Model { get; } = new();
    protected bool DisableInput { get; set; }

    protected async Task OnValidSubmitLoginAsync(EditContext context)
    {
        if (context.Validate())
        {
            DisableInput = true;
            StateHasChanged();

            var result = await AuthenticationService.LoginAsync(Model);
            if (result)
            {
                Logger.LogInformation("{UserName} login successfully.", Model.Username);
            }
            else
            {
                Logger.LogWarning("{UserName} login fail.", Model.Username);
                UINotifier.Show(L["Please check your username and password. If you are still unable to log in, contact your administrator."], UINotifierSeverity.Error);
            }

            DisableInput = false;
            StateHasChanged();
        }
    }
}