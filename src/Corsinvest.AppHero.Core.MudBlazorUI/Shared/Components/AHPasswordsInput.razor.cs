/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHPasswordsInput
{
    [Parameter] public bool IsValid { get; set; }
    [Parameter] public EventCallback<bool> IsValidChanged { get; set; }
    [EditorRequired][Parameter] public PasswordOptions PasswordOptions { get; set; } = default!;

    [Inject] private IStringLocalizer<AHPasswordsInput> L { get; set; } = default!;

    private PasswordModel Model { get; set; } = new();
    private ResetPasswordFormModelValidator ModelValidator { get; set; } = default!;

    class PasswordModel
    {
        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string ConfirmPassword { get; set; } = default!;
    }

    class ResetPasswordFormModelValidator : AbstractModelValidator<PasswordModel>
    {
        public ResetPasswordFormModelValidator(PasswordOptions passwordOptions)
        {
            RuleFor(a => a.Password).MatchesPasswordOptions(passwordOptions);
            RuleFor(a => a.ConfirmPassword).Equal(x => x.Password);
        }
    }

    protected override void OnInitialized()
    {
        ModelValidator = new(PasswordOptions);
        base.OnInitialized();
    }

    private void IsValidChangedInt(bool isValid)
    {
        IsValid = ModelValidator.Validate(Model).IsValid;
        if (IsValidChanged.HasDelegate) { IsValidChanged.InvokeAsync(isValid); }

        Value = IsValid ? Model.Password : string.Empty;
        if (ValueChanged.HasDelegate) { ValueChanged.InvokeAsync(Value); }
    }
}