using ArchipelagoDebugClient.Models;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;

public class MessageLogViewModel : ViewModelBase
{
    private ObservableCollection<BindableMessage> messages = [];

    public ObservableCollection<BindableMessage> Messages => messages;
}
