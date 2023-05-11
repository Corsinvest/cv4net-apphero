/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Localization.Types.Database.Persistence;

namespace Corsinvest.AppHero.Localization.Types.Database;

public class LocalizerFactory : IStringLocalizerFactory
{
    private readonly IDistributedCache _cache;
    private readonly IOptions<Options> _options;
    private readonly LocalizationDbContext _db;
    private readonly ILoggerFactory _loggerFactory;

    public LocalizerFactory(IDistributedCache cache, IOptions<Options> options, LocalizationDbContext db, ILoggerFactory loggerFactory)
    {
        _cache = cache;
        _options = options;
        _db = db;
        _loggerFactory = loggerFactory;
    }

    public IStringLocalizer Create(Type resourceSource) => new Localizer(_cache, _options, resourceSource, _db, _loggerFactory);
    public IStringLocalizer Create(string baseName, string location) => new Localizer(_cache, _options, null!, _db, _loggerFactory);
}