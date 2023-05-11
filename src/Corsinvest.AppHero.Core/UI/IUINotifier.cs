/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.UI;

public interface IUINotifier
{
    void Show(string message, UINotifierSeverity type, string title = null!, string icon = null!);
    void Show(bool condition, string trueMessage, string falseMessage, string title = null!);
}