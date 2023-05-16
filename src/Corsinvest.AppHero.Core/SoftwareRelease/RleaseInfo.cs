/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.SoftwareRelease;

public class RleaseInfo
{
    public string? Url { get; set; }
    public bool Prerelease { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string Version { get; set; } = default!;
}
