/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
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
        IconConvert = [];

        foreach (var item in Enum.GetValues<UIIcon>())
        {
            var value = item switch
            {
                UIIcon.None => string.Empty,
                UIIcon.Settings => Icons.Material.Filled.Settings,
                UIIcon.Widget => Icons.Material.Filled.Widgets,
                UIIcon.Link => Icons.Material.Filled.Link,
                UIIcon.Create => Icons.Material.Filled.Add,
                UIIcon.Delete => Icons.Material.Filled.DeleteForever,
                UIIcon.Edit => Icons.Material.Filled.Edit,
                UIIcon.Save => Icons.Material.Filled.Save,
                UIIcon.Search => Icons.Material.Filled.Search,
                UIIcon.ExportExcel => Icons.Custom.FileFormats.FileExcel,
                UIIcon.ExportPdf => Icons.Custom.FileFormats.FilePdf,
                UIIcon.AdminSecurity => Icons.Material.Filled.AdminPanelSettings,
                UIIcon.Users => Icons.Material.Filled.Groups,
                UIIcon.Roles => Icons.Material.Filled.Security,
                UIIcon.Telegram => Icons.Custom.Brands.Telegram,
                UIIcon.Profile => Icons.Material.Filled.AccountCircle,
                UIIcon.ManagePermissions => Icons.Material.Filled.Policy,
                UIIcon.ViewPermissions => Icons.Material.Filled.Policy,
                UIIcon.ManageRoles => Icons.Material.Filled.Security,
                UIIcon.RestPassword => Icons.Material.Filled.Key,
                UIIcon.ActiveUser => Icons.Material.Filled.Person,
                UIIcon.DeactivateUser => Icons.Material.Filled.PersonOff,
                UIIcon.Extension => Icons.Material.Filled.Extension,
                UIIcon.Folder => Icons.Material.Filled.Folder,
                UIIcon.Translate => Icons.Material.Filled.Translate,
                UIIcon.Email => Icons.Material.Filled.Email,
                UIIcon.Google => Icons.Custom.Brands.Google,
                UIIcon.Microsoft => Icons.Custom.Brands.Microsoft,
                UIIcon.Info => Icons.Material.Filled.Info,
                UIIcon.CalendarMonth => Icons.Material.Filled.CalendarMonth,
                UIIcon.PersonSearch => Icons.Material.Filled.PersonSearch,
                UIIcon.SettingsApplications => Icons.Material.Filled.SettingsApplications,
                UIIcon.Facebook => Icons.Custom.Brands.Facebook,
                UIIcon.Twitter => Icons.Custom.Brands.Twitter,
                UIIcon.Apple => Icons.Custom.Brands.Apple,
                UIIcon.Logout => Icons.Material.Filled.Logout,
                UIIcon.MicrosoftAzure => Icons.Custom.Brands.MicrosoftAzure,
                UIIcon.MicrosoftWindows => Icons.Custom.Brands.MicrosoftWindows,
                UIIcon.Key => Icons.Material.Filled.Key,
                UIIcon.Discord => Icons.Custom.Brands.Discord,
                UIIcon.Chat => Icons.Material.Filled.Chat,
                UIIcon.WebHook => Icons.Material.Filled.Webhook,
                _ => string.Empty,
            };

            if (!string.IsNullOrEmpty(value)) { IconConvert.Add($"{nameof(UIIcon)}.{item}", value); }
        }
    }
}
