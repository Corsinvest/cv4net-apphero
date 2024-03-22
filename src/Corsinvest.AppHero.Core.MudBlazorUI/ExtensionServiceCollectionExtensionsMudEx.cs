/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

using MudExtensions.Services;

namespace Corsinvest.AppHero.Core.MudBlazorUI;

internal static class ExtensionServiceCollectionExtensionsMudEx
{
    public static IServiceCollection AddMudExtensionsMudEx(this IServiceCollection services) => services.AddMudExtensions();
}