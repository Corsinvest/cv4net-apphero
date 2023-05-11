/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Validators;
using FluentValidation;

namespace Corsinvest.AppHero.Core.Security.Identity.Validators;

public class ApplicationUserValidator : AbstractModelValidator<ApplicationUser>
{
    public ApplicationUserValidator(IOptionsSnapshot<Options> IdentityOptions)
    {
        var user = RuleFor(a => a.UserName).NotEmpty();
        if (IdentityOptions.Value.LoginUseEmail) { user.EmailAddress(); }

        RuleFor(a => a.Email).NotEmpty();
        RuleFor(a => a.DefaultCulture).NotEmpty();
    }
}