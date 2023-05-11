/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;
using Corsinvest.AppHero.Core.MudBlazorUI.Style;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages;

[Authorize]
public partial class Index
{
    [Inject] private IOptionsSnapshot<AppOptions> AppOptions { get; set; } = default!;
    [Inject] private IOptionsSnapshot<UIOptions> UIOptions { get; set; } = default!;

    private Type Render { get; set; } = typeof(AHWidgets);
    protected override void OnInitialized() => Render = TypeHelper.GetType(UIOptions.Value.ClassIndexPageComponent, typeof(AHWidgets));
}