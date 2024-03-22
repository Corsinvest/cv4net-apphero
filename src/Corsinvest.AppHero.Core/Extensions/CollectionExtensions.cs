/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Extensions;

public static class CollectionExtensions
{
    public static int IndexOf<T>(this T[] array, T obj) => Array.IndexOf(array, obj);


    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) => source.Select((item, index) => (item, index));

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
    }

    public static void AddIf<T>(this List<T> list, bool condition, T item)
    {
        if (condition) { list.Add(item); }
    }

    public static void RemoveIf<T>(this List<T> list, bool condition, T item)
    {
        if (condition) { list.Remove(item); }
    }

    public static IEnumerable<T>? ForEach<T>(this IEnumerable<T>? source, Action<T> action)
    {
        if (source is not null)
        {
            foreach (var item in source) { action(item); }
        }

        return source;
    }

    public static async Task<IEnumerable<T>?> ForEachAsync<T>(this IEnumerable<T>? source,
                                                              Func<T, Task> action,
                                                              CancellationToken cancellationToken = default)
    {
        if (source is not null)
        {
            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await action.Invoke(item).ConfigureAwait(false);
            }
        }

        return source;
    }
}