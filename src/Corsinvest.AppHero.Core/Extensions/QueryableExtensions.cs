/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Linq.Expressions;

namespace Corsinvest.AppHero.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Where<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, bool condition)
        => condition
            ? query.Where(predicate)
            : query;

    public static IQueryable<T> Where<T>(this IQueryable<T> query, Expression<Func<T, int, bool>> predicate, bool condition)
        => condition
            ? query.Where(predicate)
            : query;
}