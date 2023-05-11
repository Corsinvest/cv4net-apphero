/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleWidget
{
    public ModuleWidget(ModuleBase module, string name)
    {
        Module = module;
        Name = name;
        Permission = new($"{module.PermissionWidgetBaseKey}.{Name}", Name, UIIcon.Widget.GetName());
    }

    public string Name { get; }
    public ModuleBase Module { get; }
    public string? Class { get; set; } = null!;
    public string GroupName { get; set; } = string.Empty;
    public Type Render { get; init; } = default!;
    public ModuleLink? RefLink => Module.Link;
    public Permission Permission { get; }
    public bool ShowDefaultHeader { get; set; } = true;
}