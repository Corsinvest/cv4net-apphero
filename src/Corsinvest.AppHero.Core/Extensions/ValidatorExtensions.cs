/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Corsinvest.AppHero.Core.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MatchesPasswordOptions<T>(this IRuleBuilder<T, string> ruleBuilder, PasswordOptions passwordOptions)
        => ruleBuilder.NotEmpty()
                      .WithMessage("Your password cannot be empty")

                      .MinimumLength(passwordOptions.RequiredLength)
                      .WithMessage($"Your password length must be at least {passwordOptions.RequiredLength}.")

                      .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                      .When(a => passwordOptions.RequireUppercase, ApplyConditionTo.CurrentValidator)

                      .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                      .When(a => passwordOptions.RequireLowercase, ApplyConditionTo.CurrentValidator)

                      .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                      .When(a => passwordOptions.RequireDigit, ApplyConditionTo.CurrentValidator)

                      .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).")
                      .When(a => passwordOptions.RequireDigit, ApplyConditionTo.CurrentValidator);
}