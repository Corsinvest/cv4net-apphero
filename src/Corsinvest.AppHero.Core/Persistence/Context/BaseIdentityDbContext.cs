/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Persistence.Configurations;
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Core.Persistence.Context;

public abstract class BaseIdentityDbContext<TUser, TRole> : IdentityDbContext<TUser,
                                                                              TRole,
                                                                              string,
                                                                              ApplicationUserClaim,
                                                                              ApplicationUserRole,
                                                                              ApplicationUserLogin,
                                                                              ApplicationRoleClaim,
                                                                              ApplicationUserToken>
                                                            where TUser : ApplicationUser
                                                            where TRole : ApplicationRole
{
    protected BaseIdentityDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationRoleClaimConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserClaimConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserLoginConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserTokenConfiguration());
    }
}