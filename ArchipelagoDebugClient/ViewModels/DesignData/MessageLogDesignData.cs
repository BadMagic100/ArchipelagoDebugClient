using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Media;

namespace ArchipelagoDebugClient.ViewModels.DesignData;

public class MessageLogDesignData : MessageLogViewModel
{
    public MessageLogDesignData() : base(new SessionProvider())
    {
        Messages.Add(new BindableMessage([
            new BindableMessagePart("BadMagic (Team #1) playing Hollow Knight has joined. Client(0.4.0), [].", Colors.White)
        ]));
        Messages.Add(new BindableMessage([
            new BindableMessagePart("Now that you are connected, you can use !help to list cmmands to run via the server.", Colors.White)
        ]));
        Messages.Add(new BindableMessage([
            new BindableMessagePart("BadMagic2", Colors.White),
            new BindableMessagePart(" sent ", Colors.White),
            new BindableMessagePart("Mantis_Claw", Colors.Plum),
            new BindableMessagePart(" to ", Colors.White),
            new BindableMessagePart("BadMagic", Colors.White),
            new BindableMessagePart(" (", Colors.White),
            new BindableMessagePart("Grub-Fungal_Bouncy", new Color(255, 0, 128, 0)),
            new BindableMessagePart(")", Colors.White)
        ]));
    }
}
