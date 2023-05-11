/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

//namespace Corsinvest.AppHero.Core.MudBlazorUI.DataGrid;

//public class DataGridManagerEFCore<T> : DataGridManager<T> where T : class
//{
//    public DataGridManagerEFCore(IServiceScopeFactory scopeFactory,
//                                 IBlazorDownloadFileService blazorDownloadFileService,
//                                 IDialogService dialogService)
//        : base(scopeFactory, blazorDownloadFileService, dialogService)
//    {
//        QueryAsync = DefaultQueryAsyncEF;
//        SaveAsync = DefaultSaveAsyncEF;
//        DeleteAsync = DefaultDeleteAsyncEF;
//    }

//    public async Task<IEnumerable<T>> DefaultQueryAsyncEF() => await Task.FromResult(DefaultItemsEF());

//    public IEnumerable<T> DefaultItemsEF()
//        => DbContext == null
//            ? throw new ArgumentNullException(nameof(DbContext))
//            : DbContext.Set<T>();

//    public async Task<bool> DefaultSaveAsyncEF(T item, bool isNew)
//    {
//        var ret = SaveBeforeAsync == null || await SaveBeforeAsync.Invoke(item, isNew);
//        if (ret)
//        {
//            if (isNew) { DbContext.Set<T>().Add(item); }
//            await DbContext.SaveChangesAsync();
//            if (ret && SaveAfterAsync != null) { await SaveAfterAsync.Invoke(item, isNew); }
//        }
//        return ret;
//    }

//    public async Task<bool> DefaultDeleteAsyncEF(IEnumerable<T> items)
//    {
//        foreach (var item in items) { DbContext.Set<T>().Remove(item); }
//        await DbContext.SaveChangesAsync();
//        await DeleteAfterAsync?.Invoke(items)!;
//        return true;
//    }

//    public DbContext DbContext { get; set; } = default!;
//}
