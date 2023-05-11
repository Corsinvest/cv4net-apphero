/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Services;

public class UINotifier : IUINotifier
{
    private readonly ISnackbar _snackbar;

    public UINotifier(ISnackbar snackbar) => _snackbar = snackbar;

    public void Show(string message, UINotifierSeverity type, string title = null!, string icon = null!)
    {
        var snackType = type switch
        {
            UINotifierSeverity.Error => Severity.Error,
            UINotifierSeverity.Warning => Severity.Warning,
            UINotifierSeverity.Info => Severity.Info,
            UINotifierSeverity.Success => Severity.Success,
            _ => Severity.Success,
        };

        if (!string.IsNullOrWhiteSpace(title)) { message = $"<b>{title}</b><br />{message}"; }
        _snackbar.Add(message, snackType, config =>
        {
            if (icon != null) { config.Icon = icon; }
        });
    }

    public void Show(bool condition, string trueMessage, string falseMessage, string title = null!)
        => Show(condition ? trueMessage : falseMessage,
                condition ? UINotifierSeverity.Success : UINotifierSeverity.Error,
                title);
}