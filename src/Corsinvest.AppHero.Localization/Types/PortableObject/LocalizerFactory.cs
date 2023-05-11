/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

//namespace Corsinvest.AppHero.Localization.Types.PortableObject;

//public class LocalizerFactory : IStringLocalizerFactory
//{
//    private readonly IDistributedCache _cache;
//    private readonly IOptions<Settings> _settings;
//    private readonly ILoggerFactory _loggerFactory;

//    public LocalizerFactory(IDistributedCache cache, IOptions<Settings> settings, ILoggerFactory loggerFactory)
//    {
//        _cache = cache;
//        _settings = settings;
//        _loggerFactory = loggerFactory;
//    }

//    public IStringLocalizer Create(Type resourceSource) => new Localizer(_cache, _settings, resourceSource, _loggerFactory);

//    public IStringLocalizer Create(string baseName, string location) => new Localizer(_cache, _settings, null!, _loggerFactory);
//}