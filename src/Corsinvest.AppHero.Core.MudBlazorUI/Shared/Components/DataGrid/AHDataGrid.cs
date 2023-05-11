/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.MudBlazorUI.Style;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;

public partial class AHDataGrid<T> : MudDataGrid<T> where T : class
{
    [Parameter] public PermissionsRead Permissions { get; set; } = default!;
    [Parameter] public IDataGridManager<T> DataGridManager { get; set; } = default!;
    [Parameter] public RenderFragment<T> EditTemplate { get; set; } = default!;

    [Inject] protected IOptionsSnapshot<UIOptions> UIOptions { get; set; } = default!;

    protected override void OnInitialized()
    {
        var style = UIOptions.Value.Theme.Table;
        Dense = style.IsDense;
        Striped = style.IsStriped;
        Bordered = style.IsBordered;
        Hover = style.IsHoverable;

        if (DataGridManager != null)
        {
            DataGridManager.Permissions = Permissions;
            var dataGridManager = DataGridManager.ToDataGridManager();
            ServerData = dataGridManager.LoadServerData;
            dataGridManager.EditTemplate = EditTemplate;
            dataGridManager.DataGrid = this;

            //default sort
            foreach (var item in DataGridManager.DefaultSort.WithIndex())
            {
                SortDefinitions.Add(item.item.Key, new(item.item.Key, item.item.Value, item.index + 1, null!));
            }
        }

        base.OnInitialized();
    }
}