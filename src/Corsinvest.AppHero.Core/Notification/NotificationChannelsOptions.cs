/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Notification;

public class NotificationChannelsOptions<T> where T : NotificationChannelOptions
{
    public List<T> Channels { get; } = [];
}