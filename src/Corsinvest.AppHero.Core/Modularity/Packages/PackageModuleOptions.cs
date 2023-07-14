/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity.Packages;

public class PackageModuleOptions
{
    public string Class { get; set; } = default!;
    public bool Enabled { get; set; }
}
