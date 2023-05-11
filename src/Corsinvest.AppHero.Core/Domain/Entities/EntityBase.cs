/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Domain.Entities;

public abstract class EntityBase<T> : EntityBase, IAggregateRoot<T>
{
    [Key]
    public T Id { get; set; } = default!;
}

public abstract class EntityBase : IAggregateRoot //, IHasDomainEvents
{
    //private readonly HashSet<DomainEvent> _domainEvents = new();

    //[NotMapped]
    //public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.ToList().AsReadOnly();

    //public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    //public void RemoveDomainEvent(DomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    //public void ClearDomainEvents() => _domainEvents.Clear();
}
