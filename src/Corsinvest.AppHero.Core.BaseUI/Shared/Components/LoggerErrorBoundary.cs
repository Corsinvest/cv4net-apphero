/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Components.Web;

namespace Corsinvest.AppHero.Core.BaseUI.Shared.Components;

public class AHLoggerErrorBoundary : ErrorBoundary
{
    [Inject] ILogger<AHLoggerErrorBoundary> Logger { get; set; } = null!;

    protected override async Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, string.Empty);
        await base.OnErrorAsync(exception);
    }
}
