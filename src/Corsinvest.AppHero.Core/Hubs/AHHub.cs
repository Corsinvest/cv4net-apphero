/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Corsinvest.AppHero.Core.Hubs;

public class AHHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _onlineUsers = new();

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_onlineUsers.TryRemove(Context.ConnectionId, out var _))
        {
            await Clients.All.SendAsync(SignalRConstants.DisconnectUser, Context.User?.Identity?.Name);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        if (!_onlineUsers.ContainsKey(Context.ConnectionId))
        {
            await Clients.Client(Context.ConnectionId).SendAsync(SignalRConstants.ConnectUser, Context.User!.Identity!.Name);
        }
        await base.OnConnectedAsync();
    }
}