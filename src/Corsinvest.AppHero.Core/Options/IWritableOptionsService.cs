/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Options;

public interface IWritableOptionsService<T> : IOptions<T> where T : class, new()
{
    void Update(Action<T> applyChanges);

    void Update(T applyChanges);
}
