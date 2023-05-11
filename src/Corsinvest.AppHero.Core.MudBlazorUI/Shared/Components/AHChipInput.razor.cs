/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Linq.Expressions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHChipInput<T> where T : class
{
    [Parameter] public string Label { get; set; } = default!;
    [Parameter][Category("Validation")] public Expression<Func<T>>? For { get; set; } = default!;
    [Parameter][Category("Behavior")] public bool CoerceText { get; set; } = true;
    [Parameter][Category("Behavior")] public bool CoerceValue { get; set; }
    [Parameter] public int MaxItems { get; set; }
    [Parameter] public List<T> SelectedItems { get; set; } = default!;
    [Parameter] public Func<string, Task<IEnumerable<T>>> SearchFunc { get; set; } = async (search) => await Task.FromResult(Enumerable.Empty<T>());
    [Parameter] public Func<T, string> DisplayValueFunc { get; set; } = _ => string.Empty;
    [Parameter] public EventCallback ValueChanged { get; set; }

    private MudAutocomplete<T>? RefAutocomplete { get; set; }

    private void Closed(MudChip chip)
    {
        SelectedItems.Remove((T)chip.Value);
        ValueChanged.InvokeAsync();
    }

    private void OnAutocompleteValueChanged(T? obj)
    {
        RefAutocomplete?.Clear();

        if (obj != null)
        {
            var displayValue = DisplayValueFunc(obj);
            if (!(string.IsNullOrWhiteSpace(displayValue)
                || SelectedItems.Any(a => DisplayValueFunc(a) == displayValue)))
            {
                SelectedItems.Add(obj);
                ValueChanged.InvokeAsync();
            }
        }
    }
}