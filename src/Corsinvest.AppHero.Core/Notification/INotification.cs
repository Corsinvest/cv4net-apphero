/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Notification;

public interface INotification : IGroupableService
{
    string IGroupableService.GetGroupName() => "Notification";
    string IGroupableService.GetGroupIcon() => UI.UIIcon.Telegram.GetName();
}