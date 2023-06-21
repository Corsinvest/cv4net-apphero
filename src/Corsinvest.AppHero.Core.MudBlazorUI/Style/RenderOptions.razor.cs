/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Style;

public partial class RenderOptions : IDisposable
{
    [Inject] private LayoutService LayoutService { get; set; } = default!;

    protected override void OnInitialized() => LayoutService.MajorUpdateOccured += LayoutService_MajorUpdateOccured;
    public void Dispose() => LayoutService.MajorUpdateOccured -= LayoutService_MajorUpdateOccured;
    private void LayoutService_MajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
}