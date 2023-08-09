/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using BlazorDownloadFile;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using MiniExcelLibs;
using System.Data;
using System.Linq.Dynamic.Core;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;

public class DataGridManager<T> : IDataGridManager<T> where T : class
{
    private readonly IDialogService _dialogService;
    private readonly IBlazorDownloadFileService _blazorDownloadFileService;
    private GridState<T> _state = default!;

    public DataGridManager(IBlazorDownloadFileService blazorDownloadFileService, IDialogService dialogService)
    {
        _blazorDownloadFileService = blazorDownloadFileService;
        _dialogService = dialogService;
        NewAsync = NewDefaultAsync;
    }

    #region Delete
    public async Task<bool> DeleteSelectedItemsAsync(bool askConfirm)
    {
        var items = DataGrid!.SelectedItems.ToArray();
        var ret = items.Length > 0 && await DeleteItemsAsync(items, askConfirm);
        if (ret) { DataGrid!.SelectedItems.Clear(); }
        return ret;
    }

    public async Task<bool> DeleteItemsAsync(IEnumerable<T> items, bool askConfirm)
    {
        var ret = false;
        if (!askConfirm || await _dialogService.DeleteAsync("Confirm delete selections?"))
        {
            if (await DeleteAsync(items))
            {
                if (DeleteAfterAsync != null) { await DeleteAfterAsync.Invoke(items); }

                ret = true;
                await Refresh();
            }
        }

        return ret;
    }

    public Func<IEnumerable<T>, Task<bool>> DeleteAsync { get; set; } = default!;
    public Func<IEnumerable<T>, Task> DeleteAfterAsync { get; set; } = default!;
    #endregion

    public async Task Refresh()
    {
        if (QueryAsync != null) { await DataGrid!.ReloadServerData(); }
    }

    #region New
    public virtual async Task NewDefaultAsync()
    {
        if (NewBeforeAsync != null) { await NewBeforeAsync.Invoke(); }
        await EditAsync(Activator.CreateInstance<T>(), true);
    }

    public Func<Task> NewAsync { get; set; } = default!;
    public Func<Task> NewBeforeAsync { get; set; } = default!;
    #endregion

    #region Edit
    public RenderFragment<T> EditTemplate { get; set; } = default!;

    public virtual async Task<bool> EditAsync(T item, bool isNew)
    {
        var editItem = await BeforeEditAsync(item, isNew);
        var ret = await _dialogService.ShowDialogEditorAsync(Title,
                                                        EditTemplate,
                                                        editItem,
                                                        isNew,
                                                        SaveAsync,
                                                        new DialogOptions
                                                        {
                                                            CloseButton = true,
                                                            MaxWidth = EditDialogMaxWidth,
                                                            FullWidth = true,
                                                            DisableBackdropClick = true
                                                        },
                                                        "Cancel",
                                                        "Save");

        if (ret) { await Refresh(); }
        return ret;
    }

    public Func<T, bool, Task<T>> BeforeEditAsync { get; set; } = async (item, isNew) => await Task.FromResult(item);
    public MaxWidth EditDialogMaxWidth { get; set; }
    #endregion

    #region Save
    public async Task<bool> SaveDefaultAsync(T item, bool isNew)
    {
        var ret = SaveBeforeAsync == null || await SaveBeforeAsync.Invoke(item, isNew);
        if (ret)
        {
            ret = await SaveAsync(item, isNew);
            if (ret && SaveAfterAsync != null) { await SaveAfterAsync.Invoke(item, isNew); }
        }
        return ret;
    }

    public Func<T, bool, Task<bool>> SaveAsync { get; set; } = default!;
    public Func<T, bool, Task<bool>> SaveBeforeAsync { get; set; } = default!;
    public Func<T, bool, Task> SaveAfterAsync { get; set; } = default!;
    #endregion

    public string Title { get; set; } = default!;
    public Dictionary<string, bool> DefaultSort { get; set; } = new();
    public Func<Task<IEnumerable<T>>> QueryAsync { get; set; } = default!;
    public MudDataGrid<T>? DataGrid { get; set; } = default!;

