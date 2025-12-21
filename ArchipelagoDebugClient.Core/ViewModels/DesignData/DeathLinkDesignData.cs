using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels.DesignData;
public class DeathLinkDesignData : DeathLinkViewModel
{
    public DeathLinkDesignData() : base(new SessionProvider())
    {
        Messages.Add("[SEND] BadMagic died quickly and painlessly at 1735018127.3916597");
        Messages.Add("[RECV] BadMagic2 died at 1735018127.3916597");
    }
}
