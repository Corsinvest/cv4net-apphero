/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity.Packages;

public class PackageDto
{
    public bool IsInstallated { get; set; }
    public IEnumerable<Version> Versions { get; set; } = default!;
    public Version? CurrentVersion { get; set; }
    public long DownloadCount { get; set; }
    public string IconUrl { get; set; } = default!;
    public string Owners { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string Feed { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Tags { get; set; } = default!;
    public string Authors { get; set; } = default!;
}