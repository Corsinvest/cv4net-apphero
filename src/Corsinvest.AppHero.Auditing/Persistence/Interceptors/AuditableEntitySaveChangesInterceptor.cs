/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Auditing.Domains.Contracts;
using Corsinvest.AppHero.Auditing.Domains.Entities;
using Corsinvest.AppHero.Core.Security.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Corsinvest.AppHero.Auditing.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private List<AuditTrail> _temporaryAuditTrailList = [];

    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
                                                                                InterceptionResult<int> result,
                                                                                CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        _temporaryAuditTrailList = TryInsertTemporaryAuditTrail(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData,
                                                           int result,
                                                           CancellationToken cancellationToken = default)
    {
        var resultvalueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await TryUpdateTemporaryPropertiesForAuditTrail(eventData.Context, cancellationToken);
        return resultvalueTask;
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;
        var userName = _currentUserService.UserName;
        var now = DateTime.Now;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDelete>().Where(a => a.State == EntityState.Deleted))
        {
            entry.Entity.DeletedBy = userName;
            entry.Entity.DeletedOn = now;
            entry.State = EntityState.Modified;
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userName;
                    entry.Entity.CreatedOn = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userName;
                    entry.Entity.LastModifiedOn = now;
                    break;

                case EntityState.Deleted: break;

                case EntityState.Unchanged:
                    if (entry.References.Any(a => a.TargetEntry != null
                                                    && a.TargetEntry.Metadata.IsOwned()
                                                    && (a.TargetEntry.State == EntityState.Added
                                                        || a.TargetEntry.State == EntityState.Modified)))
                    {
                        entry.Entity.LastModifiedBy = userName;
                        entry.Entity.LastModifiedOn = now;
                    }
                    break;
            }
        }
    }

    private List<AuditTrail> TryInsertTemporaryAuditTrail(DbContext? context)
    {
        if (context is null) return [];
        var userId = _currentUserService.UserName;
        context.ChangeTracker.DetectChanges();

        var temporaryAuditEntries = new List<AuditTrail>();

        foreach (var entry in context.ChangeTracker.Entries<IAuditingEnabled>())
        {
            if (entry.Entity is AuditTrail
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged) { continue; }

            var auditEntry = new AuditTrail()
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId,
                DateTime = DateTime.Now,
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey() && property.CurrentValue is not null)
                {
                    auditEntry.PrimaryKey[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        if (property.CurrentValue is not null) { auditEntry.NewValues[propertyName] = property.CurrentValue; }
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        if (property.OriginalValue is not null) { auditEntry.OldValues[propertyName] = property.OriginalValue; }
                        break;

                    case EntityState.Modified when property.IsModified
                                                    && (property.OriginalValue is null
                                                        && property.CurrentValue is not null
                                                        || property.OriginalValue is not null
                                                        && !property.OriginalValue.Equals(property.CurrentValue)):
                        auditEntry.AffectedColumns.Add(propertyName);
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        if (property.CurrentValue is not null) { auditEntry.NewValues[propertyName] = property.CurrentValue; }
                        break;
                }
            }
            temporaryAuditEntries.Add(auditEntry);
        }
        return temporaryAuditEntries;
    }

    private async Task TryUpdateTemporaryPropertiesForAuditTrail(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) { return; }

        if (_temporaryAuditTrailList.Count != 0)
        {
            foreach (var auditEntry in _temporaryAuditTrailList)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is not null)
                    {
                        auditEntry.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditEntry.NewValues is not null && prop.CurrentValue is not null)
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                context.Add(auditEntry);
            }
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}