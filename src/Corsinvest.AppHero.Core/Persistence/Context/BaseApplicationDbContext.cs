/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Core.Persistence.Context;

public abstract class BaseApplicationDbContext<TUser, TRole> : BaseIdentityDbContext<TUser, TRole>
    where TUser : ApplicationUser
    where TRole : ApplicationRole
{
    public BaseApplicationDbContext(DbContextOptions options) : base(options) { }
}
