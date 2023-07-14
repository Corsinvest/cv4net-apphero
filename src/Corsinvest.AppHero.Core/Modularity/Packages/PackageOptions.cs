/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity.Packages;

public class PackageOptions
{
    public string Id { get; set; } = default!;
    public Version Version { get; set; } = default!;
    public bool IsNuGetPackage { get; set; }
    public List<PackageModuleOptions> Modules { get; } = new();
}
