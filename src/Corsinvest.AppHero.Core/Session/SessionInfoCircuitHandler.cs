/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using Wangkanai.Detection.Services;

namespace Corsinvest.AppHero.Core.Session;

public class SessionInfoCircuitHandler : CircuitHandler, ISessionsInfoTracker
{
    private readonly ConcurrentDictionary<string, SessionInfo> sessions = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public event EventHandler OnChanged = default!;

    public IEnumerable<SessionInfo> Sessions => sessions.Values;

    public SessionInfoCircuitHandler(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var info = GetInfo();
        sessions.TryRemove(info.HttpConnectionId, out var _);
        sessions.TryAdd(info.HttpConnectionId, info);
        OnChanged?.Invoke(this, EventArgs.Empty);

        return base.OnConnectionUpAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var info = GetInfo();
        if (info.HttpConnectionId != null)
        {
            sessions.TryRemove(info.HttpConnectionId, out var _);
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        return base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    public void SetHubConnectionId(string hubConnectionId)
    {
        var httpContextAccessor = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        if (httpContextAccessor.HttpContext != null)
        {
            if (sessions.TryGetValue(httpContextAccessor.HttpContext.Connection.Id, out var session))
            {
                session.HubConnectionId = hubConnectionId;
                OnChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private SessionInfo GetInfo()
    {
        var detectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDetectionService>();
        var currentUserService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ICurrentUserService>();
        return new SessionInfo
        {
            HttpConnectionId = currentUserService.HttpConnectionId,
            UserName = currentUserService.UserName,
            IpAddress = currentUserService.IpAddress,
            Browser = detectionService.Browser.Name.ToString(),
            BrowserVersion = detectionService.Browser.Version,
            Platform = detectionService.Platform.Name.ToString(),
            PlatformVersion = detectionService.Platform.Version,
            Device = detectionService.Device.Type.ToString(),
            UserAgent = detectionService.UserAgent.ToString(),
            Login = DateTime.Now,
        };
    }
}