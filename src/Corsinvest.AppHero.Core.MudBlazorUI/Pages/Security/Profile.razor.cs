/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using FluentValidation;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Security;

public partial class Profile
{
    [Inject] private IOptionsSnapshot<Core.Security.Identity.Options> IdentityOptions { get; set; } = default!;
    [Inject] private ICurrentUserService CurrentUserService { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private PasswordFluentValidator PasswordValidator { get; set; } = default!;
    private ApplicationUser User { get; set; } = default!;
    private MudForm? RefProfile { get; set; } = default!;
    private MudForm? RefChangepPassword { get; set; } = default!;
    private ChangePasswordModel ChangePassword { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        PasswordValidator = new(IdentityOptions.Value);
        User = (await UserManager.GetUserAsync(CurrentUserService.ClaimsPrincipal!))!;
    }

    private async Task UpdateUserAsync()
    {
        await RefProfile!.Validate();
        if (RefProfile.IsValid)
        {
            var result = await UserManager.UpdateAsync(User);
            UINotifier.Show(result.Succeeded, L["Update successfully."], result.ToStringErrors());

            if (result.Succeeded) { CurrentUserService.SetCulture(User.DefaultCulture, NavigationManager.Uri); }
        }
    }

    #region Change Password
    private async Task ChangePasswordAsync()
    {
        await RefChangepPassword!.Validate();
        if (RefChangepPassword.IsValid)
        {
            var result = await UserManager.ChangePasswordAsync(User, ChangePassword.CurrentPassword, ChangePassword.NewPassword);
            UINotifier.Show(result.Succeeded, L["Changed password successfully."], result.ToStringErrors());
        }
    }

    class ChangePasswordModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    class PasswordFluentValidator : AbstractModelValidator<ChangePasswordModel>
    {
        public PasswordFluentValidator(Core.Security.Identity.Options identityOptions)
        {
            RuleFor(a => a.CurrentPassword).NotEmpty();
            RuleFor(a => a.NewPassword).NotEmpty().MatchesPasswordOptions(identityOptions.Password);
            RuleFor(a => a.ConfirmPassword).NotEmpty().Equal(x => x.NewPassword);
        }
    }
    #endregion
}