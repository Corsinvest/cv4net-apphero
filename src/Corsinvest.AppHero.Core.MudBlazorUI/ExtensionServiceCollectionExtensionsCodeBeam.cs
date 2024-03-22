/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using MudBlazor.Extensions;

namespace Corsinvest.AppHero.Core.MudBlazorUI;

internal static class ExtensionServiceCollectionExtensionsCodeBeam
{
    public static IServiceCollection AddMudExtensionsCodeBeam(this IServiceCollection services) => services.AddMudExtensions();
}