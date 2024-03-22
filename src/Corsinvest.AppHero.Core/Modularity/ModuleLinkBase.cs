/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Modularity;


public abstract class ModuleLinkBase<T> where T : ModuleLinkBase<T>
{
    public ModuleLinkBase(ModuleBase module,
                          string text,
                          string url = "",
                          bool isExternal = false,
                          bool inBasicRole = false)
    {
        Module = module;
        Text = text;
        Url = url;
        IsExternal = isExternal;
        SetLink(inBasicRole);
    }

    virtual protected string GetPermissionKey() => Module.PermissionLinkBaseKey;
    virtual protected string GetBaseUrl() => Module.BaseUrl;

    protected void SetLink(bool inBasicRole)
    {
        var permissionKey = Module.PermissionLinkBaseKey;
        var baseUrl = Module.BaseUrl;

        if (!string.IsNullOrWhiteSpace(Url)) { permissionKey += $".{Url}"; }

        Permission = new(permissionKey, $"{Name} {Text}", UIIcon.Link.GetName(), UIColor.Default, inBasicRole);

        if (IsExternal)
        {
            RealUrl = Url;
        }
        else
        {
            RealUrl = string.IsNullOrWhiteSpace(Url)
                            ? baseUrl
                            : $"{baseUrl}/{Url}";
        }
    }

    protected abstract string Name { get; }
    public ModuleBase Module { get; }
    public string Text { get; }
    public string Icon { get; set; } = default!;
    public string Url { get; }
    public string RealUrl { get; private set; } = default!;
    public UIColor IconColor { get; set; } = UIColor.Default;
    public bool Enabled { get; set; } = true;
    public int Order { get; set; }
    public bool IsExternal { get; }
    public Permission Permission { get; private set; } = default!;
}

//public abstract class ModuleLinkBase<T> where T : ModuleLinkBase<T>
//{
//    public ModuleLinkBase(ModuleBase module, string text, string url = "", bool isExternal = false)
//    {
//        Module = module;
//        Text = text;
//        Url = url;
//        IsExternal = isExternal;
//        SetLink();
//    }

//    public ModuleLinkBase(T parent, string text, string url = "", bool isExternal = false)
//    {
//        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
//        Module = parent.Module ?? throw new ArgumentNullException(nameof(parent.Module));
//        Text = text;
//        Url = url;
//        IsExternal = isExternal;
//        SetLink();
//    }

//    private void SetLink()
//    {
//        var permissionKey = Parent == null ? Module.PermissionLinkBaseKey : Parent.Permission.Key;
//        var baseUrl = Parent == null ? Module.BaseUrl : Parent.RealUrl;

//        if (!string.IsNullOrWhiteSpace(Url)) { permissionKey += $".{Url}"; }

//        Permission = new(permissionKey, $"{PrefixPermissionDescription} {Text}", UIIcon.Link.GetName());

//        if (IsExternal)
//        {
//            RealUrl = Url;
//        }
//        else
//        {
//            RealUrl = string.IsNullOrWhiteSpace(Url)
//                            ? baseUrl
//                            : $"{baseUrl}/{Url}";
//        }
//    }

//    protected abstract string PrefixPermissionDescription { get; }

//    public ModuleBase Module { get; }
//    public T? Parent { get; }
//    public string Text { get; }
//    public string Icon { get; set; } = default!;
//    public string Url { get; }
//    public string RealUrl { get; private set; } = default!;
//    public UIColor IconColor { get; set; } = UIColor.Default;
//    public bool Enabled { get; set; } = true;
//    public int Order { get; set; }
//    public bool IsExternal { get; }
//    public IEnumerable<T> Child { get; set; } = Array.Empty<T>();
//    public Permission Permission { get; private set; } = default!;
//    public IEnumerable<T> GetFlatLinks() => new[] { this }.Union(Child.Traverse(a => a.Child)).Cast<T>().ToList().AsReadOnly();
//}