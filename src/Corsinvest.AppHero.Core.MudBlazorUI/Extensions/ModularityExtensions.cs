/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Extensions;

public static class ModularityExtensions
{
    public static string ToMBIcon(this IModularityService modularityService, string category)
        => MudBlazorHelper.ToMBIcon(modularityService.GetCategoryIcon(category));

    public static string ToMBIcon(this IGroupableModule groupableService) => MudBlazorHelper.ToMBIcon(groupableService.GetGroupIcon());
    public static string ToMBIcon(this ModuleMenuItem menuItem) => MudBlazorHelper.ToMBIcon(menuItem.Icon);
    public static string ToMBIcon(this ModuleLink link) => MudBlazorHelper.ToMBIcon(link.Icon);
    public static string ToMBIcon(this ModuleBase module) => MudBlazorHelper.ToMBIcon(module.Icon);
    public static string ToMBIcon(this Permission permission) => MudBlazorHelper.ToMBIcon(permission.Icon);
}