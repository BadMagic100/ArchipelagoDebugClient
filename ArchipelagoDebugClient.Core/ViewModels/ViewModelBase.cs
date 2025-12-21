using Archipelago.MultiClient.Net;
using ArchipelagoDebugClient.Services;
using ReactiveUI;
using System.Reactive.Linq;

namespace ArchipelagoDebugClient.ViewModels;

public class ViewModelBase : ReactiveObject
{
    // todo - this should be protected but can't be until MainView's functionality moves to the viewmodel
    public readonly SessionProvider sessionProvider;

    private readonly ObservableAsPropertyHelper<ArchipelagoSession?> _session;
    public ArchipelagoSession? Session => _session.Value;

    private readonly ObservableAsPropertyHelper<bool> _hasSession;
    public bool HasSession => _hasSession.Value;

    public ViewModelBase(SessionProvider sessionProvider)
    {
        this.sessionProvider = sessionProvider;
        _session = Observable.Concat(
            Observable.Return(sessionProvider.Session),
            Observable.FromEvent<ArchipelagoSession?>(h => sessionProvider.OnSessionChanged += h, h => sessionProvider.OnSessionChanged -= h)
        ).DistinctUntilChanged().ToProperty(this, x => x.Session);
        _hasSession = this.WhenAnyValue(x => x.Session).Select(x => x != null)
            .ToProperty(this, x => x.HasSession);
    }
}
