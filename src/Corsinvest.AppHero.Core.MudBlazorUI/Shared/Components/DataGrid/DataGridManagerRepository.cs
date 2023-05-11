/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using BlazorDownloadFile;
using Corsinvest.AppHero.Core.Domain.Repository;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;

public class DataGridManagerRepository<T> : DataGridManager<T>, IDataGridManagerRepository<T>
    where T : class
{
    public DataGridManagerRepository(IBlazorDownloadFileService blazorDownloadFileService,
                                     IDialogService dialogService,
                                     IRepository<T> repository)
        : base(blazorDownloadFileService, dialogService)
    {
        Repository = repository;
        QueryAsync = DefaultQueryAsync;
        SaveAsync = DefaultSaveAsync;
        DeleteAsync = DefaultDeleteAsync;
    }

    public async Task<IEnumerable<T>> DefaultQueryAsync() => await Repository.ListAsync();

    public async Task<bool> DefaultSaveAsync(T item, bool isNew)
    {
        var ret = SaveBeforeAsync == null || await SaveBeforeAsync.Invoke(item, isNew);
        if (ret)
        {
            if (isNew)
            {
                await Repository.AddAsync(item);
            }
            else
            {
                await Repository.UpdateAsync(item);
            }
            if (ret && SaveAfterAsync != null) { await SaveAfterAsync.Invoke(item, isNew); }
        }
        return ret;
    }

    public async Task<bool> DefaultDeleteAsync(IEnumerable<T> items)
    {
        await Repository.DeleteRangeAsync(items);
        await DeleteAfterAsync?.Invoke(items)!;
        return true;
    }

    public IRepository<T> Repository { get; }
}