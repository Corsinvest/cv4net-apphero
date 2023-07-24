/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: Auth

namespace Corsinvest.AppHero.Core.Security.Auth;

public enum ExternalAuthError
{
    Unknown = -99,
    UserCreationFailed = -1,
    UserIsNotAllowed = 0,
    UserLockedOut = 1,
    CannotAddExternalLogin = 2,
    ExternalAuthError = 3,
    ExternalUnknownUserId = 4,
    ProviderNotFound = 5,
    UserNotActive = 6,
    AutoImportUser = 7,
}
