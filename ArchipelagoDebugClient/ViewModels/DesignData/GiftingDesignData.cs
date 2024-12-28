using Archipelago.Gifting.Net.Traits;
using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels.DesignData;
public class GiftingDesignData : GiftingViewModel
{
    public GiftingDesignData() : base(new SessionProvider())
    {
        Messages.Add("Sent a gift!");
        Messages.Add("Recieved a gift!");

        CurrentTraits.Add(new ObservableTrait(GiftFlag.Armor, 2, 1));
        CurrentTraits.Add(new ObservableTrait(GiftFlag.Heal, 1, 1));
    }
}
