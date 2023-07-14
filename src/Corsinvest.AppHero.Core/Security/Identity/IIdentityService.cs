/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Corsinvest.AppHero.Core.Security.Identity;

public interface IIdentityService : ITransientDependency
{
    Task SendEmailResetPasswordAsync(string email);
    Task SendEmailConfirmRegistrationAsync(ApplicationUser user);
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles);

    Task<string> GenerateJwtAsync(ApplicationUser user);
    Task<ClaimsPrincipal> GetClaimsPrincipal(string token);
}
