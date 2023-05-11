/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Services;

public class UIMessageBox : IUIMessageBox
{
    private readonly IDialogService _dialogService;

    public UIMessageBox(IDialogService dialogService) => _dialogService = dialogService;

    public async Task ShowErrorAsync(string title, string message) => await _dialogService.ShowErrorAsync(title, message);
    public async Task ShowInfoAsync(string title, string message) => await _dialogService.ShowInfoAsync(title, message);
    public async Task<bool> ShowQuestionAsync(string title, string message) => await _dialogService.ShowQuestionAsync(title, message);
}