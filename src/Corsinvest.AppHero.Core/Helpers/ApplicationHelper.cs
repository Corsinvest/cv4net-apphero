/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Cli;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Corsinvest.AppHero.Core.Helpers;

public static class ApplicationHelper
{
    static ApplicationHelper()
    {
        Directory.CreateDirectory(PathData);
        Directory.CreateDirectory(PathModules);
    }

    public static Type RootComponent { get; set; } = default!;
    public static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

    public static string HelpUrl { get; } = "/doc/index.html";
    public static string ModuleComponentUrl { get; } = "/Module";

    public static string PartPath(string path)
        => path.StartsWith(PathExecution, StringComparison.InvariantCultureIgnoreCase)
            ? Path.Combine(".", path[PathExecution.Length..])
            : path;

    public static string PathExecution => Directory.GetCurrentDirectory();
    public static string PathData => Path.Combine(PathExecution, "data");
    public static string FileNameOptions { get; } = "appsettings.json";
    public static string PathModules => Path.Combine(PathData, "modules");

    public static string ProductVersion
        => System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).ProductVersion!;

    public static string GetGravatar(string email, string forceDefault = "retro")
    {
        var hash = MD5.HashData(Encoding.ASCII.GetBytes(email))
                      .Select(a => a.ToString("X2"))
                      .JoinAsString("")
                      .ToLower();
        return $"https://www.gravatar.com/avatar/{hash}?d={forceDefault}";
    }

    public static async Task<bool> ExecuteCommandAsync(IHost host, string[] args)
    {
        var ret = false;
        foreach (var item in ICliCommand.GetCommands())
        {
            if (args.Contains($"--{item.Name}")) { ret = await item.ExecuteAsync(host, args); }
        }

        return ret;
    }
}