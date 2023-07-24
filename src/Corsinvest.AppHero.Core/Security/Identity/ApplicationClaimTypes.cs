/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Security.Identity;

public static class ApplicationClaimTypes
{
    public static string Permission { get; } = "Permission";
    public static string ProfileImageUrl { get; } = "ProfileImageUrl";
    public static string FullName { get; } = "FullName";
}