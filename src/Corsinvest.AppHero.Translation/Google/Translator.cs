/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Corsinvest.AppHero.Translation.Google;

public class Translator
{
    private class WebResult
    {
        [JsonProperty("data")]
        public Data_ Data { get; set; } = default!;

        public class Data_
        {
            [JsonProperty("translations")]
            public Translation_[] Translations { get; set; } = default!;

            public class Translation_
            {
                [JsonProperty("translatedText")]
                public string TranslatedText { get; set; } = default!;
            }
        }
    }

    public static async Task<IResult<IEnumerable<string>>> TranslateAsync(ILogger<Translator> logger,
                                                                          Options options,
                                                                          string source,
                                                                          string targets,
                                                                          IEnumerable<string> texts)
    {
        var oks = new List<string>();
        var fails = new List<string>();
        IResult<IEnumerable<string>> ret;

        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-rapidapi-key", options.RapidApiKey);
            client.DefaultRequestHeaders.Add("x-rapidapi-host", "google-translate1.p.rapidapi.com");

            foreach (var item in texts)
            {
                var response = await client.PostAsync("https://google-translate1.p.rapidapi.com/language/translate/v2",
                                                new FormUrlEncodedContent(new Dictionary<string, string>
                                                {
                                                            { "q", item },
                                                            { "target", targets },
                                                            { "source", source },
                                                }));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<WebResult>();
                    oks.Add(result!.Data.Translations[0].TranslatedText);
                }
                else
                {
                    fails.Add(response.ReasonPhrase!);
                }
            }

            ret = fails.Count == 0
                    ? Result.Ok(oks)
                    : Result.Fail(fails);

        }
        catch (Exception ex)
        {
            ret = Result.Fail<IEnumerable<string>>(ex.Message);
            logger.LogError(ex, ex.Message);
        }

        return ret;
    }
}
