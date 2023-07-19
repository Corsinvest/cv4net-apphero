/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleCategory
{
    public ModuleCategory(string name, string icon, int order)
    {
        Name = name;
        Icon = icon;
        Order = order;
    }

    public string Name { get; }
    public string Icon { get; set; }
    public int Order { get; set; }
}
