/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Corsinvest.AppHero.Core.Extensions;

public static class IdentityExtensions
{
    public static string ToStringErrors(this IdentityResult result)
        => result.Errors.Select(a => $"{a.Code} - {a.Description}").JoinAsString(Environment.NewLine);

    public static async Task<ApplicationRole> CreateRoleAsync(this RoleManager<ApplicationRole> roleManager, string name, string description)
    {
        var role = await roleManager.FindByNameAsync(name);
        if (role == null)
        {
            //create role
            role = new ApplicationRole
            {
                Name = name,
                Description = description
            };
            await roleManager.CreateAsync(role);
        }

        return role;
    }

    public static async Task AddPermissionsAsync(this RoleManager<ApplicationRole> roleManager, ApplicationRole role, IEnumerable<string> permissions)
    {
        //create permissions
        var permissionsIndDb = (await roleManager.GetClaimsAsync(role))
                                    .Where(a => a.Type == ApplicationClaimTypes.Permission)
                                    .Select(a => a.Value).ToArray();

        foreach (var item in permissions.Where(a => !permissionsIndDb.Contains(a)))
        {
            await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, item));
        }
    }

    public static async Task RemovePermissionsAsync(this RoleManager<ApplicationRole> roleManager, ApplicationRole role, IEnumerable<string> permissions)
    {
        //create permissions
        var permissionsIndDb = (await roleManager.GetClaimsAsync(role))
                                    .Where(a => a.Type == ApplicationClaimTypes.Permission)
                                    .ToArray();

        foreach (var item in permissionsIndDb.Where(a => permissions.Contains(a.Value)))
        {
            await roleManager.RemoveClaimAsync(role, item);
        }
    }

    public static async Task<IdentityResult> AddRolesToUserAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user, IEnumerable<string> roles)
    {
        var rolesIndDb = await userManager.GetRolesAsync(user);
        return await userManager.AddToRolesAsync(user, roles.Where(a => !rolesIndDb.Contains(a)));
    }

    public static async Task<IdentityResult> ResetPasswordAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user, string password)
        => await userManager.ResetPasswordAsync(user, await userManager.GeneratePasswordResetTokenAsync(user), password);

    public static async Task<IdentityResult> RemoveRolesToUserAsync(this UserManager<ApplicationUser> userManager, ApplicationUser user, IEnumerable<string> roles)
    {
        var rolesIndDb = await userManager.GetRolesAsync(user);
        return await userManager.RemoveFromRolesAsync(user, roles.Where(a => rolesIndDb.Contains(a)));
    }
}