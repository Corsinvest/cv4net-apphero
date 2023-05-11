/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Options;

public partial class AHDialogEditOptions
{
    [Parameter] public ModuleBase Module { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IServiceScopeFactory ScopeFactory { get; set; } = default!;

    private DynamicComponent? RefOptions { get; set; } = default!;

    private async Task SubmitAsync()
    {
        await ((ISavable)RefOptions!.Instance!).SaveAsync();

        using var scope = ScopeFactory.CreateScope();
        await Module.RefreshOptionsAsync(scope);

        Logger.LogInformation(L["Save options {Module}"], Module.FullInfo);
        MudDialog.Close(DialogResult.Ok(true));

        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    void Cancel() => MudDialog.Cancel();
}