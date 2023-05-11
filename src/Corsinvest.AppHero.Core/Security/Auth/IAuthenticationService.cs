/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;

namespace Corsinvest.AppHero.Core.Security.Auth;

public interface IAuthenticationService
{
    Task ExecuteLoginAsync(ApplicationUser user, bool rememberMe);
    Task<bool> LoginAsync(LoginRequest loginRequest);
    Task Logout();
}