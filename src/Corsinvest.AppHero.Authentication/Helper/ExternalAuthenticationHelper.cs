/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth;

namespace Corsinvest.AppHero.Authentication.Helper;

public static class ExternalAuthenticationHelper
{
    public static string MakeUrlError(ExternalAuthError error, string? description = null)
        => string.IsNullOrEmpty(description)
            ? $"/Auth/ExternalAuthError/{error}"
            : $"/Auth/ExternalAuthError/{error}/{description}";
}