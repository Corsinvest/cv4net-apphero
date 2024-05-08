/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Authentication;

[AllowAnonymous]
public partial class Signup
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IIdentityService IdentityService { get; set; } = default!;
    [Inject] private IOptionsSnapshot<Core.Security.Identity.Options> IdentityOptions { get; set; } = default!;
    private bool IsValidPassword { get; set; }
    private MudForm? RefForm { get; set; } = default!;
    private RegisterModel Model { get; } = new();
    private RegisterModelValidator RegisterValidator { get; set; } = default!;
    private bool ShowWait { get; set; }

    class RegisterModel
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public bool AgreeToTerms { get; set; }
    }

    class RegisterModelValidator : AbstractModelValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.AgreeToTerms).Equal(true);
        }
    }

    protected override void OnInitialized()
    {
        if (!IdentityOptions.Value.CanRegister) { NavigationManager.NavigateTo("/"); }
    }

    private async Task SubmitAsync()
    {
        await RefForm!.Validate();
        if (RefForm.IsValid)
        {
            ShowWait = true;
            StateHasChanged();

            var result = await IdentityService.CreateUserAsync(new()
            {
                UserName = Model.UserName,
                Email = Model.Email,
            }, Model.Password, [RoleConstants.BasicRole]);

            if (result.Succeeded)
            {
                var msg = "Register successfully!";
                if (IdentityOptions.Value.SignIn.RequireConfirmedEmail)
                {
                    msg += Environment.NewLine + "Please check your mailbox!";
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }

                UINotifier.Show(L[msg], UINotifierSeverity.Info);
            }
            else
            {
                UINotifier.Show(result.ToStringErrors(), UINotifierSeverity.Error);
            }

            ShowWait = false;
            StateHasChanged();
        }
    }
}