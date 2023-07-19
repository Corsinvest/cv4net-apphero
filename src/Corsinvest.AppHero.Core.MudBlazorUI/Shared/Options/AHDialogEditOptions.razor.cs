/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Options;

public partial class AHDialogEditOptions
{
    [Parameter] public ModuleBase Module { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

    [Inject] private IServiceScopeFactory ScopeFactory { get; set; } = default!;

    private DynamicComponent? RefOptions { get; set; } = default!;
    public bool InSaving { get; set; }

    private async Task SubmitAsync()
    {
        InSaving = true;
        await ((ISavable)RefOptions!.Instance!).SaveAsync();

        using var scope = ScopeFactory.CreateScope();
        await Module.RefreshOptionsAsync(scope);

        InSaving = false;

        Logger.LogInformation(L["Save options {0}", Module.FullInfo]);
        MudDialog.Close(DialogResult.Ok(true));
        //force refresh
        //NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    void Cancel() => MudDialog.Cancel();
}