using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using ArchipelagoDebugClient.Services;
using ArchipelagoDebugClient.Views;
using Avalonia.Threading;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace ArchipelagoDebugClient.ViewModels;

public class DeathLinkViewModel : ViewModelBase
{
    private DeathLinkService? _deathLinkService;
    public DeathLinkService? DeathLinkService
    {
        get => _deathLinkService;
        set => this.RaiseAndSetIfChanged(ref _deathLinkService, value);
    }

    private string _cause = "";
    public string Cause
    {
        get => _cause;
        set => this.RaiseAndSetIfChanged(ref _cause, value);
    }

    private bool _deathLinkEnabled = false;
    public bool DeathLinkEnabled
    {
        get => _deathLinkEnabled;
        set
        {
            if (value)
            {
                DeathLinkService?.EnableDeathLink();
            }
            else
            {
                DeathLinkService?.DisableDeathLink();
            }
            this.RaiseAndSetIfChanged(ref _deathLinkEnabled, value);
        }
    }

    public ObservableCollection<string> Messages { get; } = [];

    public ReactiveCommand<Unit, Unit> SendDeathLinkCommand { get; }

    public DeathLinkViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        SendDeathLinkCommand = ReactiveCommand.Create(SendDeathLink, 
            this.WhenAnyValue(x => x.DeathLinkService, x => x.DeathLinkEnabled,
                (service, enabled) => service != null && enabled));

        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private void SendDeathLink()
    {
        string? cause = string.IsNullOrEmpty(Cause) ? null : Cause;
        DeathLink deathLink = new(MainView.session!.Players.ActivePlayer.Alias, cause);
        Messages.Add($"[SEND] {GetMessage(deathLink)}");
        DeathLinkService!.SendDeathLink(deathLink);
    }

    private void OnDeathLinkRecieved(DeathLink deathLink)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            Messages.Add($"[RECV] {GetMessage(deathLink)}");
        });
    }

    private string GetMessage(DeathLink deathLink)
    {
        return $"Source: {deathLink.Source} Cause: {deathLink.Cause} Time: {deathLink.Timestamp}";
    }

    private void OnSessionChanged(ArchipelagoSession? session)
    {
        if (session != null)
        {
            DeathLinkService = session.CreateDeathLinkService();
            DeathLinkService.OnDeathLinkReceived += OnDeathLinkRecieved;
        }
        else
        {
            if (DeathLinkService != null)
            {
                DeathLinkService.OnDeathLinkReceived -= OnDeathLinkRecieved;
            }
            DeathLinkService = null;
            Messages.Clear();
        }
    }
}
