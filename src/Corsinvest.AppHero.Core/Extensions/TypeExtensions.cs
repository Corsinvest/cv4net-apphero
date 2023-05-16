/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Reflection;

namespace Corsinvest.AppHero.Core.Extensions;

public static class TypeExtensions
{
    public static PropertyInfo? GetPropertyFromPath(this Type type, string path)
    {
        var typeInfo = type;
        PropertyInfo? pi = null;
        foreach (var item in path.Split('.'))
        {
            pi = typeInfo.GetPublicProperty(item)!;
            if (pi == null) { break; }
            typeInfo = pi.PropertyType;
        }

        return pi;
    }

    private static PropertyInfo? GetPublicProperty(this Type type, string propertyName)
    {
        if (type.IsInterface)
        {
            var pi = type.GetProperty(propertyName);
            if (pi == null)
            {
                foreach (var item in type.GetInterfaces())
                {
                    pi = item.GetPublicProperty(propertyName);
                    if (pi != null) { break; }
                }
            }

            return pi;
        }

        return type.GetProperty(propertyName);
    }
}