/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Hubs;

public static class SignalRConstants
{
    public static string HubUrl { get; } = "/signalRHub";

    //    public static  string SendUpdateDashboard { get; } =  "UpdateDashboardAsync";
    //    public static  string ReceiveUpdateDashboard { get; } =  "UpdateDashboard";
    //    public static  string ReceiveChatNotification { get; } =  "ReceiveChatNotification";
    //    public const string OnChangeRolePermissions = "OnChangeRolePermissions";
    //    public const string LogoutUsersByRole = "LogoutUsersByRole";
    //    public const string PingRequest = "PingRequestAsync";
    //    public const string PingResponse = "PingResponseAsync";
    //    public const string UpdateOnlineUsers = "UpdateOnlineUsers";
    //    public const string OCRTaskCompleted = "OCRTaskCompleted";

    public static string SendNotification { get; } = nameof(SendNotification);
    public static string SendMessage { get; } = nameof(SendMessage);
    public static string ConnectUser { get; } = nameof(ConnectUser);
    public static string DisconnectUser { get; } = nameof(DisconnectUser);
    public static string JobCompleted { get; } = nameof(JobCompleted);
    public static string JobStart { get; } = nameof(JobStart);
    public static string ForceLogout { get; } = nameof(ForceLogout);
}
