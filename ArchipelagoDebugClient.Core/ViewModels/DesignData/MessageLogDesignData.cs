using Archipelago.MultiClient.Net.Colors;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;

namespace ArchipelagoDebugClient.ViewModels.DesignData;

public class MessageLogDesignData : MessageLogViewModel
{
    public MessageLogDesignData() : base(new SessionProvider())
    {
        Messages.Add(new BindableMessage([
            new BindableMessagePart("BadMagic (Team #1) playing Hollow Knight has joined. Client(0.4.0), [].", null)
        ]));
        Messages.Add(new BindableMessage([
            new BindableMessagePart("Now that you are connected, you can use !help to list commands to run via the server.", null)
        ]));
        Messages.Add(new BindableMessage([
            new BindableMessagePart("BadMagic2", null),
            new BindableMessagePart(" sent ", null),
            new BindableMessagePart("Mantis_Claw", PaletteColor.Plum),
            new BindableMessagePart(" to ", null),
            new BindableMessagePart("BadMagic", null),
            new BindableMessagePart(" (", null),
            new BindableMessagePart("Grub-Fungal_Bouncy", PaletteColor.Green),
            new BindableMessagePart(")", null)
        ]));
        Messages.Add(new BindableMessage([
            new BindableMessagePart("Taste the rainbow!", null),
            new BindableMessagePart(" White", PaletteColor.White),
            new BindableMessagePart(" Black", PaletteColor.Black),
            new BindableMessagePart(" Red", PaletteColor.Red),
            new BindableMessagePart(" Salmon", PaletteColor.Salmon),
            new BindableMessagePart(" Yellow", PaletteColor.Yellow),
            new BindableMessagePart(" Green", PaletteColor.Green),
            new BindableMessagePart(" Blue", PaletteColor.Blue),
            new BindableMessagePart(" SlateBlue", PaletteColor.SlateBlue),
            new BindableMessagePart(" Cyan", PaletteColor.Cyan),
            new BindableMessagePart(" Plum", PaletteColor.Plum),
            new BindableMessagePart(" Magenta", PaletteColor.Magenta),
        ]));
    }
}
