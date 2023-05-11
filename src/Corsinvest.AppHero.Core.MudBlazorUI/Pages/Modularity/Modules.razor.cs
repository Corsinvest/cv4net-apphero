/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.BaseUI.Modularity;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Modularity;

public partial class Modules : ModulesBase
{
    protected override async Task ShowOptionsAsync(string @class)
        => await ModularityService.GetByClass(@class)!.ShowDialogOptionsAsync(DialogService);
}