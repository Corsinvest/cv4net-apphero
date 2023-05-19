/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;
using Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components.DataGrid;
using Corsinvest.AppHero.Core.MudBlazorUI.Shared.Options;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;
using Color = MudBlazor.Color;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Extensions;

public static class MudBlazorExtensions
{
    public static DataGridManager<T> ToDataGridManager<T>(this IDataGridManager<T> dataGridManager) where T : class
        => (DataGridManager<T>)dataGridManager;

    public static Color ToMBColor(this UIColor color)
        => color switch
        {
            UIColor.Default => Color.Default,
            UIColor.Primary => Color.Primary,
            UIColor.Secondary => Color.Secondary,
            UIColor.Tertiary => Color.Tertiary,
            UIColor.Info => Color.Info,
            UIColor.Success => Color.Success,
            UIColor.Warning => Color.Warning,
            UIColor.Error => Color.Error,
            UIColor.Dark => Color.Dark,
            UIColor.Transparent => Color.Transparent,
            UIColor.Inherit => Color.Inherit,
            UIColor.Surface => Color.Surface,
            _ => Color.Default,
        };

    public static async Task<bool> ShowDialogEditorAsync<T>(this IDialogService dialogService,
                                                            string title,
                                                            RenderFragment<T> content,
                                                            T model,
                                                            bool isNew,
                                                            Func<T, bool, Task<bool>> submitAsync,
                                                            DialogOptions dialogOptions,
                                                            string cancelText = null!,
                                                            string okText = null!)
    {
        var parameters = new DialogParameters
            {
                { nameof(AHEditDialog<T>.Title), title },
                { nameof(AHEditDialog<T>.CancelText), cancelText},
                { nameof(AHEditDialog<T>.OkText), okText},
                { nameof(AHEditDialog<T>.Content), content },
                { nameof(AHEditDialog<T>.Model), model},
                { nameof(AHEditDialog<T>.IsNew), isNew},
                { nameof(AHEditDialog<T>.SubmitAsync), submitAsync}
            };

        var dialog = dialogService.Show<AHEditDialog<T>>(title, parameters, dialogOptions);
        var result = await dialog.Result;
        return !result.Canceled;
    }

    public static async Task<DialogResult> ShowDialogOptionsAsync(this ModuleBase module, IDialogService dialogService)
        => await dialogService.Show<AHDialogEditOptions>(null,
                                                         new DialogParameters { [nameof(AHDialogEditOptions.Module)] = module },
                                                         new DialogOptions
                                                         {
                                                             CloseButton = false,
                                                             CloseOnEscapeKey = false,
                                                             MaxWidth = MaxWidth.False,
                                                         }).Result;

    public static async Task<bool> ShowQuestionAsync(this IDialogService dialogService, string title, string message)
        => await dialogService.ShowMessageBox(title,
                                              message,
                                              "Yes",
                                              "No",
                                              null,
                                              new DialogOptions()
                                              {
                                                CloseButton = true,
                                                DisableBackdropClick = true,
                                                MaxWidth = MaxWidth.ExtraSmall,
                                                FullWidth = true,
                                               }) ?? false;
    //=> await dialogService.ShowConfirmationDialogAsync(title,
    //                                                   message,
    //                                                   "Yes",
    //                                                   "No",
    //                                                   Icons.Material.Filled.QuestionMark,
    //                                                   new DialogOptionsEx()
    //                                                   {
    //                                                       DragMode = MudDialogDragMode.Simple,
    //                                                       CloseButton = true,
    //                                                       DisableBackdropClick = true,
    //                                                       MaxWidth = MaxWidth.ExtraSmall,
    //                                                       FullWidth = true,
    //                                                       Animations = new[] { AnimationType.FlipX }
    //                                                   });

    public static async Task ShowInfoAsync(this IDialogService dialogService, string title, string message)
        => await dialogService.ShowMessageBox(title, message, "Ok");
    //=> await dialogService.ShowInformationAsync(title, message, Icons.Material.Filled.Info, canClose: true);


    public static async Task ShowErrorAsync(this IDialogService dialogService, string title, string message)
        => await dialogService.ShowMessageBox(title, message, "Ok");

    public static async Task<bool> DeleteAsync(this IDialogService dialogService, string contentText)
        => await dialogService.ShowQuestionAsync(contentText, "Delete");

    public static async Task<string?> PromptAsync(this IDialogService dialogService, string title, string message, string contentText)
        => await dialogService.PromptAsync(title,
                                           message,
                                           contentText,
                                           "Ok",
                                           "Cancel",
                                           Icons.Material.Filled.VerifiedUser,
                                           s => !string.IsNullOrEmpty(s),
                                           new DialogOptionsEx()
                                           {
                                               DragMode = MudDialogDragMode.Simple,
                                               CloseButton = true,
                                               DisableBackdropClick = true,
                                               MaxWidth = MaxWidth.ExtraSmall,
                                               FullWidth = true,
                                               Animations = new[] { AnimationType.FlipX }
                                           });
}
