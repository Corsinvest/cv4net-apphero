/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleMenuItem : ModuleLinkBase<ModuleMenuItem>
{
    public ModuleMenuItem(ModuleBase module, string text, string url = "", bool isExternal = false) : base(module, text, url, isExternal) { }
    protected override string Name { get; } = "MenuItem";
    public Action Action { get; set; } = default!;
}