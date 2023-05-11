/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Auditing.Domains.Entities;
using Corsinvest.AppHero.Core.Persistence.Converter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Corsinvest.AppHero.Auditing.Persistence.Configurations;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.ToTable("AuditTrails");

        builder.Property(a => a.AuditType).HasConversion<string>();

        builder.Property(e => e.AffectedColumns)
           .HasConversion(
                 v => JsonConvert.SerializeObject(v),
                 v => JsonConvert.DeserializeObject<List<string>>(v),
                 new ValueComparer<ICollection<string>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                               c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                               c => c.ToList()));

        builder.Property(a => a.OldValues).HasConversion<JsonStringConverter<Dictionary<string, object>>>();
        builder.Property(a => a.NewValues).HasConversion<JsonStringConverter<Dictionary<string, object>>>();
        builder.Property(a => a.PrimaryKey).HasConversion<JsonStringConverter<Dictionary<string, object>>>();
    }
}