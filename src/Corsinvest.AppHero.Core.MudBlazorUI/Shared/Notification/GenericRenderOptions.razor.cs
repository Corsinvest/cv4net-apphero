﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Notification;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using MudBlazor.Extensions.Components.ObjectEdit;
using Corsinvest.AppHero.Core.MudBlazorUI.Helpers;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Notification;

public partial class GenericRenderOptions<TOptions, T>
    where TOptions : NotificationChannelsOptions<T>, new()
    where T : NotificationChannelOptions, new()
{
    private string DefaultGroupName { get; set; } = default!;

    protected override void OnInitialized()
    {
        //if (Options.GetType().GetProperties().Any(a => !a.PropertyType.IsAssignableTo(typeof(IEnumerable))
        //                                                && Type.GetTypeCode(a.PropertyType) == TypeCode.Object))
        //{
        DefaultGroupName = "General";
        //}
    }

    private void Configure(ObjectEditMeta<T> meta)
    {
        foreach (var item in meta.AllProperties)
        {
            if (item.PropertyName == "Name" || item.PropertyName == "Enabled")
            {
                item.Ignore(true);
            }
            else
            {
                MudExObjectEditFormHelper.FixPropertyItem(item);
            }            
        }
    }
}