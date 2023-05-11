/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using Nextended.Core.DeepClone;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Style;

public class ThemeOptions
{
    static ThemeOptions()
    {
        var theme = new MudTheme();
        //theme.Palette.AppbarBackground = "rgba(255,255,255,0.7)";
        theme.Palette.Primary = Colors.Blue.Default;


        theme.PaletteDark.Primary = Colors.Blue.Default;
        //theme.PaletteDark.AppbarBackground = "rgba(21,27,34,0.7)";
        //theme.PaletteDark.Black = "#27272f";
        //theme.PaletteDark.Background = "rgb(21,27,34)";
        //theme.PaletteDark.BackgroundGrey = "#27272f";
        //theme.PaletteDark.Surface = "#212B36";
        //theme.PaletteDark.DrawerBackground = "rgb(21,27,34)";
        //theme.PaletteDark.DrawerText = "rgba(255,255,255, 0.50)";
        //theme.PaletteDark.DrawerIcon = "rgba(255,255,255, 0.50)";
        //theme.PaletteDark.AppbarText = "rgba(255,255,255, 0.70)";
        //theme.PaletteDark.TextPrimary = "rgba(255,255,255, 0.70)";
        //theme.PaletteDark.TextSecondary = "rgba(255,255,255, 0.50)";
        //theme.PaletteDark.ActionDefault = "#adadb1";
        //theme.PaletteDark.ActionDisabled = "rgba(255,255,255, 0.26)";
        //theme.PaletteDark.ActionDisabledBackground = "rgba(255,255,255, 0.12)";
        //theme.PaletteDark.Divider = "rgba(255,255,255, 0.12)";
        //theme.PaletteDark.DividerLight = "rgba(255,255,255, 0.06)";
        //theme.PaletteDark.TableLines = "rgba(255,255,255, 0.12)";
        //theme.PaletteDark.LinesDefault = "rgba(255,255,255, 0.12)";
        //theme.PaletteDark.LinesInputs = "rgba(255,255,255, 0.3)";
        //theme.PaletteDark.TextDisabled = "rgba(255,255,255, 0.2)";

        DefaultTheme = theme;
    }

    public string PrimaryColor
    {
        get => Current.Palette.Primary.Value;
        set
        {
            Current.Palette.Primary = value;
            Current.PaletteDark.Primary = value;
        }
    }

    public int DefaultBorderRadius
    {
        get => int.TryParse(Current.LayoutProperties.DefaultBorderRadius[0..^2], out var value)
                    ? value
                    : 0;
        set => Current.LayoutProperties.DefaultBorderRadius = value + "px";
    }

    public bool DarkMode { get; set; }
    [JsonIgnore] public static MudTheme DefaultTheme { get; }

    private MudTheme? _current;
    [JsonIgnore]
    public MudTheme Current => _current ??= DefaultTheme.CloneDeep(Nextended.Core.DeepClone.FieldType.Both);

    public TableOptions Table { get; set; } = new()!;

    public void Reset()
    {
        _current = null;
        DarkMode = false;
    }
}