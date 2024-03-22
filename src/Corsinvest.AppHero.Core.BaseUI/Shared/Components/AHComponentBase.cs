/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Helpers;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Corsinvest.AppHero.Core.BaseUI.Shared.Components;

public abstract class AHComponentBase : ComponentBase
{
    [Parameter]
    [Category("Common")]
    public string Class { get; set; } = default!;

    [Parameter]
    [Category("Common")]
    public string Style { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    [Category("Common")]
    public Dictionary<string, object> UserAttributes { get; set; } = [];

    [Inject] private IStringLocalizerFactory LocalizerFactory { get; set; } = default!;
    [Inject] private ILoggerFactory LoggerFactory { get; set; } = default!;

    private ILogger _logger = default!;
    protected ILogger Logger => _logger ??= LoggerFactory.CreateLogger(GetType());

    private IStringLocalizer _l = default!;
    protected IStringLocalizer L => _l ??= LocalizerFactory.Create(GetType());

    public static string GetDescriptionProperty<T>(Expression<Func<T, object>> expression) => ClassHelper.GetDescriptionProperty(expression);
}
