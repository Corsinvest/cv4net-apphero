/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: Impl

using Corsinvest.AppHero.Localization.Types.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Localization.Types.Database;

public class Localizer : BaseStringLocalizer
{
    private readonly LocalizationDbContext _db;
    private readonly ILogger<Localizer> _logger;

    public Localizer(IDistributedCache cache,
                     IOptions<Options> options,
                     Type resourceType,
                     LocalizationDbContext db,
                     ILoggerFactory loggerFactory)
        : base(cache, options, resourceType, loggerFactory)
    {
        _db = db;
        _logger = loggerFactory.CreateLogger<Localizer>();
    }

    public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _db.Localizations
                .AsNoTracking()
                .Where(a => a.Context == Context, Options.Value.SplitResourceByType)
                .Select(a => new LocalizedString(a.Key, a.Value, false))
                .ToArray();

    protected override string GetStringImp(string key)
    {
        // Look in db source
        var value = _db.Localizations
                       .AsNoTracking()
                       .Where(a => a.Key == key && a.CultureName == CurrentCultureName)
                       .Where(a => a.Context == Context, Options.Value.SplitResourceByType)
                       .Select(a => a.Value)
                       .FirstOrDefault();

        var found = value != null;
        if (!found)
        {
            _logger.LogInformation("Localization not exist! Culture: '{CultureName}', Context: '{Context}', Key: '{key}'",
                                   CurrentCultureName,
                                   Context,
                                   key);

            if (CreateKeyIfNotExists)
            {
                var loc = new Models.Localization
                {
                    Key = key,
                    Context = Options.Value.SplitResourceByType ? Context : null,
                    Value = string.Empty,
                    CultureName = CurrentCultureName,
                };
                _db.Localizations.Add(loc);

                var success = false;
                try
                {
                    success = _db.SaveChanges() > 0;
                    if (success)
                    {
                        value = key;
                        _logger.LogInformation("New resource added. Culture: '{CultureName}', Context: '{Context}', Key: '{}'",
                                               loc.CultureName,
                                               loc.Context,
                                               loc.Key);
                    }
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);
                    success = false;
                }

                _logger.LogInformation("Save resource to db, status: '{success}', context: {Context}, key: '{key}', value: '{value}'",
                                       success,
                                       Context,
                                       key,
                                       value ?? key);
            }
        }

        return value!;
    }
}