/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Auditing.Domains.Entities;
using Corsinvest.AppHero.Auditing.Persistence.Configurations;
using Corsinvest.AppHero.Auditing.Persistence.Interceptors;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Persistence.Context;
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Auditing.Persistence.Context;

public abstract class BaseApplicationDbContext<TUser, TRole> : BaseIdentityDbContext<TUser, TRole>
    where TUser : ApplicationUser
    where TRole : ApplicationRole
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public BaseApplicationDbContext(DbContextOptions options, AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<AuditTrail> AuditTrails { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        modelBuilder.ApplyConfiguration(new AuditTrailConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
}


public abstract class BaseAuditDbContext<TUser, TRole> : BaseIdentityDbContext<TUser, TRole>
    where TUser : ApplicationUser
    where TRole : ApplicationRole
{
    public BaseAuditDbContext(DbContextOptions options) : base(options) { }

    public DbSet<AuditTrail> AuditTrails { get; set; } = default!;
}