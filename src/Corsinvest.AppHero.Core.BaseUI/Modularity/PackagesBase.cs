/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.BaseUI.Modularity;

public partial class PackagesBase : AHComponentBase
{
    [Inject] protected IDataGridManager<PackageDto> DataGridManager { get; set; } = default!;
    [Inject] private IWritableOptionsService<PackagesOptions> WritablePackagesOptions { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IUINotifier UINotifier { get; set; } = default!;
    [Inject] private IUIMessageBox UIMessageBox { get; set; } = default!;

    protected bool ShowWait { get; private set; }
    protected List<string> Errors { get; } = new();

    protected override void OnInitialized()
    {
        DataGridManager.Title = L["Package"];
        DataGridManager.QueryAsync = async () =>
        {
            StateHasChanged();

            Errors.Clear();
            var ret = await ModularityService.GetPackagesAsync(WritablePackagesOptions.Value);
            Errors.AddRange(ret.Errors.Select(a => a.Message));

            StateHasChanged();
            return ret.Value;
        };
    }

    protected async Task InstallerAsync(PackageDto data, bool install)
    {
        if (await UIMessageBox.ShowQuestionAsync(string.Format("{0} result?", install ? L["Install"] : L["Uninstall"]), L["Confirm?"]))
        {
            ShowWait = true;
            var packagesOptions = WritablePackagesOptions.Value!;
            var package = packagesOptions.Packages.FirstOrDefault(a => a.Id == data.Id);
            if (install)
            {
                //install
                Logger.LogInformation("Install Package {Id}", data.Id);
                if (package == null)
                {
                    package = new PackageOptions
                    {
                        Id = data.Id,
                        IsNuGetPackage = true,
                    };
                    packagesOptions.Packages.Add(package);
                }
                package.IsNuGetPackage = true;
                package.Version = data.CurrentVersion!;
            }
            else
            {
                //Uninstall
                Logger.LogInformation("Uninstall Package {Id}", data.Id);

                if (package != null) { packagesOptions.Packages.Remove(package); }
            }

            WritablePackagesOptions.Update(packagesOptions);
            ShowWait = false;
            await DataGridManager.Refresh();

            UINotifier.Show(L["Package configuration updated. Restart application!"], UINotifierSeverity.Success);
        }
    }
}