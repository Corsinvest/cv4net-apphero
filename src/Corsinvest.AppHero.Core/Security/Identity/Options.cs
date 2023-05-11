/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Corsinvest.AppHero.Core.Security.Identity;

public class Options
{
    public Options()
    {
        SignIn.RequireConfirmedEmail = false;
        SignIn.RequireConfirmedPhoneNumber = false;

        Password.RequiredLength = 6;
        Password.RequireDigit = false;
        Password.RequireLowercase = false;
        Password.RequireNonAlphanumeric = false;
        Password.RequireUppercase = false;

        Lockout.MaxFailedAccessAttempts = 5;
        Lockout.AllowedForNewUsers = true;

        User.RequireUniqueEmail = true;
    }

    public TemplateOptions Template { get; set; } = new TemplateOptions();
    public bool EnableLocalLogin { get; set; } = true;
    public int LoginCookieExpirationHours { get; set; } = 12;
    public bool LoginUseEmail { get; set; } = true;
    public bool CanRegister { get; set; } = false;
    public UserOptions User { get; set; } = new UserOptions();
    public PasswordOptions Password { get; set; } = new PasswordOptions();
    public LockoutOptions Lockout { get; set; } = new LockoutOptions();
    public SignInOptions SignIn { get; set; } = new SignInOptions();
    public Microsoft.AspNetCore.Identity.IdentityOptions ToIdentityOptions() => this.Adapt<Microsoft.AspNetCore.Identity.IdentityOptions>();
    public void SetIdentityOptions(Microsoft.AspNetCore.Identity.IdentityOptions options) => this.Adapt(options);
}
