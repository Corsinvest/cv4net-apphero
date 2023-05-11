/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Options;

public partial class Index
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private IServiceScopeFactory ServiceScopeFactory { get; set; } = default!;

    private bool ExpansionPanels { get; set; } = true;
    private bool IsValid { get; set; }
    private string[] Errors { get; set; } = default!;
    private Dictionary<string, DynamicComponent> OptionsComponents { get; } = new();
    private IEnumerable<ModuleBase> Modules { get; set; } = default!;

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

        NavigationManager.NavigateTo(NavigationManager.Uri, true);

        UINotifier.Show(L["Options saved."], UINotifierSeverity.Success);
    }
}
