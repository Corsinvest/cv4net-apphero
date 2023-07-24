/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Extensions;

public static class ModularityExtensions
{
    public static IEnumerable<ModuleBase> IsType(this IEnumerable<ModuleBase> modules, ModuleType type) => modules.Where(a => a.Type == type);
    public static IEnumerable<ModuleBase> IsCategory(this IEnumerable<ModuleBase> modules, string category) => modules.Where(a => a.Category == category);
    public static IEnumerable<ModuleBase> IsEnabled(this IEnumerable<ModuleBase> modules) => modules.Where(a => a.Enabled);
    public static IEnumerable<ModuleBase> IsConfigured(this IEnumerable<ModuleBase> modules) => modules.Where(a => a.Configured);
    public static IEnumerable<ModuleBase> Implements<T>(this IEnumerable<ModuleBase> modules) => modules.Where(a => typeof(T).IsAssignableFrom(a.GetType()));
    public static IEnumerable<ModuleBase> NotImplements<T>(this IEnumerable<ModuleBase> modules) => modules.Where(a => !typeof(T).IsAssignableFrom(a.GetType()));

    public static string GetCategoryIcon(this IModularityService modularityService, string category)
        => modularityService.Categories.FirstOrDefault(a => a.Name == category)?.Icon + "";
}