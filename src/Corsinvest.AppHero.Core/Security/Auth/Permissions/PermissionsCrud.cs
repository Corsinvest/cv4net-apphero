/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Constants;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Security.Auth.Permissions;

public class PermissionsCrud : PermissionsRead
{
    public PermissionsCrud(string prefix) : base(prefix)
    {
        Create = new($"{Prefix}.{ActionConstants.Create}", "Create", UIIcon.Create.GetName());
        Edit = new($"{Prefix}.{ActionConstants.Edit}", "Edit", UIIcon.Edit.GetName());
        Delete = new($"{Prefix}.{ActionConstants.Delete}", "Delete", UIIcon.Delete.GetName(), UIColor.Error);
    }

    public static PermissionsCrud Fake() => new("Fake");
    public Permission Create { get; }
    public Permission Edit { get; }
    public Permission Delete { get; }
    public override IEnumerable<Permission> Permissions => new[] { Create, Edit, Delete }.Union(base.Permissions);
}