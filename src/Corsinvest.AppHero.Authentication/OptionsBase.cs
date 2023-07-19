/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Authentication;

public class OptionsBase
{
    [Display(Name = "Default Roles New User must be separated with a comma character (\",\")")]
    public string DefaultRolesNewUser { get; set; } = default!;
    public bool AutoImportUser { get; set; } = true;

    public void MapCustomJson(ClaimActionCollection collection)
    {
        collection.MapCustomJson(nameof(DefaultRolesNewUser), (a) => { return DefaultRolesNewUser; });
        collection.MapCustomJson(nameof(AutoImportUser), (a) => { return AutoImportUser.ToString(); });
    }
}