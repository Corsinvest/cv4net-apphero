/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleLink : ModuleLinkTreeBase<ModuleLink>
{
    public ModuleLink(ModuleBase module, string text, string url = "", bool isExternal = false) : base(module, text, url, isExternal) { }
    public ModuleLink(ModuleLink parent, string text, string url = "", bool isExternal = false) : base(parent, text, url, isExternal) { }

    protected override string Name { get; } = "NavMenu";
    public Type Render { get; set; } = default!;
}