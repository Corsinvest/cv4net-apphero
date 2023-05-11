/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.UI;

namespace Corsinvest.AppHero.AppBss.Components;

public class SelectTest : IUIAppBarItem
{
    public Type Render { get; } = typeof(SelectTestRender);
}