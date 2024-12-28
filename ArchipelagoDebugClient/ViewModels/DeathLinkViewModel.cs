using Archipelago.MultiClient.Net;
using ArchipelagoDebugClient.Services;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;

public class DeathLinkViewModel : ViewModelBase
{
    private ObservableCollection<string> messages = [];

    public ObservableCollection<string> Messages => messages;

    public DeathLinkViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private void OnSessionChanged(ArchipelagoSession? obj)
    {
        
    }
}
