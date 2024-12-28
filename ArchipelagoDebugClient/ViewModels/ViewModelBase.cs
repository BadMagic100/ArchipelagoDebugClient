using ArchipelagoDebugClient.Services;
using ReactiveUI;

namespace ArchipelagoDebugClient.ViewModels;

public class ViewModelBase(SessionProvider sessionProvider) : ReactiveObject
{
    // todo - this should be protected but can't be until MainView's functionality moves to the viewmodel
    public readonly SessionProvider sessionProvider = sessionProvider;
}
