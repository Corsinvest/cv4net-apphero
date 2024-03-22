/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.JSInterop;

namespace Corsinvest.AppHero.Core.Service;

public class BrowserService : IBrowserService
{
    private readonly IJSRuntime _jSRuntime;
    public BrowserService(IJSRuntime jSRuntime) => _jSRuntime = jSRuntime;
    public async Task CopyToClipboard(string text) => await _jSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    public async Task Open(string url, string target) => await _jSRuntime.InvokeVoidAsync("open", url, target);
}