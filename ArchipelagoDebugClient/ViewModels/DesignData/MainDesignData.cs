using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels.DesignData;

public class MainDesignData : MainViewModel
{
    public MainDesignData() 
        : base(new SessionProvider(), 
            new MessageLogDesignData(),
            new LocationsDesignData(),
            new DeathLinkDesignData(),
            new GiftingDesignData(), 
            new DataStorageViewModel(new SessionProvider()), 
            new SlotDataDesignData(),
            new SettingsViewModel(new SessionProvider(), new PersistentAppSettings()))
    {
    }
}
