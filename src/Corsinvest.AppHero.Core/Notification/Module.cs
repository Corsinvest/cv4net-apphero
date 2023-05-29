/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Notification;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Notification";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Notifications";

        Roles = new Role[]
        {
            new("",
                "",
                Permissions.Notification.Data.Permissions
                    .Union(new[] { Permissions.Notification.Test }))
        };
    }

    public static class Permissions
    {
        public static class Notification
        {
            public static PermissionsCrud Data { get; } = new($"{typeof(Module).FullName}.{nameof(Notification)}.{nameof(Data)}");
            public static Permission Test { get; } = new($"{Data.Prefix}.{nameof(Test)}", "Test", UIIcon.Telegram.GetName(), UIColor.Success);
        }

        public static Permission Save { get; } = new($"{typeof(Module).FullName}.{nameof(Save)}", "Save", null!, UIColor.Success);
    }
}