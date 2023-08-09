/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Core.Cli;

public interface ICliCommand
{
    string Name { get; }
    string Description { get; }
    Task<bool> ExecuteAsync(IHost host, string[] args);

    public static IEnumerable<ICliCommand> GetCommands()
        => AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(a => !a.IsInterface && !a.IsAbstract && typeof(ICliCommand).IsAssignableFrom(a))
                    .Select(a => (ICliCommand)Activator.CreateInstance(a)!)
                    .ToList();
}