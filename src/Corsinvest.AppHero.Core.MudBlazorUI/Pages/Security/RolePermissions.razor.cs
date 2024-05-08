/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Security;

public partial class RolePermissions
{
    [EditorRequired][Parameter] public HashSet<string> Permissions { get; set; } = [];
    [Parameter] public bool CanManagePermissions { get; set; }
    [Parameter] public EventCallback<string> PermissionGranted { get; set; }
    [Parameter] public EventCallback<string> PermissionDenied { get; set; }

    [Inject] private IModularityService ModularityService { get; set; } = default!;

    class TreeDataPermission
    {
        public string? Key { get; set; }
        public string Path { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public string Description { get; set; } = default!;
        public MudBlazor.Color Color { get; set; }
        public bool IsExpanded { get; set; } = false;
        public bool? IsChecked { get; set; } = false;
        public bool HasChild => Child.Count > 0;
        public TreeDataPermission? Parent { get; set; }
        public HashSet<TreeDataPermission> Child { get; set; } = [];
    }

    private HashSet<TreeDataPermission> TreeData { get; set; } = [];
    protected override void OnInitialized() => RefreshData();

    private void RefreshData()
    {
        TreeData.Clear();

        foreach (var module in ModularityService.Modules.OrderBy(a => a.Description))
        {
            foreach (var permission in module.GetPermissions().OrderBy(a => a.Key))
            {
                TreeDataPermission parent = null!;
                TreeDataPermission treeItem = null!;

                var paths = permission.Key.Split(".").ToList();

                //find first node
                var data = TreeData;
                foreach (var item in paths.ToArray())
                {
                    treeItem = data.FirstOrDefault(a => a.Description == item)!;
                    if (treeItem == null) { break; }
                    paths.Remove(item);
                    parent = treeItem;
                    data = treeItem.Child;
                }

                //create nodes
                foreach (var item in paths)
                {
                    treeItem = new TreeDataPermission()
                    {
                        Description = item,
                        Parent = parent
                    };

                    if (parent != null) { treeItem.Path = parent.Path + "/"; }
                    treeItem.Path += item;

                    if (module.Class == treeItem.Path.Replace("/", ".")) { treeItem.Icon = module.ToMBIcon(); }

                    if (parent == null)
                    {
                        TreeData.Add(treeItem);
                    }
                    else
                    {
                        parent.Child.Add(treeItem);
                    }

                    parent = treeItem;
                }

                treeItem!.IsChecked = Permissions.Contains(permission.Key);
                treeItem.Key = permission.Key;
                FixParent(treeItem);
                treeItem.Icon = permission.ToMBIcon();
                treeItem.Color = permission.Color.ToMBColor();
                treeItem.Description = permission.Description;
            }
        }

        //foreach (var module in ModularityService.Modules.OrderBy(a => a.Description))
        //{
        //    var permissions = module.GetPermissions();
        //    if (permissions.Any())
        //    {
        //        var treeItem = new TreeDataPermission()
        //        {
        //            Description = module.Description,
        //            Icon = module.ToMBIcon(),
        //            Path = "/"
        //        };
        //        TreeData.Add(treeItem);

        //        foreach (var item in permissions)
        //        {
        //            //Substring(module.Class.Length + 1)
        //            var keys = item.Key.Split(".");
        //            var key = keys.Length > 0
        //                        ? keys[1..].JoinAsString(".")
        //                        : item.Key;

        //            AddChild(treeItem, key, item, permissions);
        //        }

        //        foreach (var item in treeItem.Child.Traverse(a => a.Child).Where(a => Permissions.Contains(a.Key!)))
        //        {
        //            item.IsChecked = true;
        //            FixParent(item);
        //        }
        //    }
        //}
    }

    //private static void AddChild(TreeDataPermission parent, string key, Permission permission, IEnumerable<Permission> permissions)
    //{
    //    var keys = key.Split(".");
    //    var path = $"{parent.Path}/{keys[0]}";
    //    var item = parent.Child.FirstOrDefault(a => a.Path == path);
    //    if (item == null)
    //    {
    //        var hasChild = keys.Length > 1;
    //        item = new TreeDataPermission()
    //        {
    //            Path = path,
    //            Key = hasChild ? null : permission.Key,
    //            Parent = parent,
    //            Description = hasChild ? keys[0] : permission.Description,
    //            Icon = hasChild ? null! : permission.ToMBIcon(),
    //            Color = hasChild ? MudBlazor.Color.Default : permission.Color.ToMBColor(),
    //        };

    //        parent.Child.Add(item);
    //    }

    //    if (keys.Length > 1)
    //    {
    //        AddChild(item, keys[1..].JoinAsString("."), permission, permissions);
    //    }
    //}

    private static void FixParent(TreeDataPermission item)
    {
        var parent = item.Parent;
        while (parent != null)
        {
            parent.IsChecked = parent.Child.Any(i => i.IsChecked is null)
                                ? null
                                : parent.Child.All(i => i.IsChecked is true)
                                    ? true
                                    : parent.Child.All(i => i.IsChecked is false)
                                        ? false
                                        : null;
            parent = parent.Parent;
        }
    }

    private async Task ValueChangedAsync(TreeDataPermission item)
    {
        item.IsChecked = !(item.IsChecked ?? true);
        // checked status on any item items should mirror this parent item
        if (item.HasChild)
        {
            foreach (var child in item.Child.Traverse(a => a.Child)) { child.IsChecked = item.IsChecked; }
        }

        FixParent(item);

        await PermissionChangedAsync(item);
        foreach (var prerm in item.Child.Traverse(a => a.Child).Where(a => a.Key != null))
        {
            if (prerm != null) { await PermissionChangedAsync(prerm); }
        }
    }

    private async Task PermissionChangedAsync(TreeDataPermission permission)
    {
        if (permission.Key != null)
        {
            if (permission.IsChecked is true)
            {
                Permissions.Add(permission.Key);
                await PermissionGranted.InvokeAsync(permission.Key);
            }
            else
            {
                Permissions.Remove(permission.Key);
                await PermissionDenied.InvokeAsync(permission.Key);
            }
        }
    }
}
