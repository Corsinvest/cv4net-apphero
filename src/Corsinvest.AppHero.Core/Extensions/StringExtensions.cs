/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Text.RegularExpressions;

namespace Corsinvest.AppHero.Core.Extensions;

public static partial class StringExtensions
{
    public static string[] SplitNewLine(this string value) => value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    public static bool IsNumeric(this string value) => decimal.TryParse(value, out _);
    public static string ToCamelCaseWithSpace(this string value) => CamelCaseWithSpaceRegex().Replace(value + "", " $1");

    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex CamelCaseWithSpaceRegex();
}
