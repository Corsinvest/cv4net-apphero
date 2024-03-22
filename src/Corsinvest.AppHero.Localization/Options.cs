/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Localization;

public class Options
{
    public TypeArchive Type { get; set; } = TypeArchive.DataBase;
    public bool CreateKeyIfNotExists { get; set; } = true;
    public bool SplitResourceByType { get; set; } = false;
    public HashSet<string> SupportedCultures { get; set; } = ["en-US", "it-IT"];
    public bool LocalizeDefaultCulture { get; set; } = false;
    public string DefaultCulture { get; set; } = "en-US";
    public string ResourcesPath { get; set; } = Path.Combine(new Module().PathData);
    public string CacheKeyPrefix { get; set; } = "L10N_";
}