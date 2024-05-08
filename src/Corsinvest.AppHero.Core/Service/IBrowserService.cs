/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;

namespace Corsinvest.AppHero.Core.Service;

public interface IBrowserService : IScopedDependency
{
    Task CopyToClipboardAsync(string text);
    Task OpenAsync(string url, string target);
}
