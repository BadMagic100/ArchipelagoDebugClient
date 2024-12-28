using Archipelago.MultiClient.Net;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;

public class MessageLogViewModel : ViewModelBase
{
    private ObservableCollection<BindableMessage> messages = [];

    public ObservableCollection<BindableMessage> Messages => messages;

    public MessageLogViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private void OnSessionChanged(ArchipelagoSession? obj)
    {
        
    }
}
