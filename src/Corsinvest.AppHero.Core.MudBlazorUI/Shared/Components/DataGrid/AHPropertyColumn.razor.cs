/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

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

        if (!string.IsNullOrWhiteSpace(PropertyName))
        {
            var L = LocalizerFactory.Create(GetType());
            Title = L[MudBlazorHelper.GetDescriptionProperty<T>(PropertyName)];

            if (IsFormattedValue) { CellTemplate = RenderFormattedValue; }
        }
    }

    private TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
        => string.IsNullOrWhiteSpace(PropertyName)
            ? null
            : typeof(T).GetProperty(PropertyName)!.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().FirstOrDefault();

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


//public partial class AHDataGridColumn<T> : Column<T>
//{
//    [Parameter] public Type FormatProvider { get; set; } = default!;
//    [Parameter] public string DataFormatString { get; set; } = default!;

//    protected override void OnInitialized()
//    {
//        base.OnInitialized();

//        if (!string.IsNullOrWhiteSpace(PropertyName))
//        {
//            Title = MudBlazorHelper.GetDescriptionProperty<T>(PropertyName);

//            if (IsFormattedValue) { CellTemplate = RenderFormattedValue; }

//            if (Type.GetTypeCode(base.PropertyType) == TypeCode.Boolean)
//            {
//                CellTemplate = RenderBool;
//            }
//        }
//    }

//    private TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
//        => string.IsNullOrWhiteSpace(PropertyName)
//            ? null
//            : typeof(T).GetProperty(PropertyName)!.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().FirstOrDefault();

//    private bool IsFormattedValue
//        => !string.IsNullOrWhiteSpace(DataFormatString)
//            || !string.IsNullOrWhiteSpace(GetAttribute<DisplayFormatAttribute>()?.DataFormatString);

//    private string GetFormattedString(object item)
//    {
//        var dataFormatString = DataFormatString;
//        var format = GetAttribute<DisplayFormatAttribute>();
//        if (string.IsNullOrWhiteSpace(DataFormatString)) { dataFormatString = format?.DataFormatString; }

//        if (item == null && !string.IsNullOrWhiteSpace(format?.NullDisplayText))
//        {
//            return format.NullDisplayText;
//        }
//        else if (string.IsNullOrWhiteSpace(dataFormatString))
//        {
//            return item + "";
//        }
//        else if (FormatProvider != null)
//        {
//            return string.Format((IFormatProvider)Activator.CreateInstance(FormatProvider)!, dataFormatString, item);
//        }
//        else
//        {
//            return string.Format(dataFormatString, item);
//        }
//    }

//    protected override object CellContent(T item)
//    {
//        return null;
//    }

//    protected override object PropertyFunc(T item)
//    {
//        return null;
//    }

//    protected override void SetProperty(object item, object value) { }
//}