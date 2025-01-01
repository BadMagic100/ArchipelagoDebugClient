using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels.DesignData;

public class MainDesignData : MainViewModel
{
    public MainDesignData() 
        : base(new SessionProvider(), 
            new MessageLogDesignData(),
            new DeathLinkDesignData(),
            new GiftingDesignData(), 
            new DataStorageViewModel(new SessionProvider()), 
            new SlotDataDesignData())
    {
    }
}
