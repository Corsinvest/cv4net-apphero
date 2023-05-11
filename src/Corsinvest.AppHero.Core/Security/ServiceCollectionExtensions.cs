/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using static Corsinvest.AppHero.Core.Security.Module.Permissions;

namespace Corsinvest.AppHero.Core.Security;

public static class ServiceCollectionExtensions
{
    public static async Task AppHeroPopulateSecurityAsync(this IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ServiceCollectionExtensions));
        logger.LogInformation("Initialize Security db");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var modularityService = services.GetRequiredService<IModularityService>();

        var adminRole = await roleManager.CreateRoleAsync(RoleConstants.AdministratorRole, "Admin Group");
        _ = await roleManager.CreateRoleAsync(RoleConstants.BasicRole, "Basic Group");

        var userName = "admin@local";
        var adminUser = await userManager.FindByNameAsync(userName);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = userName,
                IsActive = true,
                FirstName = "John",
                LastName = "Doe",
                Email = userName,
                EmailConfirmed = true,
                ProfileImageUrl = ApplicationHelper.GetGravatar(userName)
            };

            //default password
            await userManager.CreateAsync(adminUser, "Password123!");
        }

        await userManager.AddRolesToUserAsync(adminUser, new List<string>() { adminRole.Name! });

        //create roles for module
        foreach (var module in modularityService.Modules.Where(a => a.GetAllRoles().Any()))
        {
            var moduleAdminRole = await roleManager.CreateRoleAsync(module.RoleAdminKey, module.RoleAdminDescription);

            //admin role module
            await roleManager.AddPermissionsAsync(moduleAdminRole, module.GetPermissions().Select(a => a.Key));

            //admin application
            await userManager.AddRolesToUserAsync(adminUser, new[] { module.RoleAdminKey });

            //role and permission specific module
            foreach (var item in module.GetAllRoles().Where(a => !string.IsNullOrWhiteSpace(a.Key)))
            {
                //create role and permission
                await roleManager.AddPermissionsAsync(await roleManager.CreateRoleAsync(item.Key, item.Description),
                                                      item.Permissions.Select(a => a.Key));
            }
        }
    }
}