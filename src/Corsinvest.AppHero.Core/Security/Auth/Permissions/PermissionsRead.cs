/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Constants;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Security.Auth.Permissions;

public class PermissionsRead
{
    public PermissionsRead(string prefix)
    {
        Prefix = prefix;
        Search = new($"{Prefix}.{ActionConstants.Search}", "Search", UIIcon.Search.GetName());
        Export = new($"{Prefix}.{ActionConstants.Export}", "Export", UIIcon.ExportExcel.GetName());
    }

    public string Prefix { get; }
    public Permission Search { get; }
    public Permission Export { get; }
    public virtual IEnumerable<Permission> Permissions => new[] { Search, Export };
}
