/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using Semver;

namespace Corsinvest.AppHero.Core.SoftwareRelease;

public class DockerHubReleaseService : IReleaseService
{
    private readonly IOptionsSnapshot<AppOptions> _appOptions;

    public DockerHubReleaseService(IOptionsSnapshot<AppOptions> appOptions) => _appOptions = appOptions;

    public async Task<IEnumerable<RleaseInfo>> GetReleasesAsync()
    {
        var repo = _appOptions.Value.RepoDockerHub;

        using var client = new HttpClient();
        var data = await client.GetStringAsync($"https://registry.hub.docker.com/v2/repositories/{repo}/tags");

        return JsonConvert.DeserializeObject<Data>(data)!.Results
                          .Select(a => new RleaseInfo()
                          {
                              Prerelease = SemVersion.Parse(a.Name, Semver.SemVersionStyles.Any).IsPrerelease,
                              Url = $"https://hub.docker.com/r/{repo}/tags",
                              PublishedAt = a.TagLastPushed,
                              Version = a.Name
                          })
                          .ToArray();
    }

    class Data
    {
        [JsonProperty("results")]
        public List<Result_> Results { get; set; } = new();

        public class Result_
        {
            [JsonProperty("name")]
            public string Name { get; set; } = default!;

            [JsonProperty("tag_last_pushed")]
            public DateTime TagLastPushed { get; set; }
        }
    }
}