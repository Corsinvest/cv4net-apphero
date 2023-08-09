/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using ConsoleTables;
using Microsoft.Extensions.Hosting;

namespace Corsinvest.AppHero.Core.Cli;

public class Help : ICliCommand
{
    public string Name { get; } = "help";
    public string Description { get; } = "Show help";

    public async Task<bool> ExecuteAsync(IHost host, string[] args)
    {
        Console.Out.WriteLine("Show commands");

        var table = new ConsoleTable("Command", "Description");
        foreach (var item in ICliCommand.GetCommands())
        {
            table.AddRow($"--{item.Name}", item.Description);
        }
        table.Write(Format.Minimal);

        return await Task.FromResult(true);
    }
}
