/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.BaseUI.Shared.Components;

public partial class AHPermission
{
    [EditorRequired][Parameter] public Permission Permission { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public bool Loading { get; set; }

    [Inject] private IPermissionService PermissionService { get; set; } = default!;

    private bool Can { get; set; }

    protected override async Task OnInitializedAsync() => Can = await PermissionService.HasPermissionAsync(Permission);
}