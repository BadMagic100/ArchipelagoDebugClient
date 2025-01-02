using ArchipelagoDebugClient.Services;
using ArchipelagoDebugClient.ViewModels;
using ArchipelagoDebugClient.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace ArchipelagoDebugClient;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceCollection services = new();
        services.AddSingleton<SessionProvider>();
        services.AddTransient<MessageLogViewModel>();
        services.AddTransient<LocationsViewModel>();
        services.AddTransient<DeathLinkViewModel>();
        services.AddTransient<GiftingViewModel>();
        services.AddTransient<DataStorageViewModel>();
        services.AddTransient<SlotDataViewModel>();
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

        base.OnFrameworkInitializationCompleted();
    }
}
