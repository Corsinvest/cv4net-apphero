/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Domain.Entities;

public interface IAggregateRoot : IEntity { }

public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey> { }