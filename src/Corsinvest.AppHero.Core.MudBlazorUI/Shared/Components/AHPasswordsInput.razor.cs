/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHPasswordsInput
{
    [Parameter] public bool IsValid { get; set; }
    [Parameter] public EventCallback<bool> IsValidChanged { get; set; }
    [Parameter] public string Password { get; set; } = default!;
    [Parameter] public EventCallback<string> PasswordChanged { get; set; }
    [Parameter] public string Label { get; set; } = default!;
    [EditorRequired][Parameter] public PasswordOptions PasswordOptions { get; set; } = default!;

    [Inject] private IStringLocalizer<AHPasswordsInput> L { get; set; } = default!;

    private MudForm RefForm { get; set; } = default!;

    private ChangePasswordModel Model { get; set; } = new();
    private ResetPasswordFormModelValidator ModelValidator { get; set; } = default!;

    class ChangePasswordModel
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    class ResetPasswordFormModelValidator : AbstractModelValidator<ChangePasswordModel>
    {
        public ResetPasswordFormModelValidator(PasswordOptions passwordOptions)
        {
            RuleFor(a => a.Password).NotEmpty().MatchesPasswordOptions(passwordOptions);
            RuleFor(a => a.ConfirmPassword).Equal(x => x.Password);
        }
    }

    protected override void OnInitialized() => ModelValidator = new(PasswordOptions);

    private async Task FieldChanged() 
    {
        await RefForm.Validate();
    }

    private void IsValidChangedInt(bool isValid)
    {
        IsValid = ModelValidator.Validate(Model).IsValid;
        if (IsValidChanged.HasDelegate) { IsValidChanged.InvokeAsync(IsValid); }

        Password = IsValid ? Model.Password : string.Empty;
        if (PasswordChanged.HasDelegate) { PasswordChanged.InvokeAsync(Password); }
    }
}