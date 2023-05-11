/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHEditDialog<T>
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public string Title { get; set; } = default!;
    [Parameter] public string? CancelText { get; set; }
    [Parameter] public string? OkText { get; set; }
    [Parameter] public RenderFragment<T> Content { get; set; } = default!;
    [Parameter] public T Model { get; set; } = default!;
    [Parameter] public bool IsNew { get; set; }
    [Parameter] public Func<T, bool, Task<bool>> SubmitAsync { get; set; } = default!;

    public void Cancel() => MudDialog.Cancel();
    private T ModelTmp { get; set; } = default!;

    protected override void OnInitialized()
    {
        ModelTmp = Model;

        //TODO fix for copy value
        //ModelTmp = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(Model))!;

        CancelText ??= L["Cancel"];
        OkText ??= L["Ok"];
    }

    private async Task OnSubmitAsync(EditContext context)
    {
        context.NotifyValidationStateChanged();
        if (context.Validate())
        {
            foreach (var property in Model!.GetType().GetProperties().Where(a => a.CanWrite))
            {
                property.SetValue(Model, property.GetValue(ModelTmp));
            }

            var close = SubmitAsync != null && await SubmitAsync(Model, IsNew);
            StateHasChanged();

            if (close) { MudDialog.Close(); }
        }
    }
}