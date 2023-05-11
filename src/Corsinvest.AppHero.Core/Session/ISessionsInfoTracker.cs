/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Session;

public interface ISessionsInfoTracker
{
    IEnumerable<SessionInfo> Sessions { get; }
    event EventHandler OnChanged;
    void SetHubConnectionId(string hubConnectionId);
}