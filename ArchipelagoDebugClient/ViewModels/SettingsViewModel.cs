using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Styling;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ArchipelagoDebugClient.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public static IReadOnlyList<Theme> ThemeVariants { get; } = Enum.GetValues<Theme>().ToList();

    private Theme _theme;
    public Theme ThemeName
    {
        get => _theme;
        set => this.RaiseAndSetIfChanged(ref _theme, value);
    }

    private ObservableAsPropertyHelper<ThemeVariant> _themeVariant;
    public ThemeVariant ThemeVariant => _themeVariant.Value;

    public SettingsViewModel(SessionProvider sessionProvider, PersistentAppSettings settings) : base(sessionProvider)
    {
        _theme = settings.Theme;
        _themeVariant = this.WhenAnyValue(x => x.ThemeName)
            .Select(ThemeNameToThemeVariant)
            .ToProperty(this, x => x.ThemeVariant);
    }

    private ThemeVariant ThemeNameToThemeVariant(Theme theme)
    {
        return theme switch
        {
            Theme.Dark => ThemeVariant.Dark,
            Theme.Light => ThemeVariant.Light,
            Theme.System => ThemeVariant.Default,
            _ => throw new NotImplementedException()
        };
    }
}
