/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Domain.Entities;

public interface IEntity
{
    //IReadOnlyCollection<DomainEvent> DomainEvents { get; }
}

public interface IEntity<TKey> : IEntity
{
    TKey Id { get; }
}