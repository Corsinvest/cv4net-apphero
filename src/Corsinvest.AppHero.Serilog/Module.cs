/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Serilog;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Serilog";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Serilog";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) => services.AddTransient<Middleware>();

    public override async Task OnApplicationInitializationAsync(IHost host)
    {
        var app = (WebApplication)host;
        app.UseMiddleware<Middleware>();
        await Task.CompletedTask;
    }
}