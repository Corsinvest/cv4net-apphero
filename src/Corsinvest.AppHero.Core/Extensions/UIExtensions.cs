/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.Core.Extensions;

public static class UIExtensions
{
    public static string GetName(this UIIcon icon) => $"{nameof(UIIcon)}.{icon}";
}