namespace ArchipelagoDebugClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MessageLogViewModel MessageLog { get; } = new();
    public DeathLinkViewModel DeathLink { get; } = new();
    public GiftingViewModel Gifting { get; } = new();
    public DataStorageViewModel DataStorage { get; } = new();
    public SlotDataViewModel SlotData { get; } = new();
}
