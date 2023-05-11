/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Notification;

namespace Corsinvest.AppHero.Core.BaseUI.Shared.Notification;

public class NotificationChannelsOptionsGridBase<T> : AHComponentBase where T : NotificationChannelOptions, new()
{
    [Parameter] public NotificationChannelsOptions<T> Options { get; set; } = default!;
    [Parameter] public RenderFragment<T> Row { get; set; } = default!;
    [Inject] protected IDataGridManager<T> DataGridManager { get; set; } = default!;
    [Inject] private IUIMessageBox ViewMessageBox { get; set; } = default!;

    protected bool LoadingTest { get; set; }

    protected override void OnInitialized()
    {
        DataGridManager.Title = L["Notification "] + Activator.CreateInstance<T>().Name;
        DataGridManager.DefaultSort = new()
        {
            [nameof(NotificationChannelOptions.Type)] = false,
            [nameof(NotificationChannelOptions.Name)] = false
        };

        DataGridManager.QueryAsync = async () => await Task.FromResult(Options.Channels);

        DataGridManager.SaveAsync = async (item, isNew) =>
        {
            if (isNew) { Options.Channels.Add(item); }
            return await Task.FromResult(true);
        };

        DataGridManager.DeleteAsync = async (items) =>
        {
            foreach (var item in items) { Options.Channels.Remove(item); ; }
            return await Task.FromResult(true);
        };
    }

    protected async Task TestAsync(T channel)
    {
        LoadingTest = true;
        if (await ViewMessageBox.ShowQuestionAsync(L["Send test"], L["Execute Test?"]))
        {
            try { await channel.SendTest(); }
            catch (Exception ex) { await ViewMessageBox.ShowInfoAsync(L["Error"], ex.Message); }
        }
        LoadingTest = false;
    }
}