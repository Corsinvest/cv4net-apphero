/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Security.Auth.Permissions;

public class Role
{
    public Role(string key, string description, IEnumerable<Permission> permissions)
    {
        Key = key;
        Description = description;
        Permissions = permissions;
    }

    public string Key { get; }
    public string Description { get; }
    public IEnumerable<Permission> Permissions { get; }
}
