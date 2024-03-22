/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Core.Cli;

public class ResetUserPassword : ICliCommand
{
    public string Name { get; } = "reset-user-password";
    public string Description { get; } = "Reset user password";

    public async Task<bool> ExecuteAsync(IHost host, string[] args)
    {
        using var scope = host.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>()!;
        Console.Out.WriteLine("Username reset password:");
        var userName = Console.ReadLine();

        if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
        {
            Console.Out.WriteLine("Username not valid:");
        }
        else
        {
            var user = (await userManager.FindByNameAsync(userName));
            if (user == null)
            {
                Console.Out.WriteLine("Username not valid:");
            }
            else
            {
                await userManager.ResetPasswordAsync(user, UserConstants.PasswordDefault);
                Console.Out.WriteLine($"Reset user '{userName}' with password {UserConstants.PasswordDefault}");
            }
        }

        return true;
    }
}