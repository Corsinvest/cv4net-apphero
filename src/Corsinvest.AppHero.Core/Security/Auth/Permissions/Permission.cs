/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Security.Auth.Permissions;

public class Permission
{
    public Permission(string key,
                      string description,
                      string icon = null!,
                      UIColor color = UIColor.Default,
                      bool inBasicRole = false)
    {
        Key = key;
        Description = description;
        Icon = icon;
        Color = color;
        InBasicRole= inBasicRole;
    }

    public string Key { get; }
    public string Description { get; }
    public string Icon { get; }
    public UIColor Color { get; }
    public bool InBasicRole { get; }
}