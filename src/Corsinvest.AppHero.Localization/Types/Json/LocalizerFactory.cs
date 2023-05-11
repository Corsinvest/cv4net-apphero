/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Localization.Types.Json;

public class LocalizerFactory : IStringLocalizerFactory
{
    private readonly IDistributedCache _cache;
    private readonly IOptions<Options> _options;
    private readonly ILoggerFactory _loggerFactory;

    public LocalizerFactory(IDistributedCache cache, IOptions<Options> options, ILoggerFactory loggerFactory)
    {
        _cache = cache;
        _options = options;
        _loggerFactory = loggerFactory;
    }

    public IStringLocalizer Create(Type resourceSource) => new Localizer(_cache, _options, resourceSource, _loggerFactory);
    public IStringLocalizer Create(string baseName, string location) => new Localizer(_cache, _options, null!, _loggerFactory);
}
