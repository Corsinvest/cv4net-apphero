/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Core.Domain.Repository;

public class BaseDbRepository<TContext, TEntity> : RepositoryBase<TEntity>, IReadRepository<TEntity>, IRepository<TEntity>
    where TEntity : class //, IAggregateRoot
{
    public BaseDbRepository(TContext dbContext) : base((dbContext as DbContext)!) { }

    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false).ProjectToType<TResult>();
}