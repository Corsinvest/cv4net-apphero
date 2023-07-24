/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: Auth

using Corsinvest.AppHero.Core.Security.Auth;

namespace Corsinvest.AppHero.Core.BaseUI.Pages.Authentication;

public class ExternalAuthErrorBase : AHComponentBase
{
    [Parameter] public string ErrorEnumValue { get; set; } = default!;
    [Parameter] public string Description { get; set; } = default!;

    protected string ErrorDecode { get; set; } = default!;

    protected override void OnInitialized()
        => Description = Enum.TryParse<ExternalAuthError>(ErrorEnumValue, out var enumValue)
                            ? enumValue switch
                            {
                                ExternalAuthError.UserCreationFailed => L["User cannot be created"],
                                ExternalAuthError.UserIsNotAllowed => L["Login not allowed, check email inbox for account confirmation"],
                                ExternalAuthError.UserLockedOut => L["User is locked out"],
                                ExternalAuthError.CannotAddExternalLogin => L["Cannot create binding for this external login provider to the account"],
                                ExternalAuthError.ExternalAuthError => L["External provider cannot authenticate.\nCheck configuration."],
                                ExternalAuthError.ExternalUnknownUserId => L["External authentication provider did not pass user identifier"],
                                ExternalAuthError.ProviderNotFound => L["Choosen provider has not been found/configured"],
                                ExternalAuthError.UserNotActive => L["User Not Active"],
                                ExternalAuthError.Unknown => L["Unknown reason"],
                                ExternalAuthError.AutoImportUser => L["Auto Import User disabled"],
                                _ => L["Unknown reason"],
                            }
                            : L["Unknown reason"];
}