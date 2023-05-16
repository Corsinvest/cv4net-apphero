/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Semver;

namespace Corsinvest.AppHero.Core.SoftwareRelease;

public interface IReleaseService
{
    Task<IEnumerable<RleaseInfo>> GetReleasesAsync();

    private static DateTime? LastRun { get; set; }
    private static RleaseInfo? LastValue { get; set; }

    public async Task<RleaseInfo?> NewReleaseIsAvaibleAsync()
    {
        if (LastRun == null || (DateTime.Now - LastRun).Value.Minutes > 5)
        {
            LastRun = DateTime.Now;

            if (SemVersion.TryParse(ApplicationHelper.ProductVersion, SemVersionStyles.Any, out var currSemVer))
            {
                IEnumerable<RleaseInfo>? data = null;
                try
                {
                    data = await GetReleasesAsync();
                }
                catch { }

                foreach (var item in data ?? new List<RleaseInfo>())
                {
                    if (SemVersion.TryParse(item.Version, SemVersionStyles.Any, out var semVer))
                    {
                        if (currSemVer.ComparePrecedenceTo(semVer) == -1)
                        {
                            LastValue = item;
                            break;
                        }
                    }
                }
            }
        }

        return LastValue;
    }
}
