/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Identity;

namespace Corsinvest.AppHero.Core.Security.Identity;

public class ApplicationUser : IdentityUser
{
    public static string FolderProfileImage { get; set; } = "profile";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string AvatarName => $"{FirstName?.First()}{LastName?.First()}";
    //public string? TenantId { get; set; }
    //public string? TenantName { get; set; }

    [Required] public string DefaultCulture { get; set; } = "en-US";
    [Column(TypeName = "text")] public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    [NotMapped] public string UserManager { get; set; } = default!;
    public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = new List<ApplicationUserClaim>();
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; } = new List<ApplicationUserLogin>();
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; } = new List<ApplicationUserToken>();
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}