/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Models;
using Newtonsoft.Json;

namespace Corsinvest.AppHero.Core.Modularity.Packages;

public class PackageSourceOptions : Credential
{
    public string Name { get; set; } = default!;
    public string Feed { get; set; } = default!;
    public List<string> Packages { get; } = new();

    [JsonIgnore]
    public string PackagesToString
    {
        get => Packages.JoinAsString("\n");
        set
        {
            Packages.Clear();
            Packages.AddRange((value + "")
                    .Split('\n')
                    .Where(a => !string.IsNullOrWhiteSpace(a)));
        }
    }
}