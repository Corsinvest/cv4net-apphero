/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Humanizer;
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

    public static string GetDescriptionProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var propertyInfo = ClassHelper.GetPropertyInfo(propertyExpression.Body);
        return propertyInfo == null
                ? null!
                : GetDescriptionProperty(propertyInfo.DeclaringType!, propertyInfo.Name);
    }

    //    public static string GetDescriptionProperty<T>(Expression<Func<T, object>> propertyExpression) => GetDescriptionProperty<T, object>(propertyExpression);

    public static string GetDescriptionProperty<T>(string propertyName) => GetDescriptionProperty(typeof(T), propertyName);

    public static string GetDescriptionProperty(Type type, string propertyName)
    {
        var label = string.Empty;
        var propertyInfo = type.GetProperty(propertyName);
        if (propertyInfo != null)
        {
            if (label == string.Empty)
            {
                label = propertyInfo?.GetCustomAttributes(typeof(DisplayAttribute), true)
                                     .Cast<DisplayAttribute>()
                                     .FirstOrDefault()?.Name ?? string.Empty;

                if (label == string.Empty)
                {
                    label = propertyInfo?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                         .Cast<DisplayNameAttribute>()
                                         .FirstOrDefault()?.DisplayName ?? string.Empty;
                }

                if (label == string.Empty) { label = propertyName.Humanize(LetterCasing.Title); }
            }
        }

        return label;
    }

}