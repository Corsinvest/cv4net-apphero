/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;

namespace Corsinvest.AppHero.Core.Service;

public interface IFileStorageService : ITransientDependency
{
    Task<string> UploadAsync(Stream strem, string fileName, string subFolder);
    void Remove(string filename, string subFolder);
}