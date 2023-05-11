/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.BaseUI.Extensions;

public static class ModularityExtensions
{
    public static void SetRenderIndex<TModule, TIndex>(this IModularityService modularityService)
        where TModule : ModuleBase
        where TIndex : ComponentBase
        => modularityService.Get<TModule>()!.SetRenderIndex<TIndex>();

    public static void SetRenderOptions<TModule, TRenderOptions>(this IModularityService modularityService)
        where TModule : ModuleBase
        where TRenderOptions : ComponentBase
        => modularityService.Get<TModule>()!.SetRenderOptions<TRenderOptions>();
}