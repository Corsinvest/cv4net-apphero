/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;

namespace Corsinvest.AppHero.Core.Service;

public interface IBrowserService : IScopedDependency
{
    Task CopyToClipboard(string text);
    Task Open(string url, string target);
}
