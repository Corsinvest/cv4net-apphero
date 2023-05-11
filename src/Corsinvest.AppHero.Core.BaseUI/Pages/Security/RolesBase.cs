/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.Core.BaseUI.Pages.Security;

public partial class RolesBase : AHComponentBase
{
    [Inject] protected IDataGridManager<ApplicationRole> DataGridManager { get; set; } = default!;
    [Inject] private RoleManager<ApplicationRole> RoleManager { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private IUINotifier UINotifier { get; set; } = default!;

    private ApplicationRole CurrentRole { get; set; } = default!;
    protected bool ShowPermissionsDialog { get; set; }
    protected HashSet<string> CurrentPermissions { get; set; } = default!;

    protected bool CanManagePermissions { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CanManagePermissions = await PermissionService.HasPermissionAsync(Core.Security.Module.Permissions.Role.ManagePermissions);

        DataGridManager.Title = L["Role"];
        DataGridManager.DefaultSort = new() { [nameof(ApplicationRole.Name)] = false };
        DataGridManager.QueryAsync = async () => await Task.FromResult(RoleManager.Roles.Include(a => a.Claims));

        DataGridManager.SaveAsync = async (item, isNew) =>
        {
            var result = isNew
                       ? await RoleManager.CreateAsync(item)
                       : await RoleManager.UpdateAsync(item);

            UINotifier.Show(result.Succeeded, L["Saved successfully."], result.ToStringErrors());
            return result.Succeeded;
        };

        DataGridManager.DeleteAsync = async (items) =>
        {
            var ret = true;
            foreach (var item in items)
            {
                var result = await RoleManager.DeleteAsync(item);
                if (result.Succeeded)
                {
                    if (!result.Succeeded)
                    {
                        UINotifier.Show(result.ToStringErrors(), UINotifierSeverity.Error);
                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        };
    }

    protected void ShowPermissionsManager(ApplicationRole role)
    {
        CurrentRole = role;
        CurrentPermissions = role.Claims.Where(a => a.ClaimType == ApplicationClaimTypes.Permission)
                                        .Select(a => a.ClaimValue!)
                                        .ToHashSet();
        ShowPermissionsDialog = true;
    }

    protected async Task PermissionGrantedAsync(string key) => await RoleManager.AddPermissionsAsync(CurrentRole, new[] { key });
    protected async Task PermissionDeniedAsync(string key) => await RoleManager.RemovePermissionsAsync(CurrentRole, new[] { key });
}