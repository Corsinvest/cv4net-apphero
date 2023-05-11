/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Notification;

public interface INotificationChannelsOptions
{
    [Display(Name = "Notification Channels")]
    IEnumerable<string> NotificationChannels { get; set; }
}