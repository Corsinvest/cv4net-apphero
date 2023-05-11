/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Web;

namespace Corsinvest.AppHero.Core.Helpers;

public static class UrlHelper
{
    public static string SetParameter(string url, string name, string value) => SetParameter(url, new() { { name, value } });

    public static string SetParameter(string url, Dictionary<string, string> values)
    {
        var separateURL = url.Split('?');
        var queryString = HttpUtility.ParseQueryString(separateURL.Length > 1 ? separateURL[1] : "");
        foreach (var (key, value) in values) { queryString[key] = value; }
        return separateURL[0] + "?" + queryString.ToString();
    }
}