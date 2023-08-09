/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.Notification.Hangouts;

public class Module : ModuleBase, IForceLoadModule, INotification
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Notification,Hangouts";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Icon = "fa-solid fa-quote-right";
        Description = "Hangouts";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) => AddOptions<Options>(services, config);
}