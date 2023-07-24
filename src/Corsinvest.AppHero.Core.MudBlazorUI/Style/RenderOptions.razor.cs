/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Style;

public partial class RenderOptions : IDisposable
{
    [Inject] private LayoutService LayoutService { get; set; } = default!;

    protected override void OnInitialized() => LayoutService.MajorUpdateOccurred += LayoutService_MajorUpdateOccured;
    public void Dispose() => LayoutService.MajorUpdateOccurred -= LayoutService_MajorUpdateOccured;
    private void LayoutService_MajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
}