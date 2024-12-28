using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels;

public class MainViewModel(
    SessionProvider sessionProvider,
    MessageLogViewModel messageLog,
    DeathLinkViewModel deathLink,
    GiftingViewModel gifting,
    DataStorageViewModel dataStorage,
    SlotDataViewModel slotData) : ViewModelBase(sessionProvider)
{
    public MessageLogViewModel MessageLog { get; } = messageLog;
    public DeathLinkViewModel DeathLink { get; } = deathLink;
    public GiftingViewModel Gifting { get; } = gifting;
    public DataStorageViewModel DataStorage { get; } = dataStorage;
    public SlotDataViewModel SlotData { get; } = slotData;
}
