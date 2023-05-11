/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.MudBlazorUI.Style;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Security;

public partial class Users
{
    [Inject] private IOptionsSnapshot<Core.Security.Identity.Options> IdentityOptions { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] private RoleManager<ApplicationRole> RoleManager { get; set; } = default!;
    [Inject] private ICurrentUserService CurrentUserService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IDataGridManager<ApplicationUser> DataGridManager { get; set; } = default!;
    [Inject] private IOptionsSnapshot<UIOptions> UIOptions { get; set; } = default!;

    private TableOptions TableOptions => UIOptions.Value.Theme.Table;
    private ApplicationUser CurrentUser { get; set; } = default!;
    private IEnumerable<ApplicationRole> Roles { get; set; } = default!;
    private bool ShowRolesDialog { get; set; }
    private bool ShowDialogResetPassword { get; set; }
    private string Password { get; set; } = default!;
    private bool IsNew { get; set; }

    protected override void OnInitialized()
    {
        DataGridManager.Title = L["User"];
        DataGridManager.DefaultSort = new() { [nameof(ApplicationUser.UserName)] = false };
        DataGridManager.QueryAsync = async () => await Task.FromResult(UserManager.Users.Include(a => a.UserRoles).ThenInclude(a => a.Role));

        DataGridManager.BeforeEditAsync = async (item, isNew) =>
        {
            Password = "";
            IsNew = isNew;
            return await Task.FromResult(item);
        };

        DataGridManager.SaveAsync = async (item, isNew) =>
        {
            var result = isNew
                       ? await UserManager.CreateAsync(item, Password)
                       : await UserManager.UpdateAsync(item);

            UINotifier.Show(result.Succeeded, L["Saved successfully."], result.ToStringErrors());
            return result.Succeeded;
        };

        DataGridManager.SaveAfterAsync = async (user, isNew) =>
        {
            await Task.CompletedTask;
            if (user.Id == CurrentUserService.UserId)
            {
                //reload page for change language
                NavigationManager.NavigateTo(UrlHelper.SetParameter("/api/account/setculture",
                                                                    new()
                                                                    {
                                                                        { "culture", user.DefaultCulture},
                                                                        { "redirectUri", NavigationManager.Uri}
                                                                    }), true);
            }
        };

        DataGridManager.DeleteAsync = async (items) =>
        {
            var _currentUserId = CurrentUserService.UserId;

            IdentityResult result = null!;
            foreach (var item in items)
            {
                if (item.Id != _currentUserId)
                {
                    result = await UserManager.DeleteAsync(item);
                    if (!result.Succeeded) { break; }
                }
            }

            UINotifier.Show(result.Succeeded, L["Delete successfully."], result.ToStringErrors());
            return result.Succeeded;
        };
    }

    private void ShowRolesManager(ApplicationUser user)
    {
        CurrentUser = user;
        Roles = RoleManager.Roles.OrderBy(a => a.Name).ToArray();
        ShowRolesDialog = true;
    }

    private async Task AddRemoveRoleAsync(bool addOrRemove, string role)
    {
        if (addOrRemove)
        {
            await UserManager.AddRolesToUserAsync(CurrentUser, new[] { role });
        }
        else
        {
            await UserManager.RemoveRolesToUserAsync(CurrentUser, new[] { role });
        }
    }

    private async Task ResetPasswordAsync()
    {
        var result = await UserManager.ResetPasswordAsync(CurrentUser, Password);
        ShowDialogResetPassword = false;
        UINotifier.Show(result.Succeeded, L["Change password successfully."], result.ToStringErrors());
    }

    private void ResetPassword(ApplicationUser user)
    {
        //send email reset password
        //IdentityService.SendEmailResetPasswordAsync(user);  

        Password = "";
        CurrentUser = user;
        ShowDialogResetPassword = true;
    }
}