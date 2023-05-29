/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Humanizer;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using MudBlazor.Extensions.Options;
using MudExtensions;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Helpers;

public static class MudExObjectEditFormHelper
{
    public static void FixPropertyItem(ObjectEditPropertyMeta item)
    {
        if (!string.IsNullOrEmpty(item.GroupInfo.Name)) { item.GroupInfo.Name = item.GroupInfo.Name.Humanize(LetterCasing.Title); }

        item.WithLabelResolver(pi => ClassHelper.GetDescriptionProperty(pi.DeclaringType!, pi.Name));

        item.WithSettings(a =>
        {
            //a.Localizer = stringLocalizer;
            a.LabelResolverFn = (pi) => ClassHelper.GetDescriptionProperty(pi.DeclaringType!, pi.Name);
        });

        //hide browsable
        item.Ignore(!(item.PropertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), true)
                                       .Cast<BrowsableAttribute>()
                                       .FirstOrDefault()?.Browsable ?? true));

        if (item.Value is ICollection)
        {
            //fix dialog
            item.WithAdditionalAttribute(nameof(MudExCollectionEditor<object>.DialogOptions), new DialogOptionsEx()
            {
                MaximizeButton = true,
                CloseButton = true,
                FullHeight = true,
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = new[] { AnimationType.SlideIn },
                Position = DialogPosition.Center,
                DisableSizeMarginY = true,
                DisablePositionMargin = true
            });

            //foreach (var item1 in item.MainEditMeta.Properties())
            //{
            //    FixPropertyItem(item1, stringLocalizer);
            //}

        }

        //if (Type.GetTypeCode(item.PropertyInfo.PropertyType) == TypeCode.Boolean)
        //{
        //    item.RenderWith<MudSwitch<bool>, bool>(a => a.Checked, options => { options.Color = Color.Primary; });
        //}

        var dataType = item.PropertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), true)
                                        .Cast<DataTypeAttribute>()
                                        .FirstOrDefault();
        if (dataType != null)
        {
            switch (dataType.DataType)
            {
                //case DataType.Custom:
                //    var customDataType = (dataType.CustomDataType + "").ToLower();
                //    if (customDataType == "cron")                    {                    }
                //    break;

                //case DataType.DateTime: break;
                //case DataType.Date: break;
                //case DataType.Time: break;
                //case DataType.Duration: break;
                //case DataType.PhoneNumber: break;
                //case DataType.Currency: break;
                //case DataType.Text: break;
                //case DataType.Html: break;
                case DataType.MultilineText:
                    item.RenderWith<MudTextField<string>, string>(a => a.Value,
                                                                  options =>
                                                                  {
                                                                      options.Lines = (item.Value + "").SplitNewLine().Length > 15
                                                                                        ? 10
                                                                                        : 5;
                                                                  });
                    break;

                //case DataType.EmailAddress: break;
                case DataType.Password: item.RenderWith<MudPasswordField<string>, string>(a => a.Value); break;

                //case DataType.Url: break;
                //case DataType.ImageUrl: break;
                //case DataType.CreditCard: break;
                //case DataType.PostalCode: break;
                //case DataType.Upload: break;
                default: break;
            }
        }
    }
}