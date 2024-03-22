/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleCategory(string name, string icon, int order)
{
    public string Name { get; } = name;
    public string Icon { get; set; } = icon;
    public int Order { get; set; } = order;
}
