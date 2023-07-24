/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;

namespace Corsinvest.AppHero.Core.Security.Auth;

public class SecurityReadWrite
{
    public ApplicationRole Role { get; set; } = default!;

    public SecurityPermissionState Read { get; set; }
    public SecurityPermissionState Write { get; set; }

    public static bool CanPermissionRead(IEnumerable<SecurityReadWrite> security, ApplicationUser user)
        => CanPermission(security, user, true);

    public static bool CanPermissionWrite(IEnumerable<SecurityReadWrite> security, ApplicationUser user)
        => CanPermission(security, user, false);

    private static bool CanPermission(IEnumerable<SecurityReadWrite> security, ApplicationUser user, bool forRead)
    {
        var users = security.Where(a => a.Read == SecurityPermissionState.Allow, forRead)
                            .Where(a => a.Write == SecurityPermissionState.Allow, !forRead)
                            .SelectMany(a => a.Role.UserRoles)
                            .Select(a => a.User)
                            .ToList();

        foreach (var item in security.Where(a => a.Read == SecurityPermissionState.Denay, forRead)
                                     .Where(a => a.Write == SecurityPermissionState.Denay, !forRead)
                                     .SelectMany(a => a.Role.UserRoles)
                                     .Select(a => a.User))
        {
            users.Remove(item);
        }

        return users.Contains(user);
    }
}
