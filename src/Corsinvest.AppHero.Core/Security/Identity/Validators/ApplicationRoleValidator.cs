/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Validators;
using FluentValidation;

namespace Corsinvest.AppHero.Core.Security.Identity.Validators;

public class ApplicationRoleValidator : AbstractModelValidator<ApplicationRole>
{
    public ApplicationRoleValidator()
    {
        RuleFor(a => a.Name).NotEmpty();
        RuleFor(a => a.Description).NotEmpty();
    }
}
