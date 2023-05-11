/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHBoolIcon
{
    [Parameter] public bool Value { get; set; }
    [Parameter] public MudBlazor.Size Size { get; set; } = MudBlazor.Size.Small;
    [Parameter] public AHBoolIconType IconType { get; set; } = AHBoolIconType.CheckBox;
    [Parameter] public string? IconTrue { get; set; }
    [Parameter] public string? IconFalse { get; set; }
    [Parameter] public MudBlazor.Color ColorTrue { get; set; } = MudBlazor.Color.Default;
    [Parameter] public MudBlazor.Color ColorFalse { get; set; } = MudBlazor.Color.Default;

    private string? GetIcon()
        => IconType switch
        {
            AHBoolIconType.Thumb => Value ? Icons.Material.Outlined.ThumbUp : Icons.Material.Outlined.ThumbDown,
            AHBoolIconType.CheckBox => Value ? Icons.Material.Outlined.CheckBox : @Icons.Material.Outlined.CheckBoxOutlineBlank,
            AHBoolIconType.RadioButton => Value ? Icons.Material.Outlined.RadioButtonChecked : @Icons.Material.Outlined.RadioButtonUnchecked,
            AHBoolIconType.Toggle => Value ? Icons.Material.Outlined.ToggleOn : @Icons.Material.Outlined.ToggleOff,
            AHBoolIconType.Custom => Value ? IconTrue : IconFalse,
            _ => "",
        };
}