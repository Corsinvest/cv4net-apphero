/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Linq.Expressions;
using System.Reflection;

namespace Corsinvest.AppHero.Core.Helpers;

public static class ClassHelper
{

    public static MemberInfo? GetPropertyInfo(Expression propertyExpression)
    {
        var memberExpr = propertyExpression as MemberExpression;
        if (memberExpr == null && propertyExpression is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert)
        {
            memberExpr = unaryExpr.Operand as MemberExpression;
        }

        if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property) { return memberExpr.Member; }
        return null;
    }

    public static void SetValue(object obj, string propertyName, object value)
        => obj.GetType().GetProperty(propertyName)!.SetValue(obj, value);

    public static TValue GetValue<TValue>(object obj, string propertyName)
    {
        var objNext = obj;
        foreach (var item in propertyName.Split("."))
        {
            objNext = objNext!.GetType().GetProperty(item)!.GetValue(objNext);
        }

        return (TValue)objNext;
    }
}