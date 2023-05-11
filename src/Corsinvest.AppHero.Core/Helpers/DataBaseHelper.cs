/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Helpers;

public static class DataBaseHelper
{
    public static string CreateSQLitePath(string fileName) => $"Data Source={fileName}";
}