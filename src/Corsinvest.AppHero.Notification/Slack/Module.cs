/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.Notification.Slack;

public class Module : ModuleBase, IForceLoadModule, INotification
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Notification,Slack";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Icon = "fa-brands fa-slack";
        Description = "Slack";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) => AddOptions<Options>(services, config);
}