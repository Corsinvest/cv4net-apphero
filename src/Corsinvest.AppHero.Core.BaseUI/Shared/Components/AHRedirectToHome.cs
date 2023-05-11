/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Components.Authorization;

namespace Corsinvest.AppHero.Core.BaseUI.Shared.Components;

public class AHRedirectToHome : AHComponentBase
{
    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized() => NavigationManager.NavigateTo("/");

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationState;
        if (authState?.User?.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            //await Task.Delay(2000);
            NavigationManager.NavigateTo("/");
        }
    }
}