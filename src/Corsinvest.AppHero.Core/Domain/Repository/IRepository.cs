/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Ardalis.Specification;
//using Corsinvest.AppHero.Core.Domain.Contracts;

namespace Corsinvest.AppHero.Core.Domain.Repository;

public interface IRepository<T> : IRepositoryBase<T>
    where T : class //, IAggregateRoot
{ }