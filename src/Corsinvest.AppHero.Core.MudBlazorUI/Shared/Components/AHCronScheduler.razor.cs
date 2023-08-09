/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Service;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHCronScheduler
{
    [Parameter][CronExpressionValidator] public string Expression { get; set; } = default!;
    [Parameter] public EventCallback<string> ExpressionChanged { get; set; } = default!;
    [Parameter] public Orientation Orientation { get; set; } = Orientation.Landscape;
    [Parameter] public bool ShowDescriptor { get; set; }

    [Inject] private IBrowserService BrowserService { get; set; } = default!;

    private int Size => Orientation == Orientation.Landscape ? 6 : 12;
    private string CronExpressionDescriptor => CronHelper.GetDescription(Expression);
    private void TextChanged(string value) => ExpressionChanged.InvokeAsync(value);
    private async Task OpenCrontabGuruAsync()
        => await BrowserService.Open($"https://crontab.guru/#{(Expression + "").Replace(" ", "_")}", "_blank");
}