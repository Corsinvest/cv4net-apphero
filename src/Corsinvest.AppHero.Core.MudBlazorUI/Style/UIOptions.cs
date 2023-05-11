/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Style;

public class UIOptions
{
    public string ClassNotFoundComponent { get; set; } = default!;
    public string ClassNotAuthorizedComponent { get; set; } = default!;
    public string ClassIndexPageComponent { get; set; } = default!;
    public string LoginBackgroundImage { get; set; } = "./images/wallpaper.png";
    public ThemeOptions Theme { get; set; } = new()!;
}