/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Identity;

namespace Corsinvest.AppHero.Core.Security.Identity;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; } = default!;
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
    public virtual ICollection<ApplicationRoleClaim> Claims { get; set; } = default!;
}