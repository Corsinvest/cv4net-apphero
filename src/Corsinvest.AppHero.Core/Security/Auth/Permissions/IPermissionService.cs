/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Security.Auth.Permissions;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Permission permission);
    Task<IReadOnlyDictionary<string, bool>> HasPermissionsAsync(IEnumerable<Permission> permissions);
}