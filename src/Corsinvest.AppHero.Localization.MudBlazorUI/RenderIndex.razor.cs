/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.BaseUI.DataManager;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Translator;
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Localization.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using DomainLocalization = Corsinvest.AppHero.Localization.Types.Database.Models.Localization;

namespace Corsinvest.AppHero.Localization.MudBlazorUI;

public partial class RenderIndex
{
    [Inject] private IOptionsSnapshot<Options> Options { get; set; } = default!;
    [Inject] private IDistributedCache Cache { get; set; } = default!;
    [Inject] private IEnumerable<ITranslator> Translators { get; set; } = default!;
    [Inject] private IDataGridManagerRepository<DomainLocalization> DataGridManager { get; set; } = default!;
    [Inject] private IUIMessageBox MessageBox { get; set; } = default!;
    [Inject] private IServiceScopeFactory ServiceScopeFactory { get; set; } = default!;

    private bool LoadingLocalization { get; set; }

    protected override void OnInitialized()
    {
        DataGridManager.Title = L["Localization"];
        DataGridManager.DefaultSort = new()
        {
            [nameof(DomainLocalization.CultureName)] = false,
            [nameof(DomainLocalization.Key)] = false,
        };

        DataGridManager.SaveAfterAsync = async (item, isNew) => await Cache.SetStringAsync(GetCacheKey(item), item.Value);
        DataGridManager.DeleteAfterAsync = async (items) => await items.ForEachAsync(a => Cache.RemoveAsync(GetCacheKey(a)));
    }

    private string GetCacheKey(DomainLocalization item)
        => BaseStringLocalizer.GetCacheKey(Options.Value, item.CultureName, item.Context!, item.Key);

    private async Task TranslateAsync(ITranslator translator)
    {
        if (await MessageBox.ShowQuestionAsync(L["Translation"], L["Confirm translation of selection?"]))
        {
            LoadingLocalization = true;
            StateHasChanged();

            var errors = new StringBuilder();

            var result = true;
            using var scope = ServiceScopeFactory.CreateScope();
            foreach (var item in DataGridManager.SelectedItems.ToArray().GroupBy(a => a.CultureName))
            {
                var ret = await translator.TranslateAsync(scope,
                                                          Options.Value.DefaultCulture[..2],
                                                          item.Key[..2],
                                                          item.Select(a => a.Key));
                if (ret.IsSuccess)
                {
                    var values = item.ToList();
                    var translated = ret.Value.ToList();
                    for (int i = 0; i < values.Count; i++) { values[i].Value = translated[i]; }
                    await DataGridManager.Repository.UpdateRangeAsync(values);
                }
                else
                {
                    result = false;
                    ret.Errors.ForEach(a => errors.AppendLine(a.Message));
                    break;
                }
            }

            DataGridManager.SelectedItems.Clear();
            await DataGridManager.Refresh();
            LoadingLocalization = false;
            StateHasChanged();

            if (!result) { await MessageBox.ShowErrorAsync(L["Translation"], errors.ToString()); }
        }
    }
}