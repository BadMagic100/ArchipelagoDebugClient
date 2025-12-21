using Archipelago.MultiClient.Net.Colors;
using Archipelago.MultiClient.Net.MessageLog.Parts;

namespace ArchipelagoDebugClient.Models;

public class BindableMessagePart
{
    public string Text { get; init; }
    public PaletteColor? Color { get; }
    public bool IsBackgroundColor { get; }
    public MessagePartType Type { get; }

    public BindableMessagePart(string text, PaletteColor? color, bool isBackgroundColor = false, MessagePartType type = MessagePartType.Text)
    {
        Text = text;
        Color = color;
        IsBackgroundColor = isBackgroundColor;
        Type = type;
    }

    public BindableMessagePart(MessagePart part)
    {
        Text = part.Text;
        Color = part.PaletteColor;
        IsBackgroundColor = part.IsBackgroundColor;
        Type = part.Type;
    }
}
