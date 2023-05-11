/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
    {
        var stack = new Stack<T>(items);
        while (stack.Any())
        {
            var next = stack.Pop();
            yield return next;
            foreach (var child in childSelector(next)) { stack.Push(child); }
        }
    }

    public static string JoinAsString(this IEnumerable<string> source, string separator) => string.Join(separator, source);

    public static string JoinAsString<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source);

    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        => condition
            ? source.Where(predicate)
            : source;

    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, bool condition)
        => condition
            ? source.Where(predicate)
            : source;
}