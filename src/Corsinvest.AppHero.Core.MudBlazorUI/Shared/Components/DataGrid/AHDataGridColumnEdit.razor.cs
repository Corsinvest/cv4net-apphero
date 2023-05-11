/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;

public partial class AHDataGridColumnEdit<T> where T : class
{
    [EditorRequired][Parameter] public IDataGridManager<T> DataGridManager { get; set; } = default!;
    [EditorRequired][Parameter] public CellContext<T> Context { get; set; } = default!;

    private Permission? PermissionEdit { get; set; }

    protected override void OnInitialized()
        => PermissionEdit = DataGridManager.Permissions is PermissionsCrud crud
            ? crud.Edit
            : null;
}