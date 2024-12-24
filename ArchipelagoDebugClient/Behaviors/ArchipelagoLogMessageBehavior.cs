using ArchipelagoDebugClient.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using System.Linq;

namespace ArchipelagoDebugClient.Behaviors;

public class ArchipelagoLogMessageBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<BindableMessage> MessageProperty
        = AvaloniaProperty.RegisterAttached<ArchipelagoLogMessageBehavior, TextBlock, BindableMessage>("Message");

    static ArchipelagoLogMessageBehavior()
    {
        MessageProperty.Changed.AddClassHandler<TextBlock, BindableMessage>(HandleMessageChanged);
    }

    private static void HandleMessageChanged(TextBlock target, AvaloniaPropertyChangedEventArgs<BindableMessage> e) 
    {
        target.Inlines?.Clear();
        if (!e.NewValue.HasValue || e.NewValue.Value == null)
        {
            return;
        }
        target.Inlines?.AddRange(e.NewValue.Value.Parts.Select(p =>
        {
            IBrush foregroundBrush;
            IBrush? backgroundBrush;
            if (p.IsBackgroundColor)
            {
                foregroundBrush = new SolidColorBrush(Colors.White);
                backgroundBrush = new SolidColorBrush(p.Color);
            }
            else
            {
                foregroundBrush = new SolidColorBrush(p.Color);
                backgroundBrush = null;
            }

            return new Run()
            {
                Text = p.Text,
                Foreground = foregroundBrush,
                Background = backgroundBrush
            };
        }));
    }

    public static void SetMessage(AvaloniaObject element, BindableMessage message)
    {
        element.SetValue(MessageProperty, message);
    }

    public static BindableMessage GetMessage(AvaloniaObject element)
    {
        return element.GetValue(MessageProperty);
    }

}
