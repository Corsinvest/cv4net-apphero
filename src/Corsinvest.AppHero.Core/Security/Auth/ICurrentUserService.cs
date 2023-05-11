/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;
using Corsinvest.AppHero.Core.Security.Identity;
using System.Security.Claims;

namespace Corsinvest.AppHero.Core.Security.Auth;

public interface ICurrentUserService : IScopedDependency
{
    bool IsAuthenticated { get; }
    Task<ApplicationUser?> GetUserAsync();
    ClaimsPrincipal? ClaimsPrincipal { get; }
    string UserId { get; }
    string UserName { get; }
    string IpAddress { get; }
    string HttpConnectionId { get; }
}