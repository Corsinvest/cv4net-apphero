/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.BaseUI.Shared.Notification;
using Corsinvest.AppHero.Core.Notification;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Notification;

public partial class NotificationChannelsOptionsGrid<T> : NotificationChannelsOptionsGridBase<T>
    where T : NotificationChannelOptions, new()
{ }