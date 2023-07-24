/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Notification;

public interface INotification : IGroupableModule
{
    string IGroupableModule.GetGroupName() => "Notification";
    string IGroupableModule.GetGroupIcon() => UI.UIIcon.Telegram.GetName();
}