/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.BaseUI.Extensions;
using Corsinvest.AppHero.Core.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Localization.MudBlazorUI;

public class Module : ModuleBase, IForceLoadModule
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Localization,MudBlazor";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Description = "Localization MudBlazorUI";
    }

    public override async Task OnApplicationInitializationAsync(IHost host)
    {
        await Task.CompletedTask;
        var modularityService = host.Services.GetRequiredService<IModularityService>();
        modularityService.SetRenderIndex<Localization.Module, RenderIndex>();
    }
}