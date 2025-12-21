using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Threading;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;

public class MessageLogViewModel : ViewModelBase
{
    public ObservableCollection<BindableMessage> Messages { get; } = [];

    public MessageLogViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private void OnSessionChanged(ArchipelagoSession? session)
    {
        if (session != null)
        {
            session.MessageLog.OnMessageReceived += OnMessageRecieved;
        }
        else
        {
            Messages.Clear();
        }
    }

    private void OnMessageRecieved(LogMessage message)
    {
        Dispatcher.UIThread.Invoke(() => Messages.Add(new BindableMessage(message)));
    }
}
