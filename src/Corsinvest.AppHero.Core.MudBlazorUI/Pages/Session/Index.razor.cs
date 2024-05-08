///*
// * SPDX-FileCopyrightText: Copyright Corsinvest Srl
// * SPDX-License-Identifier: AGPL-3.0-only
// */
//using Corsinvest.AppHero.Core.Hubs;
//using Corsinvest.AppHero.Core.Session;
//using Microsoft.AspNetCore.SignalR;

//namespace Corsinvest.AppHero.Core.MudBlazorUI.Pages.Session;

//public partial class Index : IDisposable
//{
//    [Inject] private ISessionsInfoTracker SessioInfoTracker { get; set; } = default!;
//    [Inject] private IDataGridManager<SessionInfo> DataGridManager { get; set; } = default!;
//    [Inject] private IHubContext<AHHub> Hub { get; set; } = default!;

//    protected override void OnInitialized()
//    {
//        DataGridManager.Title = L["Sessions"];
//        DataGridManager.DefaultSort = new() { [nameof(SessionInfo.UserName)] = false };
//        DataGridManager.QueryAsync = async () => await Task.FromResult(SessioInfoTracker.Sessions);
//        SessioInfoTracker.OnChanged += SessioInfoTracker_SessionsChanged;
//    }

//    private async Task Logout()
//    {
//        if (await UIMessageBox.ShowQuestionAsync(L["Logout"], L["Confirm?"]))
//        {
//            foreach (var item in DataGridManager.SelectedItems.Where(a => !string.IsNullOrEmpty(a.HubConnectionId)))
//            {
//                await Hub.Clients.Client(item.HubConnectionId).SendAsync(SignalRConstants.ForceLogout);
//            }

//            UINotifier.Show(L["Session Logout!"], UINotifierSeverity.Success);
//        }
//    }

//    private void SessioInfoTracker_SessionsChanged(object? sender, EventArgs e) => InvokeAsync(DataGridManager.Refresh);
//    public void Dispose() => SessioInfoTracker.OnChanged -= SessioInfoTracker_SessionsChanged;
//}