    private string _search = default!;
    public string Search
    {
        get => _search;
        set
        {
            _search = value;
            DataGrid!.ReloadServerData();
        }
    }

    public bool ExistsSelection => DataGrid!.SelectedItems.Any();

    public async Task ExportToExcel()
    {
        var table = new DataTable();
        var columns = DataGrid!.RenderedColumns.Where(a => !string.IsNullOrEmpty(a.PropertyName) && !string.IsNullOrEmpty(a.Title))
                                                        .Select(a => new DataColumn(a.Title)
                                                        {
                                                            ColumnName = a.PropertyName,
                                                        })
                                                        .ToArray();
        table.Columns.AddRange(columns);

        foreach (var item in (await DataGrid!.ServerData(_state)).Items)
        {
            var row = columns.Select(a => item!.GetType().GetPropertyFromPath(a.ColumnName))
                             .Select(a => a == null
                                          ? ""
                                          : (a.GetValue(item) + "").ToString())
                             .ToArray();
            table.Rows.Add(row);
        }

        using var stream = new MemoryStream();
        MiniExcel.SaveAs(stream, table);
        stream.Seek(0, SeekOrigin.Begin);

        var fileName = string.IsNullOrEmpty(Title)
                        ? "data"
                        : Title;
        await _blazorDownloadFileService.DownloadFile($"{fileName}.xlsx", stream, "application/vnd.ms-excel");
    }

    public async Task<GridData<T>> LoadServerData(GridState<T> state)
    {
        _state = state;
        var query = (await QueryAsync()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(_search))
        {
            //manual search
            var where = new List<string>();
            foreach (var item in DataGrid!.RenderedColumns.Where(a => (a.Filterable == true || DataGrid!.Filterable)
                                                                      && !string.IsNullOrWhiteSpace(a.PropertyName)))
            {
                var pi = typeof(T).GetProperty(item.PropertyName)!;
                if (pi != null && pi.CanWrite)
                {
                    var @operator = Type.GetTypeCode(pi.PropertyType) switch
                    {
                        TypeCode.Empty or TypeCode.Object or TypeCode.DBNull => string.Empty,

                        TypeCode.Boolean => bool.TryParse(_search, out var valBool) ? "=" : string.Empty,

                        TypeCode.Char or TypeCode.String => "contains",

                        TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32
                                        or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal
                                        => decimal.TryParse(_search, out var decValue) ? "=" : string.Empty,

                        TypeCode.DateTime => DateTime.TryParse(_search, out var dateValue) ? "=" : string.Empty,

                        _ => string.Empty,
                    };

                    if (@operator == "contains")
                    {
                        where.Add($"{item.PropertyName} != null && {item.PropertyName}.ToLower().Contains(@0.ToLower())");
                    }
                    else if (@operator == "=")
                    {
                        where.Add($"{item.PropertyName} != null && {item.PropertyName} == @0");
                    }
                }
            }

            query = query.Where(where.JoinAsString(" || "), _search);
        }
        else
        {
            var filterOptions = new FilterOptions
            {
                FilterCaseSensitivity = DataGrid?.FilterCaseSensitivity ?? DataGridFilterCaseSensitivity.Default,
            };

            //row filter
            foreach (var item in state.FilterDefinitions)
            {
                query = query.Where(FilterExpressionGenerator.GenerateExpression(item, filterOptions));
            }
        }

        var count = query.Count();

        //order
        if (state.SortDefinitions.Any())
        {
            var orderBy = state.SortDefinitions.OrderBy(a => a.Index)
                                               .Select(a => $"@{a.SortBy} {(a.Descending ? "desc" : "asc")}")
                                               .JoinAsString(", ");
            query = query.OrderBy(orderBy);
        }

        //pagination
        query = query.Skip(state.Page * state.PageSize)
                     .Take(state.PageSize);

        return new()
        {
            Items = query,
            TotalItems = count,
        };
    }

    public PermissionsRead? Permissions { get; set; }
    public HashSet<T> SelectedItems => DataGrid!.SelectedItems;
    public T SelectedItem => DataGrid!.SelectedItem;
}