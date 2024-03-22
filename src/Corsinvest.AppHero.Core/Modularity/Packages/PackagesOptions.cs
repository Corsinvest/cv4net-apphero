/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity.Packages;

public class PackagesOptions
{
    public List<PackageOptions> Packages { get; set; } = [];
    public List<PackageSourceOptions> Sources { get; set; } = [];

    public void AddOrUpdateSource(PackageSourceOptions source)
    {
        var item = Sources.FirstOrDefault(a => a.Name == source.Name);
        if (item == null)
        {
            Sources.Add(source);
        }
        else
        {
            item.Username = source.Username;
            item.Password = source.Password;
            item.Packages.Clear();
            item.Packages.AddRange(source.Packages);
        }
    }
}