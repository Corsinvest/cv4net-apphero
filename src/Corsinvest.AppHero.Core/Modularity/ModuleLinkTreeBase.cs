/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Modularity;

public abstract class ModuleLinkTreeBase<T> : ModuleLinkBase<T>
    where T : ModuleLinkTreeBase<T>
{
    protected ModuleLinkTreeBase(ModuleBase module, string text, string url = "", bool isExternal = false, bool inBasicRole = false)
        : base(module, text, url, isExternal, inBasicRole) { }

    public ModuleLinkTreeBase(T parent, string text, string url = "", bool isExternal = false, bool inBasicRole = false)
         : base(parent.Module, text, url, isExternal, inBasicRole)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        SetLink(inBasicRole);
    }

    protected override string GetPermissionKey() => Parent == null ? base.GetPermissionKey() : Parent.Permission.Key;
    protected override string GetBaseUrl() => Parent == null ? base.GetBaseUrl() : Parent.RealUrl;

    public T? Parent { get; }

    public IEnumerable<T> Child { get; set; } = Array.Empty<T>();
    public IEnumerable<T> GetFlatLinks() => new[] { this }.Union(Child.Traverse(a => a.Child)).Cast<T>().ToList().AsReadOnly();
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