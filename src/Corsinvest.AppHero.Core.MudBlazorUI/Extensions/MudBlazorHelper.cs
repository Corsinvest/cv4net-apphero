/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Humanizer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Extensions;

public static class MudBlazorHelper
{
    static MudBlazorHelper() => InitializeIcons();

    public static string ToMBIcon(string icon)
        => IconConvert.TryGetValue(icon + "", out var tmpIcon)
            ? tmpIcon
            : icon;


    private static Dictionary<string, string> IconConvert { get; set; } = default!;

    public static void InitializeIcons()
    {
        IconConvert = new Dictionary<string, string>();

        foreach (var item in Enum.GetValues<UIIcon>())
        {
            var value = item switch
            {
                UIIcon.None => string.Empty,
                UIIcon.Settings => Icons.Material.Outlined.Settings,
                UIIcon.Widget => Icons.Material.Outlined.Widgets,
                UIIcon.Link => Icons.Material.Outlined.Link,
                UIIcon.Create => Icons.Material.Outlined.Add,
                UIIcon.Delete => Icons.Material.Outlined.DeleteForever,
                UIIcon.Edit => Icons.Material.Outlined.Edit,
                UIIcon.Save => Icons.Material.Outlined.Save,
                UIIcon.Search => Icons.Material.Outlined.Search,
                UIIcon.ExportExcel => Icons.Custom.FileFormats.FileExcel,
                UIIcon.ExportPdf => Icons.Custom.FileFormats.FilePdf,
                UIIcon.AdminSecurity => Icons.Material.Outlined.AdminPanelSettings,
                UIIcon.Users => Icons.Material.Outlined.Groups,
                UIIcon.Roles => Icons.Material.Outlined.Security,
                UIIcon.Telegram => Icons.Custom.Brands.Telegram,
                UIIcon.Profile => Icons.Material.Outlined.AccountCircle,
                UIIcon.ManagePermissions => Icons.Material.Outlined.Policy,
                UIIcon.ViewPermissions => Icons.Material.Outlined.Policy,
                UIIcon.ManageRoles => Icons.Material.Outlined.Security,
                UIIcon.RestPassword => Icons.Material.Outlined.Key,
                UIIcon.ActiveUser => Icons.Material.Outlined.Person,
                UIIcon.DeactivateUser => Icons.Material.Outlined.PersonOff,
                UIIcon.Extension => Icons.Material.Outlined.Extension,
                UIIcon.Folder => Icons.Material.Outlined.Folder,
                UIIcon.Translate => Icons.Material.Outlined.Translate,
                UIIcon.Email => Icons.Material.Outlined.Email,
                UIIcon.Google => Icons.Custom.Brands.Google,
                UIIcon.Microsoft => Icons.Custom.Brands.Microsoft,
                UIIcon.Info => Icons.Material.Outlined.Info,
                UIIcon.CalendarMonth => Icons.Material.Outlined.CalendarMonth,
                UIIcon.PersonSearch => Icons.Material.Outlined.PersonSearch,
                UIIcon.SettingsApplications => Icons.Material.Outlined.SettingsApplications,
                UIIcon.Facebook => Icons.Custom.Brands.Facebook,
                UIIcon.Twitter => Icons.Custom.Brands.Twitter,
                UIIcon.Apple => Icons.Custom.Brands.Apple,
                UIIcon.Logout => Icons.Material.Outlined.Logout,
                UIIcon.MicrosoftAzure => Icons.Custom.Brands.MicrosoftAzure,
                UIIcon.MicrosoftWindows => Icons.Custom.Brands.MicrosoftWindows,
                UIIcon.Key => Icons.Material.Outlined.Key,
                _ => string.Empty,
            };

            if (!string.IsNullOrEmpty(value))
            {
                IconConvert.Add($"{nameof(UIIcon)}.{item}", value);
            }
        }
    }

    public static string GetDescriptionProperty<T>(Expression<Func<T, object>> propertyExpression)
    {
        var propertyInfo = ClassHelper.GetPropertyInfo(propertyExpression.Body);
        return propertyInfo == null
                ? null!
                : GetDescriptionProperty(propertyInfo.DeclaringType!, propertyInfo.Name);
    }

    public static string GetDescriptionProperty<T>(string propertyName) => GetDescriptionProperty(typeof(T), propertyName);

    public static string GetDescriptionProperty(Type type, string propertyName)
    {
        var label = string.Empty;
        var propertyInfo = type.GetProperty(propertyName);
        if (propertyInfo != null)
        {
            label = propertyInfo.GetCustomAttributes(typeof(LabelAttribute), true)
                                .Cast<LabelAttribute>()
                                .FirstOrDefault()?.Name ?? string.Empty;

            if (label == string.Empty)
            {
                label = propertyInfo?.GetCustomAttributes(typeof(DisplayAttribute), true)
                                     .Cast<DisplayAttribute>()
                                     .FirstOrDefault()?.Name ?? string.Empty;

                if (label == string.Empty)
                {
                    label = propertyInfo?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                         .Cast<DisplayNameAttribute>()
                                         .FirstOrDefault()?.DisplayName ?? string.Empty;
                }

                if (label == string.Empty) { label = propertyName.Humanize(LetterCasing.Title); }
            }
        }

        return label;
    }
}
