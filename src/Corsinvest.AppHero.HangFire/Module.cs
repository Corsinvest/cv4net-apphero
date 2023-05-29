/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.BackgroundJob;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.UI;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Corsinvest.AppHero.HangFire;

public class Module : ModuleBase, IForceLoadModule
{
    public static string BackgroundJobsUrl { get; } = "/BackgroundJobs-HangFire";

    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Job,Hangfire,Cron";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Background Jobs HangFire";

        Link = new ModuleLink(this, Description)
        {
            Icon = UIIcon.CalendarMonth.GetName(),
        };

        SetRenderIndex<RenderIndex>();
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IJobService, JobService>();

        services.AddHangfireConsoleExtensions();

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Module));

        logger.LogInformation("For more Hangfire storage, visit https://www.hangfire.io/extensions.html");

        services.AddSingleton<JobActivator, JobActivatorEx>();
        services.AddHangfire((provider, configuration) =>
        {
            //configuration.UseStorage(storage);
            configuration.UseFilter(new JobFilter(provider));
            configuration.UseFilter(new LogJobFilter());
            configuration.UseConsole();
        });
    }

    public override async Task OnPostApplicationInitializationAsync(IHost host)
    {
        var app = (WebApplication)host;
        app.MapHangfireDashboardWithAuthorizationPolicy(Link!.Permission.Key,
                                                        BackgroundJobsUrl,
                                                        new DashboardOptions
                                                        {
                                                            AppPath = null,
                                                            DashboardTitle = "Jobs",
                                                            DisplayStorageConnectionString = false,
                                                        });
        await Task.CompletedTask;
    }
}