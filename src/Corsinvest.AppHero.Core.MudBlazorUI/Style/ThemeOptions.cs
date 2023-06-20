/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using System.ComponentModel;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Style;

public class ThemeOptions
{
    public string PrimaryColor
    {
        get => Current.Palette.Primary.Value;
        set
        {
            Current.Palette.Primary = value;
            Current.PaletteDark.Primary = value;
        }
    }

    [DisplayName("Border Radius")]
    public int DefaultBorderRadius
    {
        get => int.TryParse(Current.LayoutProperties.DefaultBorderRadius[0..^2], out var value)
                    ? value
                    : 0;
        set => Current.LayoutProperties.DefaultBorderRadius = value + "px";
    }

    [DisplayName("Right To Left")]
    public bool IsRightToLeft { get; set; }

    public DarkLightMode DarkLightMode { get; set; }

    private MudTheme? _current;
    [JsonIgnore]
    public MudTheme Current
    {
        get
        {
            if (_current == null)
            {
                _current = new MudTheme();
                _current.Palette.Primary = Colors.Blue.Default;
                _current.PaletteDark.Primary = Colors.Blue.Default;
            }

            return _current;
        }
    }

    public TableOptions Table { get; set; } = new()!;

    public void Reset()
    {
        _current = null;
        DarkLightMode = DarkLightMode.System;
    }
}