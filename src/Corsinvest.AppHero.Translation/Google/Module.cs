/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Translator;
using Microsoft.Extensions.Options;

namespace Corsinvest.AppHero.Translation.Google;

public class Module : ModuleBase, IForceLoadModule, ITranslator
{
    public Module()
    {
        Authors = "Corsinvest Srl";
        Company = "Corsinvest Srl";
        Keywords = "Translation,Google";
        Category = IModularityService.AdministrationCategoryName;
        Type = ModuleType.Service;
        Icon = UIIcon.Google.GetName();
        Description = "Google Translator RapidApi";
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config) => AddOptions<Options>(services, config);

    public async Task<IResult<IEnumerable<string>>> TranslateAsync(IServiceScope scope, string source, string targets, IEnumerable<string> texts)
        => await Translator.TranslateAsync(scope.ServiceProvider.GetRequiredService<ILogger<Translator>>(),
                                           scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<Options>>().Value,
                                           source,
                                           targets,
                                           texts);
}