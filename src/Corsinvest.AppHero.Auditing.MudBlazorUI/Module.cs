/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.BaseUI.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Auditing.MudBlazorUI;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Auditing,MudBlazor UI";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Auditing,MudBlazorUI";
    }

    public override async Task OnApplicationInitializationAsync(IHost host)
    {
        await Task.CompletedTask;
        var modularityService = host.Services.GetRequiredService<IModularityService>();
        modularityService.SetRenderIndex<Corsinvest.AppHero.Auditing.Module, RenderIndex>();
    }
}