/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;

namespace Corsinvest.AppHero.Core.Service;

public interface IExcelWriter : ITransientDependency
{
    Stream WriteToStream<T>(IList<T> data);
}