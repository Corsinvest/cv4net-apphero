/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Microsoft.Extensions.Localization;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHAppMenu
{
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private IStringLocalizer<AHUserMenu> L { get; set; } = default!;

    private List<ModuleLink> Links { get; set; } = new()!;
    private string Search { get; set; } = default!;

    protected override async Task OnInitializedAsync() => Links = (await ModularityService.GetAuthorizedLinksAsync(PermissionService)).ToList();
}