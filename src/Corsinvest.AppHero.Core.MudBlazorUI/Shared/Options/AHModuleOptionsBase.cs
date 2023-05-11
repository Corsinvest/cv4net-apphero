/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Linq.Expressions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Options;

public class AHModuleOptionsBase<T> : AHComponentBase, ISavable where T : class, new()
{
    [Inject] private IWritableOptionsService<T> WritableOptionsService { get; set; } = default!;
    [Inject] private IOptionsSnapshot<T> SnapshotOptions { get; set; } = default!;

    protected virtual T Options => SnapshotOptions.Value;

    public virtual async Task SaveAsync()
    {
        WritableOptionsService.Update(Options);
        await Task.CompletedTask;
    }

    public string GetDescriptionProperty(Expression<Func<T, object>> expression) => MudBlazorHelper.GetDescriptionProperty(expression);
    public string GetDescriptionProperty(string propertyName) => MudBlazorHelper.GetDescriptionProperty<T>(propertyName);
}