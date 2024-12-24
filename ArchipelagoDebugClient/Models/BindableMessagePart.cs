using Archipelago.MultiClient.Net.MessageLog.Parts;
using Avalonia.Media;

namespace ArchipelagoDebugClient.Models;

public class BindableMessagePart
{
    public string Text { get; init; }
    public Color Color { get; }
    public bool IsBackgroundColor { get; }
    public MessagePartType Type { get; }

    public BindableMessagePart(string text, Color color, bool isBackgroundColor = false, MessagePartType type = MessagePartType.Text)
    {
        Text = text;
        Color = color;
        IsBackgroundColor = isBackgroundColor;
        Type = type;
    }

    public BindableMessagePart(MessagePart part)
    {
        Text = part.Text;
        Color = new Color(255, part.Color.R, part.Color.G, part.Color.B);
        IsBackgroundColor = part.IsBackgroundColor;
        Type = part.Type;
    }
}
