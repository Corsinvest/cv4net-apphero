/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHWidgets
{
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;

    private IEnumerable<ModuleAuthorization> Authorizations { get; set; } = [];

    protected override async Task OnInitializedAsync()
        => Authorizations = (await ModularityService.GetAuthorizationsAsync(PermissionService))
                                .Where(a => a.GetWidgets().Any());

    private static string GetWidgetUrl(ModuleWidget widget)
        => widget.RefLink != null && widget.RefLink.Enabled && !string.IsNullOrWhiteSpace(widget.RefLink.RealUrl)
            ? widget.RefLink.RealUrl
            : "#";

}
