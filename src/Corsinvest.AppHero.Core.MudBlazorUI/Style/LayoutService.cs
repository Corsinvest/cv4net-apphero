/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.MudBlazorUI.Style;

public class LayoutService
{
    public event EventHandler? MajorUpdateOccurred;

    private bool _isDarkModeDefaultTheme;
    private bool _systemPreferencesDarkMode;
    private readonly UIOptions _options;

    public LayoutService(IOptionsSnapshot<UIOptions> options) => _options = options.Value;

    public void Initialize(bool isDarkModeDefaultTheme)
    {
        _isDarkModeDefaultTheme = isDarkModeDefaultTheme;
        SetDarkMode();
    }

    public bool IsDarkMode { get; private set; }

    public DarkLightMode DarkLightMode
    {
        get => _options.Theme.DarkLightMode;
        set
        {
            _options.Theme.DarkLightMode = value;
            IsDarkMode = _options.Theme.DarkLightMode switch
            {
                DarkLightMode.Dark => true,
                DarkLightMode.Light => false,
                DarkLightMode.System => _systemPreferencesDarkMode = true,
                _ => throw new ArgumentOutOfRangeException(nameof(DarkLightMode)),
            };
            OnMajorUpdateOccured();
        }
    }

    public bool IsRightToLeft
    {
        get => _options.Theme.IsRightToLeft;
        set
        {
            _options.Theme.IsRightToLeft = value;
            OnMajorUpdateOccured();
        }
    }

    public string PrimaryColor
    {
        get => _options.Theme.PrimaryColor;
        set
        {
            _options.Theme.PrimaryColor = value;
            OnMajorUpdateOccured();
        }
    }

    public int DefaultBorderRadius
    {
        get => _options.Theme.DefaultBorderRadius;
        set
        {
            _options.Theme.DefaultBorderRadius = value;
            OnMajorUpdateOccured();
        }
    }

    private void SetDarkMode()
        => IsDarkMode = _options.Theme.DarkLightMode switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => _isDarkModeDefaultTheme,
            _ => throw new ArgumentOutOfRangeException(nameof(DarkLightMode)),
        };

    public MudTheme CurrentTheme => _options.Theme.Current;
    public void SystemDarkModeChanged(bool value) => IsDarkMode = value;

    public Task OnSystemPreferenceChanged(bool darkMode)
    {
        _systemPreferencesDarkMode = darkMode;
        if (DarkLightMode == DarkLightMode.System)
        {
            IsDarkMode = darkMode;
            OnMajorUpdateOccured();
        }
        return Task.CompletedTask;
    }

    private void OnMajorUpdateOccured() => MajorUpdateOccurred?.Invoke(this, EventArgs.Empty);
}
