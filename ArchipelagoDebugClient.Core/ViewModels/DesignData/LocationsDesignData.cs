using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels.DesignData;

public class LocationsDesignData : LocationsViewModel
{
    public LocationsDesignData() : base(new SessionProvider())
    {
        scoutedLocationsCache.Edit(inner =>
        {
            inner.Load([
                new ObservableScout(10, "Test Location", 5, "Test Item", "Test", "BadMagic2", false),
                new ObservableScout(11, "Test Location 2", 5, "Test Item", "Test", "BadMagic", true)
            ]);
        });
    }
}
