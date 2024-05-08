/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Modularity;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Module,Package";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Modularity";
        Slug = "Modularity";
        InfoText = "Managemnt modules for application";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.Extension.GetName(),
        };

        Roles =
        [
            new("",
                "",
                Permissions.Module.Data.Permissions
                .Union(
                [
                    Permissions.Module.Save,
                    Permissions.Module.Info,
                    Permissions.Module.Options,
                    Permissions.Module.EnableDisable,
                ])
                .Union(Permissions.Package.Data.Permissions)
                .Union([Permissions.Package.InstallUninstall])
                .Union(Permissions.PackagesSource.Data.Permissions))
        ];
    }

    public class Permissions
    {
        public class Module
        {
            public static PermissionsRead Data { get; } = new($"{typeof(Module).FullName}.{nameof(Module)}.{nameof(Data)}");
            public static Permission Save { get; } = new($"{Data.Prefix}.{nameof(Save)}", "Save", UIIcon.Save.GetName());
            public static Permission Info { get; } = new($"{Data.Prefix}.{nameof(Info)}", "Info", UIIcon.Info.GetName());
            public static Permission Options { get; } = new($"{Data.Prefix}.{nameof(Options)}", "Options", UIIcon.Settings.GetName(), UIColor.Info);
            public static Permission EnableDisable { get; } = new($"{Data.Prefix}.{nameof(EnableDisable)}", "Enable/Disable");
        }

        public class Package
        {
            public static PermissionsRead Data { get; } = new($"{typeof(Module).FullName}.{nameof(Package)}.{nameof(Data)}");
            public static Permission InstallUninstall { get; } = new($"{Data.Prefix}.{nameof(InstallUninstall)}", "Install/Uninstall");
        }

        public class PackagesSource
        {
            public static PermissionsCrud Data { get; } = new($"{typeof(Module).FullName}.{nameof(PackagesSource)}.{nameof(Data)}");
        }
    }
}