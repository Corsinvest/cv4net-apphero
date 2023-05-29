/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Options;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Options";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Options";
        Slug = "Options";
        InfoText = "Customize application options";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.Settings.GetName()
        };

        Roles = new Role[]
        {
            new("","",new[] { Permissions.Save })
        };
    }

    public static class Permissions
    {
        public static Permission Save { get; } = new($"{typeof(Module).FullName}.{nameof(Save)}", "Save", null!, UIColor.Success);
    }
}