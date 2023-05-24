/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.Notification.Discord;

public class Module : ModuleBase, IForceLoadModule, INotification
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Notification,Discord";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Icon = UIIcon.Discord.GetName();
        Description = "Discord";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) => AddOptions<Options>(services, config);
}