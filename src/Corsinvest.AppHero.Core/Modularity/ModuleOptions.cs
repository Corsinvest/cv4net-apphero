/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleOptions
{
    internal ModuleOptions() { }
    public Type Type { get; internal set; } = default!;
    public Type? Render { get; set; }
}