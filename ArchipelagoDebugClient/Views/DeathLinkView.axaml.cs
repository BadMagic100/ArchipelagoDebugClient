using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using ArchipelagoDebugClient.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace ArchipelagoDebugClient.Views;

public partial class DeathLinkView : UserControl
{
    private DeathLinkService? service = null;

    public DeathLinkView()
    {
        InitializeComponent();
    }

    private void OnDeathlinkEnabled(object? sender, RoutedEventArgs e)
    {
        service ??= MainView.session.CreateDeathLinkService();
        service.OnDeathLinkReceived += OnReceiveDeathLink;
        service?.EnableDeathLink();
    }

    private void OnDeathlinkDisabled(object? sender, RoutedEventArgs e)
    {
        if (service != null)
        {
            service.DisableDeathLink();
            service.OnDeathLinkReceived -= OnReceiveDeathLink;
        }
    }

    private void OnDeathlinkSendClicked(object? sender, RoutedEventArgs e)
    {
        string? cause = string.IsNullOrEmpty(ReasonField.Text) ? null : ReasonField.Text;
        DeathLink deathLink = new(MainView.session!.Players.ActivePlayer.Alias, cause);
        if (DataContext is DeathLinkViewModel vm)
        {
            vm.Messages.Add($"[SEND] {GetMessage(deathLink)}");
        }
        service?.SendDeathLink(deathLink);
    }

    private void OnReceiveDeathLink(DeathLink deathLink)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (DataContext is DeathLinkViewModel vm)
            {
                vm.Messages.Add($"[RECV] {GetMessage(deathLink)}");
            }
        });
    }

    private string GetMessage(DeathLink deathLink)
    {
        return $"Source: {deathLink.Source} Cause: {deathLink.Cause} Time: {deathLink.Timestamp}";
    }
}