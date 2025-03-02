using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using ArchipelagoDebugClient.ViewModels;
using ArchipelagoDebugClient.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ArchipelagoDebugClient;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        SystemTextJsonSuspensionDriver<PersistentAppSettings> suspensionDriver = new("settings.json",
            AppSettingsSerializationContext.Default.PersistentAppSettings);
        AutoSuspendHelper suspension = new(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new PersistentAppSettings();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(suspensionDriver);
        suspension.OnFrameworkInitializationCompleted();

        PersistentAppSettings state = RxApp.SuspensionHost.GetAppState<PersistentAppSettings>();

        ServiceCollection services = new();
        services.AddSingleton(state);

        services.AddSingleton<SessionProvider>();
        services.AddTransient<MessageLogViewModel>();
        services.AddTransient<LocationsViewModel>();
        services.AddTransient<DeathLinkViewModel>();
        services.AddTransient<GiftingViewModel>();
        services.AddTransient<DataStorageViewModel>();
        services.AddTransient<SlotDataViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<MainViewModel>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        MainViewModel vm = serviceProvider.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = vm
            };
        }

        SettingsViewModel settings = serviceProvider.GetRequiredService<SettingsViewModel>();
        this[!RequestedThemeVariantProperty] = settings.WhenAnyValue(x => x.ThemeVariant).ToBinding();

        base.OnFrameworkInitializationCompleted();
    }
}
