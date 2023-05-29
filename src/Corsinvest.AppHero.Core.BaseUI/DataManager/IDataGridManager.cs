/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Contracts;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.BaseUI.DataManager;

public interface IDataGridManager<T> : IRefreshable where T : class
{
    Func<T, bool, Task<T>> BeforeEditAsync { get; set; }
    Func<IEnumerable<T>, Task> DeleteAfterAsync { get; set; }
    Func<IEnumerable<T>, Task<bool>> DeleteAsync { get; set; }
    Func<Task> NewAsync { get; set; }
    Func<Task> NewBeforeAsync { get; set; }
    Func<T, bool, Task> SaveAfterAsync { get; set; }
    Func<T, bool, Task<bool>> SaveAsync { get; set; }
    Func<T, bool, Task<bool>> SaveBeforeAsync { get; set; }
    Task<bool> DeleteItemsAsync(IEnumerable<T> items, bool askConfirm);
    Task<bool> DeleteSelectedItemsAsync(bool askConfirm);
    Task<bool> EditAsync(T item, bool isNew);
    Task NewDefaultAsync();
    Task<bool> SaveDefaultAsync(T item, bool isNew);
    PermissionsRead? Permissions { get; set; }
    Func<Task<IEnumerable<T>>> QueryAsync { get; set; }
    string Title { get; set; }
    Task ExportToExcel();
    bool ExistsSelection { get; }
    Dictionary<string, bool> DefaultSort { get; set; }
    HashSet<T> SelectedItems { get; }
    T SelectedItem { get; }
}
