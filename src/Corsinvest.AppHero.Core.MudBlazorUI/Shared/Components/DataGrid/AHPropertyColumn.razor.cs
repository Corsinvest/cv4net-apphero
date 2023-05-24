/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;

public partial class AHPropertyColumn<T, TProperty> : PropertyColumn<T, TProperty>
{
    [Parameter] public Type FormatProvider { get; set; } = default!;
    [Inject] private IStringLocalizerFactory LocalizerFactory { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Type.GetTypeCode(PropertyType) == TypeCode.Boolean && CellTemplate == null)
        {
            CellTemplate = RenderBool;
        }

        if (Property != null)
        {
            var L = LocalizerFactory.Create(GetType());
            Title = L[ClassHelper.GetDescriptionProperty(Property)];

            if (IsFormattedValue) { CellTemplate = RenderFormattedValue; }
        }
    }

    private TValue GetValue<TValue>(T item) => (TValue)((PropertyInfo)ClassHelper.GetPropertyInfo(Property.Body)!).GetValue(item)!;

    private TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
        => ClassHelper.GetPropertyInfo(Property.Body)?
                      .GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().FirstOrDefault();

    private bool IsFormattedValue
        => !string.IsNullOrWhiteSpace(Format)
            || !string.IsNullOrWhiteSpace(GetAttribute<DisplayFormatAttribute>()?.DataFormatString);

    private string GetFormattedString(object item)
    {
        var dataFormatString = Format;
        var format = GetAttribute<DisplayFormatAttribute>();
        if (string.IsNullOrWhiteSpace(Format)) { dataFormatString = format?.DataFormatString; }

        if (item == null && !string.IsNullOrWhiteSpace(format?.NullDisplayText))
        {
            return format.NullDisplayText;
        }
        else if (string.IsNullOrWhiteSpace(dataFormatString))
        {
            return item + "";
        }
        else if (FormatProvider != null)
        {
            return string.Format((IFormatProvider)Activator.CreateInstance(FormatProvider)!, dataFormatString, item);
        }
        else
        {
            return string.Format(dataFormatString, item);
        }
    }
}