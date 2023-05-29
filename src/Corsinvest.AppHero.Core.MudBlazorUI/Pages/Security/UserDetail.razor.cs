/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Service;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Security;

public partial class UserDetail
{
    [EditorRequired][Parameter] public ApplicationUser User { get; set; } = default!;
    [Parameter] public bool ManageAccount { get; set; }
    [Parameter] public bool Creation { get; set; }
    [Parameter] public string Password { get; set; } = default!;

    [Required]
    private string PasswordInt
    {
        get => Password;
        set
        {
            Password = value;
            if (PasswordChanged.HasDelegate)
            {
                PasswordChanged.InvokeAsync(value);
            }
        }
    }

    [Parameter] public EventCallback<string> PasswordChanged { get; set; } = default!;

    [Inject] private IOptionsSnapshot<Core.Security.Identity.Options> IdentityOptions { get; set; } = default!;
    [Inject] private IOptions<RequestLocalizationOptions> LocalizationOptions { get; set; } = default!;
    [Inject] private IFileStorageService FileStorageService { get; set; } = default!;

    //protected override void OnInitialized()
    //{
    //    base.OnInitialized();
    //    Model = User.Adapt<ApplicationUserDto>();
    //    Model.Password = string.Empty;
    //}

    private void ClearPhoto()
    {
        FileStorageService.Remove(Path.GetFileName(User.ProfileImageUrl)!, ApplicationUser.FolderProfileImage);
        User.ProfileImageUrl = ApplicationHelper.GetGravatar(User.Email!);
    }

    private async Task UploadPhotoAsync(InputFileChangeEventArgs e)
    {
        using var fileStream = e.File.OpenReadStream(1024 * 1024);
        using var imgStream = new MemoryStream();
        await fileStream.CopyToAsync(imgStream);
        imgStream.Position = 0;

        using var image = Image.Load(imgStream);
        //var format = await Image.DetectFormatAsync(imgStream);

        image.Mutate(i => i.Resize(new ResizeOptions()
        {
            Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop,
            Size = new SixLabors.ImageSharp.Size(128, 128)
        }));

        using var outStream = new MemoryStream();
        image.Save(outStream, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);

        var url = await FileStorageService.UploadAsync(outStream, e.File.Name, ApplicationUser.FolderProfileImage);
        User.ProfileImageUrl = string.IsNullOrWhiteSpace(url)
                                ? ApplicationHelper.GetGravatar(User.Email!)
                                : "/" + url;

        StateHasChanged();
    }
}