/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.BaseUI.Modularity;

public class PackagesSourceBase : AHComponentBase
{
    [Inject] protected IDataGridManager<PackageSourceOptions> DataGridManager { get; set; } = default!;
    [Inject] private IOptionsSnapshot<PackagesOptions> PackagesOptions { get; set; } = default!;
    [Inject] private IWritableOptionsService<PackagesOptions> WritablePackagesOptions { get; set; } = default!;

    protected override void OnInitialized()
    {
        DataGridManager.Title = L["Package Source"];
        DataGridManager.DefaultSort = new() { [nameof(PackageSourceOptions.Name)] = false };
        DataGridManager.QueryAsync = async () => await Task.FromResult(PackagesOptions.Value.Sources);

        DataGridManager.SaveAsync = async (item, isNew) =>
        {
            if (isNew) { PackagesOptions.Value.Sources.Add(item); }
            WritablePackagesOptions.Update(PackagesOptions.Value);
            return await Task.FromResult(true);
        };

        DataGridManager.DeleteAsync = async (items) =>
        {
            foreach (var item in items) { PackagesOptions.Value.Sources.Remove(item); }
            WritablePackagesOptions.Update(PackagesOptions.Value);
            return await Task.FromResult(true);
        };
    }
}