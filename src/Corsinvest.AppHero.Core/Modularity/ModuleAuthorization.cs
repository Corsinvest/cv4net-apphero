/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.Modularity;

public class ModuleAuthorization
{
    internal static async Task<ModuleAuthorization> GetInstanceAsync(IPermissionService permissionService, ModuleBase module)
        => new ModuleAuthorization()
        {
            Module = module,
            Authorization = await permissionService.HasPermissionsAsync(module.GetPermissions())
        };

    public ModuleBase Module { get; private set; } = default!;
    public IReadOnlyDictionary<string, bool> Authorization { get; private set; } = default!;
    public bool Has(string permissionKey) => Exist(permissionKey) && Authorization[permissionKey];
    public bool Has(Permission permission) => Has(permission.Key);

    public bool Exist(string permissionKey) => Authorization.ContainsKey(permissionKey);

    public bool HasAuthorizedLink(IEnumerable<string> subItems)
    {
        var items = new List<string>
        {
            $"{Module.PermissionLinkBaseKey}"
        };
        items.AddRange(subItems.Where(a => !string.IsNullOrWhiteSpace(a)));
        return Has(items.JoinAsString("."));
    }

    public bool CanWidget(ModuleWidget widget) => Has(widget.Permission);
    public IEnumerable<ModuleLink> Links => Module.GetFlatLinks().Where(a => a.Enabled && Has(a.Permission));
    public IEnumerable<ModuleMenuItem> MenuItems => Module.MenuItems.Where(a => a.Enabled && Has(a.Permission));
    public IEnumerable<ModuleWidget> GetWidgets() => Module.Widgets.Where(a => Has(a.Permission));
}