/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;

public partial class AHDataGridToolBarRead<T> where T : class
{
    [Parameter] public RenderFragment ToolBarContentBefore { get; set; } = default!;
    [Parameter] public RenderFragment ToolBarContentAfter { get; set; } = default!;
    [EditorRequired][Parameter] public IDataGridManager<T> DataGridManager { get; set; } = default!;

    private Permission? PermissionSearch { get; set; }
    private Permission? PermissionExport { get; set; }

    protected override void OnInitialized()
    {
        if (DataGridManager.Permissions is PermissionsRead read)
        {
            PermissionSearch = read.Search;
            PermissionExport = read.Export;
        }
    }
}