/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Authorization;

namespace Corsinvest.AppHero.Core.Security.Auth.Permissions;

public class PermissionService : IPermissionService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;

    public PermissionService(IAuthorizationService authorizationService, ICurrentUserService currentUserService)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyDictionary<string, bool>> HasPermissionsAsync(IEnumerable<Permission> permissions)
    {
        var user = _currentUserService.ClaimsPrincipal!;
        var ret = new Dictionary<string, bool>();
        foreach (var item in permissions)
        {
            ret.Add(item.Key, (await _authorizationService.AuthorizeAsync(user, item.Key)).Succeeded);
        }
        return ret;
    }

    public async Task<bool> HasPermissionAsync(Permission permission)
        => (await _authorizationService.AuthorizeAsync(_currentUserService.ClaimsPrincipal!, permission.Key)).Succeeded;
}