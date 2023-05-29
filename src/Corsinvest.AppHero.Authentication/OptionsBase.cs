/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace Corsinvest.AppHero.Authentication;

public class OptionsBase
{
    public string DefaultRolesNewUser { get; set; } = default!;
    public bool AutoImportUser { get; set; } = true;

    public void MapCustomJson(ClaimActionCollection collection)
    {
        collection.MapCustomJson(nameof(DefaultRolesNewUser), (a) => { return DefaultRolesNewUser; });
        collection.MapCustomJson(nameof(AutoImportUser), (a) => { return AutoImportUser.ToString(); });
    }
}