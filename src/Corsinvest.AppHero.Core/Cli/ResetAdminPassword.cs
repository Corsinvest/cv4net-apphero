/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Core.Cli;

public class ResetAdminPassword : ICliCommand
{
    public string Name { get; } = "reset-admin-password";
    public string Description { get; } = "Reset admin password";

    public async Task<bool> ExecuteAsync(IHost host, string[] args)
    {
        var userManager = host.Services.GetService<UserManager<ApplicationUser>>()!;
        var user = await userManager.FindByNameAsync("admin@local")!;
        await userManager.ResetPasswordAsync(user, "Password123!");
        return true;
    }
}