/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: Impl

namespace Corsinvest.AppHero.Localization.Types;

public abstract class BaseStringLocalizer : IStringLocalizer
{
    private readonly IDistributedCache _cache;

    protected IOptions<Options> Options { get; }
    public ILoggerFactory LoggerFactory { get; }
    protected Type ResourceType { get; }
    protected string Context => ResourceType.FullName!;
    protected string CurrentCultureName { get; }


    protected bool CreateKeyIfNotExists
        => (Options.Value.LocalizeDefaultCulture || Options.Value.DefaultCulture != CurrentCultureName)
            && Options.Value.CreateKeyIfNotExists;

    public BaseStringLocalizer(IDistributedCache cache,
                               IOptions<Options> options,
                               Type resourceType,
                               ILoggerFactory loggerFactory)
    {
        _cache = cache;
        Options = options;
        LoggerFactory = loggerFactory;
        ResourceType = resourceType;
        CurrentCultureName = Thread.CurrentThread.CurrentCulture.Name;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                : actualValue;
        }
    }

    public abstract IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);

    public static string GetCacheKey(Options options,
                                     string cultureName,
                                     string context,
                                     string key)
    {
        var cacheKey = $"{options.CacheKeyPrefix}{cultureName}_";
        if (options.SplitResourceByType) { cacheKey += $"{context}_"; }
        cacheKey += key;
        return cacheKey;
    }

    protected virtual string GetString(string key)
    {
        if (!CreateKeyIfNotExists) { return key; }

        var cacheKey = GetCacheKey(Options.Value, CurrentCultureName, Context, key);
        var cacheValue = _cache.GetString(cacheKey);
        if (!string.IsNullOrEmpty(cacheValue)) { return cacheValue; }
        var result = GetStringImpl(key);
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result)) { result = key; }
        _cache.SetString(cacheKey, result);
        return result;
    }

    protected abstract string GetStringImpl(string key);
}