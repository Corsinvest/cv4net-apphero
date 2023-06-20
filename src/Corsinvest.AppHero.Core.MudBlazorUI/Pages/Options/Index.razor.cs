/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Options;

public partial class Index
{
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private IServiceScopeFactory ServiceScopeFactory { get; set; } = default!;

    private bool IsValid { get; set; }
    private string[] Errors { get; set; } = default!;
    private Dictionary<string, DynamicComponent> OptionsComponents { get; } = new();
    private IEnumerable<ModuleBase> Modules { get; set; } = default!;
    private bool InSaving { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var modules = ModularityService.Modules
                                       .IsEnabled()
                                       .Where(a => a.HasOptions && a.Options!.Render != null)
                                       .OrderBy(a => a.Description)
                                       .ToList();

        //check permission edit
        foreach (var item in modules.ToArray())
        {
            if (!(await item.GetAuthorizationAsync(PermissionService)).Has(item.PermissionEditOptions))
            {
                modules.Remove(item);
            }
        }

        Modules = modules;
    }

    private async Task OnSaveAsync()
    {
        InSaving = true;

        await ((ISavable)this).SaveAsync();

        //components
        foreach (var item in OptionsComponents.Values.Where(a => a != null)
                                                     .Select(a => a.Instance)
                                                     .OfType<ISavable>())
        {
            await item.SaveAsync();
        }

        using var scope = ServiceScopeFactory.CreateScope();
        await ModularityService.RefreshOptionsAsync(scope);

        //refresh force
        //NavigationManager.NavigateTo(NavigationManager.Uri, true);

        InSaving = false;

        UINotifier.Show(L["Options saved."], UINotifierSeverity.Success);
    }
}
