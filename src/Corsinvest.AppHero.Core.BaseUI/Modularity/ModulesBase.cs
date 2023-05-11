/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.BaseUI.Modularity;

public abstract class ModulesBase : AHComponentBase
{
    protected class Data
    {
        public bool Enabled { get; set; }
        public bool IsChanged => Enabled != IsAvailable;
        public bool ExistRenderOptions { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsSystem { get; set; }
        public string Category { get; set; } = default!;
        public ModuleType Type { get; set; }
        public string Description { get; set; } = default!;
        public string Keywords { get; set; } = default!;
        public string Version { get; set; } = default!;
        public string Class { get; set; } = default!;
        public string Slug { get; set; } = default!;
    }

    [Inject] protected IDataGridManager<Data> DataGridManager { get; set; } = default!;
    [Inject] protected IOptionsSnapshot<PackagesOptions> PackagesOptions { get; set; } = default!;
    [Inject] protected IModularityService ModularityService { get; set; } = default!;
    [Inject] private IWritableOptionsService<PackagesOptions> WritablePackagesOptions { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IUINotifier UINotifier { get; set; } = default!;

    protected IEnumerable<Data> Items { get; private set; } = default!;

    protected override void OnInitialized()
    {
        Items = ModularityService.Modules.Select(a => new Data
        {
            Class = a.Class,
            ExistRenderOptions = !(a.Options?.Render == null),
            IsAvailable = a.Enabled,
            IsSystem = a.ForceLoad,
            Category = a.Category,
            Type = a.Type,
            Description = a.Description,
            Keywords = a.Keywords,
            Version = a.Version.ToString(),
            Enabled = a.Enabled,
            Slug = a.Slug == a.Class.Replace(".", "") ? string.Empty : a.Slug,
        }).ToList();

        DataGridManager.Title = L["Module"];

        DataGridManager.DefaultSort = new()
        {
            [nameof(Data.IsSystem)] = false,
            [nameof(Data.Category)] = false,
            [nameof(Data.Type)] = false,
            [nameof(Data.Description)] = false
        };

        DataGridManager.QueryAsync = async () =>
        {
            StateHasChanged();
            return await Task.FromResult(Items);
        };
    }

    protected void Save()
    {
        var packagesOptions = PackagesOptions.Value;
        foreach (var item in Items.Where(a => !a.IsSystem))
        {
            ModularityService.GetByClass(item.Class)!.Enabled = item.Enabled;

            var module = packagesOptions.Packages.SelectMany(a => a.Modules)
                                                 .First(a => a.Class == item.Class)!;
            module.Enabled = item.Enabled;
        }

        WritablePackagesOptions.Update(packagesOptions);

        UINotifier.Show(L["Module configuration updated.!"], UINotifierSeverity.Success);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    protected abstract Task ShowOptionsAsync(string @class);
}
