/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Session;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Corsinvest.AppHero.Core.Hubs;

public class HubClient : IAsyncDisposable
{
    private HubConnection _hubConnection = default!;
    private bool _started = false;
    private readonly ISessionsInfoTracker _sessionsInfoTracker;
    private readonly ICurrentUserService _currentUserService;
    private readonly NavigationManager _navigationManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<HubClient> _logger;

    public event EventHandler<string>? LoggedIn;
    public event EventHandler<string>? LoggedOut;
    public event EventHandler<string>? NotificationReceived;
    public event MessageReceivedEventHandler? MessageReceived;
    public event EventHandler<string>? JobStarted;
    public event EventHandler<string>? JobCompleted;
    public delegate Task MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public HubClient(NavigationManager navigationManager,
                     ICurrentUserService currentUserService,
                     IAuthenticationService authenticationService,
                     ISessionsInfoTracker sessionsInfoTracker,
                     ILogger<HubClient> logger)
    {
        _sessionsInfoTracker = sessionsInfoTracker;
        _currentUserService = currentUserService;
        _navigationManager = navigationManager;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        if (!_started)
        {
            _logger.LogInformation("HubUrl: {0}", _navigationManager.ToAbsoluteUri(SignalRConstants.HubUrl));

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri(SignalRConstants.HubUrl), config =>
                {
                    config.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    };
                })
                .Build();

            _hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
            _hubConnection.On(SignalRConstants.ForceLogout, _authenticationService.Logout);
            _hubConnection.On<string>(SignalRConstants.ConnectUser, (userId) =>
            {
                _sessionsInfoTracker.SetHubConnectionId(_hubConnection.ConnectionId!);
                LoggedIn?.Invoke(this, userId);
            });

            _hubConnection.On<string, string>(SignalRConstants.DisconnectUser, (connectionID, userId) =>
            {
                if (connectionID == _hubConnection.ConnectionId)
                {
                    _sessionsInfoTracker.SetHubConnectionId(string.Empty);
                }

                LoggedOut?.Invoke(this, userId);
            });

            _hubConnection.On<string>(SignalRConstants.SendNotification, (message) => NotificationReceived?.Invoke(this, message));

            _hubConnection.On<string, string>(SignalRConstants.SendMessage,
                                              (userId, message) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(userId, message))!);

            _hubConnection.On<string>(SignalRConstants.JobCompleted, (message) => JobCompleted?.Invoke(this, message));
            _hubConnection.On<string>(SignalRConstants.JobStart, (message) => JobStarted?.Invoke(this, message));

            // start the connection
            await _hubConnection.StartAsync();

            // register user on hub to let other clients know they've joined
            await _hubConnection.SendAsync(SignalRConstants.ConnectUser, _currentUserService.UserId);
            _started = true;
        }
    }

    public async Task StopAsync()
    {
        if (_started)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null!;
            _started = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        GC.SuppressFinalize(this);
    }

    //public async Task SendAsync(string message)
    //{
    //    if (!_started) { throw new InvalidOperationException("Client not started"); }
    //    await _hubConnection.SendAsync(SignalRConstants.SendMessage, _userId, message);
    //}

    //public async Task NotifyAsync(string message)
    //{
    //    if (!_started) { throw new InvalidOperationException("Client not started"); }
    //    await _hubConnection.SendAsync(SignalRConstants.SendNotification, message);
    //}
}