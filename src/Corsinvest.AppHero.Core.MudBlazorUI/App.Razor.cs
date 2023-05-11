/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.JSInterop;
using MudBlazor.Extensions.Helper;

namespace Corsinvest.AppHero.Core.MudBlazorUI;

public partial class App
{
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) { await JSRuntime.InitializeMudBlazorExtensionsAsync(true); }
        await base.OnAfterRenderAsync(firstRender);
    }
}
