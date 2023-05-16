/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Corsinvest.AppHero.Core.SoftwareRelease;

public class GitHubReleaseService : IReleaseService
{
    private readonly IOptionsSnapshot<AppOptions> _appOptions;
    public GitHubReleaseService(IOptionsSnapshot<AppOptions> appOptions) => _appOptions = appOptions;

    public async Task<IEnumerable<RleaseInfo>> GetReleasesAsync()
    {
        var repo = _appOptions.Value.RepoGitHub;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("cv4net-apphero", "1.0"));
        var data = await client.GetStringAsync($"https://api.github.com/repos/{repo}/releases");

        return JsonConvert.DeserializeObject<List<Data>>(data)!
                          .Where(a => !a.Draft)
                          .Select(a => new RleaseInfo()
                          {
                              Prerelease = a.Prerelease,
                              Url = a.HtmlUrl,
                              PublishedAt = a.PublishedAt,
                              Version = a.TagName
                          })
                          .ToArray();
    }

    class Data
    {
        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; } = default!;

        [JsonProperty("tag_name")]
        public string TagName { get; set; } = default!;

        [JsonProperty("draft")]
        public bool Draft { get; set; }

        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }
    }
}