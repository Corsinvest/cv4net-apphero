/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Notification.Email;

public class Module : ModuleBase, IForceLoadModule, INotification
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Notification,Mail";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Icon = UIIcon.Email.GetName();
        Description = "Email";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) => AddOptions<Options>(services, config);
}