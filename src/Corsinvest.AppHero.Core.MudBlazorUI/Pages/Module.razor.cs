/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth.Permissions;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages;

[Authorize]
public partial class Module
{
    [Parameter] public string? PageRoute { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IPermissionService PermissionService { get; set; } = default!;
    [Inject] private IOptionsSnapshot<AppOptions> AppOptions { get; set; } = default!;

    public DynamicComponent? DCRender { get; set; }
    private bool Authorized { get; set; }
    private bool CanOptions { get; set; }
    private IEnumerable<ModuleMenuItem> MenuItems { get; set; } = Enumerable.Empty<ModuleMenuItem>();
    private ModuleBase? CurrentModule { get; set; }
    private Type? Render { get; set; }
    private List<BreadcrumbItem> BreadcrumbsItems { get; set; } = new()!;
    private string ModuleSlug { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        BreadcrumbsItems.Clear();
        Authorized = false;
        CanOptions = false;

        var data = (PageRoute + "").Split('/');
        ModuleSlug = data[0];
        var subItems = data.Skip(1).ToList();

        CurrentModule = ModularityService.GetBySlug(ModuleSlug);
        if (CurrentModule == null)
        {
            NavigationManager.NavigateTo("/NotFound", false);
            return;
        }

        //permission
        var auth = await CurrentModule.GetAuthorizationAsync(PermissionService);
        Authorized = auth.HasAuthorizedLink(subItems);
        if (!Authorized)
        {
            NavigationManager.NavigateTo("/NotAuthorized", false);
            return;
        }

        CanOptions = auth.Has(CurrentModule.PermissionEditOptions);

        MenuItems = auth.MenuItems;

        //category
        BreadcrumbsItems.Add(new(CurrentModule.Category,
                                 NavigationManager.Uri,
                                 false,
                                 MudBlazorHelper.ToMBIcon(ModularityService.GetCategoryIcon(CurrentModule.Category)!)));

        //module
        BreadcrumbsItems.Add(new(CurrentModule.Link!.Text,
                                 NavigationManager.Uri,
                                 false,
                                 CurrentModule.Link.ToMBIcon()));

        //type component to render 
        Render = CurrentModule.Link.Render;

        //if (subItems.Any() && CurrentModule.Link!.Url == subItems[0])
        //{
        //    Render = CurrentModule.Link.Render;
        //    subItems.RemoveAt(0);
        //}

        var links = CurrentModule.Link.Child;
        foreach (var item in subItems)
        {
            if (string.IsNullOrWhiteSpace(item)) { break; }
            var link = links.FirstOrDefault(a => a.Text == item);
            if (link == null) { break; }

            BreadcrumbsItems.Add(new(link.Text,
                                     NavigationManager.Uri, //link.Url,
                                     false,
                                     link.ToMBIcon()));

            Render = link.Render;
            links = link.Child;
        }
    }

    private async Task ShowOptionsAsync() => await CurrentModule!.ShowDialogOptionsAsync(DialogService);
    private async Task RefreshAsync() => await ((IRefreshable)DCRender!.Instance!).Refresh();
}