/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Emailing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Corsinvest.AppHero.Core.Security.Identity;

public class IdentityService : IIdentityService
{
    private readonly IStringLocalizer<IdentityService> L;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IServiceProvider _serviceProvider;

    public IdentityService(IServiceProvider serviceProvider,
                           UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _serviceProvider = serviceProvider;
        L = serviceProvider.GetRequiredService<IStringLocalizer<IdentityService>>();
    }

    public async Task SendEmailResetPasswordAsync(string email)
    {
        var mailService = _serviceProvider.GetRequiredService<IMailService>();
        var navigationManager = _serviceProvider.GetRequiredService<NavigationManager>();
        var emailTemplateService = _serviceProvider.GetRequiredService<IEmailTemplateService>();
        var identityOptions = _serviceProvider.GetRequiredService<IOptionsSnapshot<Security.Identity.Options>>().Value;
        var appOptions = _serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = (await userManager.FindByEmailAsync(email))!;
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var body = emailTemplateService.GenerateEmailTemplate(identityOptions.Template.ResetPassword,
                                                              new
                                                              {
                                                                  User = user,
                                                                  Url = $"{navigationManager.BaseUri}auth/password/reset/{user.Email}/{token}",
                                                                  ApplicationName = appOptions.Name
                                                              });

        await mailService.SendAsync(new MailRequest(new List<string>() { user.Email! },
                                                           string.Format(L["{0} - Reset password"], appOptions.Name),
                                                           body));
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles)
    {
        var identityOptions = _serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.IdentityOptions>();
        user.EmailConfirmed = !identityOptions.SignIn.RequireConfirmedEmail;

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, roles);

            //send email confirm
            if (identityOptions.SignIn.RequireConfirmedEmail)
            {
                await SendEmailConfirmRegistrationAsync(user);
            }
        }

        return result;
    }

    public async Task SendEmailConfirmRegistrationAsync(ApplicationUser user)
    {
        var mailService = _serviceProvider.GetRequiredService<IMailService>();
        var navigationManager = _serviceProvider.GetRequiredService<NavigationManager>();
        var emailTemplateService = _serviceProvider.GetRequiredService<IEmailTemplateService>();
        var identityOptions = _serviceProvider.GetRequiredService<IOptionsSnapshot<Security.Identity.Options>>().Value;
        var appOptions = _serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var body = emailTemplateService.GenerateEmailTemplate(identityOptions.Template.ConfirmRegistration,
                                                              new
                                                              {
                                                                  User = user,
                                                                  Url = $"{navigationManager.BaseUri}auth/email/validate/{user.Email}/{token}",
                                                                  ApplicationName = appOptions.Name
                                                              });

        await mailService.SendAsync(new MailRequest(new List<string>() { user.Email! },
                                                           string.Format(L["{0} - Confirm registration"], appOptions.Name),
                                                           body));
    }
}