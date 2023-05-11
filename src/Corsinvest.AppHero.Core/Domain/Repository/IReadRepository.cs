/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Ardalis.Specification;

namespace Corsinvest.AppHero.Core.Domain.Repository;

public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class //, IAggregateRoot
{ }
