/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.MudBlazorUI.Helpers;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using System.Collections;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Options;

public partial class GenericRenderOptions<T> where T : class, new()
{
    private string DefaultGroupName { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (Options.GetType().GetProperties().Any(a => !a.PropertyType.IsAssignableTo(typeof(IEnumerable))
                                                        && Type.GetTypeCode(a.PropertyType) == TypeCode.Object))
        {
            DefaultGroupName = "General";
        }
    }

    private void Configure(ObjectEditMeta<T> meta)
    {
        foreach (var item in meta.AllProperties) { MudExObjectEditFormHelper.FixPropertyItem(item); }
    }
}