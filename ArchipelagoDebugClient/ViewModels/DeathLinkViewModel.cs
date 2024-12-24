using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;

public class DeathLinkViewModel : ViewModelBase
{
    private ObservableCollection<string> messages = [];

    public ObservableCollection<string> Messages => messages;
}
