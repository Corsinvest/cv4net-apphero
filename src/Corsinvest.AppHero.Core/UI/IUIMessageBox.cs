/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.UI;

public interface IUIMessageBox
{
    Task<bool> ShowQuestionAsync(string title, string message);
    Task ShowInfoAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
}
