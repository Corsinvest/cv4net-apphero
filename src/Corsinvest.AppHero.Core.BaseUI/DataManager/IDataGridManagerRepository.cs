/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Repository;

namespace Corsinvest.AppHero.Core.BaseUI.DataManager;

public interface IDataGridManagerRepository<T> : IDataGridManager<T> where T : class //, IAggregateRoot
{
    IRepository<T> Repository { get; }
}