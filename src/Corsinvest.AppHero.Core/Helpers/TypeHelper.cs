/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Helpers;

public static class TypeHelper
{
    public static Type GetType(string @class, Type @default)
    {
        var ret = @default;
        if (!string.IsNullOrEmpty(@class))
        {
            try
            {
                ret = Type.GetType(@class)!;
            }
            catch //(Exception ex)
            {
                //_logger.Warning(ex, ex.Message);
            }
        }
        return ret ?? @default;
    }

    public static string GetClassAndAssemblyName<T>() => $"{typeof(T).FullName}, {typeof(T).Assembly.GetName().Name}";
}