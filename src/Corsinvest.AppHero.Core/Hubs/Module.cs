/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Core.Hubs;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "SignaR";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "SignaR";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
        => services.AddScoped<HubClient>()
                   .AddSignalR();

    public override async Task OnPostApplicationInitializationAsync(IHost host)
    {
        var app = (WebApplication)host;
        app.MapHub<AHHub>(SignalRConstants.HubUrl);
        await Task.CompletedTask;
    }
}