using Archipelago.MultiClient.Net;
using System;

namespace ArchipelagoDebugClient.Services;

public class SessionProvider
{
    public event Action<ArchipelagoSession?>? OnSessionChanged;

    private ArchipelagoSession? session;
    public ArchipelagoSession? Session
    {
        get => session;
        set
        {
            if (session != value)
            {
                session = value;
                OnSessionChanged?.Invoke(session);
            }
        }
    }
}
