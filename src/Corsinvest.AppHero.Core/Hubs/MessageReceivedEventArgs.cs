/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Hubs;

public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(string userId, string message)
    {
        UserId = userId;
        Message = message;
    }

    public string UserId { get; }
    public string Message { get; }
}
