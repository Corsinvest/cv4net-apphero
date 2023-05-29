/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Authentication.ActiveDirectory;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Corsinvest.AppHero.Authentication.MudBlazorUI.ActiveDirectory;

public partial class RenderIndex
{
    [Inject] private IAutenticationActiveDirectory AuthenticationService { get; set; } = default!;
    [Inject] private IUINotifier UINotifier { get; set; } = default!;
    [Inject] protected IOptionsSnapshot<Authentication.ActiveDirectory.Options> Options { get; set; } = default!;

    protected LoginRequestAD Model { get; } = new();
    protected bool DisableInput { get; set; }

    protected override void OnInitialized()
        => Model.Domain = Options.Value.Domains.OrderBy(a => a.Default).Select(a => a.Domain).FirstOrDefault()!;

    protected async Task OnValidSubmitLoginAsync(EditContext context)
    {
        if (context.Validate())
        {
            DisableInput = true;
            StateHasChanged();

            var result = await AuthenticationService.LoginAsync(Model);
            if (result.IsSuccess)
            {
                Logger.LogInformation("{UserName} login successfully.", Model.Username);
            }
            else
            {
                var msg = result.Errors.Select(a => a.Message).JoinAsString(Environment.NewLine);
                Logger.LogWarning("{UserName} login fail." + Environment.NewLine + msg, Model.Username);
                UINotifier.Show(L[msg], UINotifierSeverity.Error);
            }

            DisableInput = false;
            StateHasChanged();
        }
    }
}