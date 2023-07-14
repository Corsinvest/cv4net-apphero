/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

namespace Corsinvest.AppHero.Translation.Microsoft;


public class Translator
{
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
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-rapidapi-key", options.RapidApiKey);
            client.DefaultRequestHeaders.Add("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");

            var response = await client.PostAsync($"https://microsoft-translator-text.p.rapidapi.com/translate?from={source}&to={targets}&api-version=3.0&profanityAction=NoAction&textType=plain",
                                                  new StringContent(JsonConvert.SerializeObject(texts.Select(a => new { Text = a }).ToArray()))
                                                  {
                                                      Headers = { ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json) }
                                                  });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WebResult[]>();
                ret = Result.Ok(result!.Select(a => a.Translations[0].Text));
            }
            else
            {
                ret = Result.Fail<IEnumerable<string>>(response.ReasonPhrase);
            }
        }
        catch (Exception ex)
        {
            ret = Result.Fail<IEnumerable<string>>(ex.Message);
            logger.LogError(ex, ex.Message);
        }

        return ret;
    }

    class WebResult
    {
        [JsonProperty("translations")]
        public Translation_[] Translations { get; set; } = default!;

        public class Translation_
        {
            [JsonProperty("text")]
            public string Text { get; set; } = default!;

            [JsonProperty("to")]
            public string To { get; set; } = default!;
        }
    }
}