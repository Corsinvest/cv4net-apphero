/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared;

public partial class NavMenu
{
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;

    private IEnumerable<ModuleAuthorization> Authorizations { get; set; } = [];

    protected override async Task OnInitializedAsync()
        => Authorizations = (await ModularityService.GetAuthorizationsAsync(PermissionService))
                                .Where(a => a.Links.Any());

    private List<IGrouping<ModuleBase, ModuleAuthorization>> GetByCategory(string category)
        => Authorizations.Where(a => a.Module.Category == category && a.Module.Link!.Enabled)
                         .GroupBy(a => a.Module)
                         .OrderBy(a => a.Key.Link!.Order)
                         .ThenBy(a => a.Key.Link!.Text)
                         .ToList();
}