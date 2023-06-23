/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Hubs;
using Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;
using Corsinvest.AppHero.Core.MudBlazorUI.Style;
using Corsinvest.AppHero.Core.SoftwareUpdater;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared;

public partial class MainLayout : IAsyncDisposable
{
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject] private IStringLocalizer<MainLayout> L { get; set; } = default!;
    [Inject] private IOptionsSnapshot<AppOptions> SnapshotAppOptions { get; set; } = default!;
    [Inject] private LayoutService LayoutService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private HubClient HubClient { get; set; } = default!;
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IServiceScopeFactory ServiceScopeFactory { get; set; } = default!;

    private AppOptions AppOptions => SnapshotAppOptions.Value;
    private bool DrawerOpen { get; set; } = true;
    private ReleaseInfo? ReleaseInfo { get; set; }
    private IReleaseService? _releaseService;
    private Timer? _timer;
    private MudThemeProvider? RefMudThemeProvider { get; set; }

    public async ValueTask DisposeAsync()
    {
        LayoutService.MajorUpdateOccured -= LayoutService_MajorUpdateOccured;

        _timer?.Dispose();

        await HubClient.StopAsync();
        HubClient.LoggedOut -= HubClient_LoggedOut;
        HubClient.LoggedIn -= HubClient_LoggedIn;
        HubClient.JobStarted -= HubClient_JobStarted;
        HubClient.JobCompleted -= HubClient_JobCompleted;
        HubClient.MessageReceived -= HubClient_MessageReceived;
        HubClient.NotificationReceived -= HubClient_NotificationReceived;

        AuthenticationStateProvider.AuthenticationStateChanged -= AuthenticationStateProvider_AuthenticationStateChanged;
        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var darkMode = await RefMudThemeProvider!.GetSystemPreference();
            LayoutService.Initialize(darkMode);

            await RefMudThemeProvider.WatchSystemPreference(LayoutService.OnSystemPreferenceChanged);

            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        LayoutService.MajorUpdateOccured += LayoutService_MajorUpdateOccured;
        using var scope = ServiceScopeFactory.CreateScope();
        _releaseService = scope.ServiceProvider.GetService<IReleaseService>();
        if (_releaseService != null)
        {
            _timer = new Timer(async (o) => ReleaseInfo = await _releaseService.NewReleaseIsAvaibleAsync(),
                               null,
                               TimeSpan.Zero,
                               TimeSpan.FromMinutes(5));
        }

        HubClient.LoggedOut += HubClient_LoggedOut;
        HubClient.LoggedIn += HubClient_LoggedIn;
        HubClient.JobStarted += HubClient_JobStarted;
        HubClient.JobCompleted += HubClient_JobCompleted;
        HubClient.MessageReceived += HubClient_MessageReceived;
        HubClient.NotificationReceived += HubClient_NotificationReceived;

        await SetUser(AuthState);
        AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
    }

    private void LayoutService_MajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
    private async Task OpenRelease() => await JSRuntime.InvokeVoidAsync("open", ReleaseInfo!.Url, "_blank");
    private void HubClient_NotificationReceived(object? sender, string e) { }
    private async Task HubClient_MessageReceived(object sender, MessageReceivedEventArgs e) { await Task.CompletedTask; }
    private void HubClient_JobCompleted(object? sender, string jobId) { }
    private void HubClient_JobStarted(object? sender, string jobId) { }

    private void HubClient_LoggedIn(object? sender, string userId)
    {
        //InvokeAsync(async () =>
        //{
        //    //    //var username = await IdentityService.GetUserNameAsync(userId);
        //    //    //Snackbar.Add($"{username} login.", Severity.Info);
        //    //    await IdentityService.UpdateLiveStatus(userId, true);
        //});
    }

    private void HubClient_LoggedOut(object? sender, string userId)
    {
        //InvokeAsync(async () =>
        //{
        //    //    //var username = await IdentityService.GetUserNameAsync(userId);
        //    //    //Snackbar.Add($"{username} logout.", Severity.Normal);
        //    //    await IdentityService.UpdateLiveStatus(userId, false);
        //});
    }

    private async Task SetUser(Task<AuthenticationState> authenticationState)
    {
        var state = await authenticationState;
        if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
            await HubClient.StartAsync();
            StateHasChanged();
        }
    }

    private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
        => InvokeAsync(async () => await SetUser(authenticationState));

    private async Task OpenFinder()
    {
        var finder = DialogService.Show<AHAppFinder>("", new DialogOptions
        {
            CloseOnEscapeKey = true,
            NoHeader = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        });

        await finder.Result;
    }

    private async Task ShowHelp()
        => await JSRuntime.InvokeVoidAsync("open",
                                           ModularityService.GetByUrl(NavigationManager.Uri)?.UrlHelp ?? ApplicationHelper.HelpUrl,
                                           "_blank");
}

//public partial class MainLayout : INotificationHandler<EventNotification<NotifySnackBarEvent>>
//{
//    public async Task Handle(EventNotification<NotifySnackBarEvent> notification, CancellationToken cancellationToken)
//    {
//        //        if (AuthState != null)
//        //        {
//        //            var state = await AuthState;
//        //            if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
//        //            {
//        //var evt = notification.Event;
//        //Snackbar.Add(evt.Message, evt.Severity);
//        //            }
//        //        }
//        //InvokeAsync(() => StateHasChanged());
//    }
//}

//class NotifyEventHandler : 
//{

//    //public TenantChangedEventHandler(
//    //    ITenantsService tenantsService,
//    //    ILogger<KeyValueChangedEventHandler> logger
//    //    )
//    //{
//    //    _tenantsService = tenantsService;
//    //    _logger = logger;
//    //}
//    //public async Task Handle(UpdatedEvent<Tenant> notification, CancellationToken cancellationToken)
//    //{
//    //    _logger.LogInformation("Tenant Changed {DomainEvent},{Entity}", nameof(notification), notification.Entity);
//    //    await _tenantsService.Refresh();

//    //}

//    /// <summary>
//    /// <inheritdoc/>
//    /// </summary>
//    /// <param name="notification"></param>
//    /// <param name="cancellationToken"></param>
//    /// <returns></returns>
//    public async Task Handle(EventNotification<NotifySnackBarEvent> notification, CancellationToken cancellationToken)
//    {
//        //var state = await AuthState;
//        //if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
//        //{
//        //    await HubClient.StartAsync();
//        //    User = state.User;
//        //    StateHasChanged();
//        //}
//        //var aa = 1;
//        //await _hub.SendNotification("pinozzo");
//        var evt = notification.Event;

//        Snackbar.Add(evt.Message, evt.Severity);
//        await Task.CompletedTask;
//    }
//}