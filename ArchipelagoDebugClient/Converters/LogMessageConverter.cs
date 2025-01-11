using ArchipelagoDebugClient.Models;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using System.Linq;

namespace ArchipelagoDebugClient.Converters;

public class LogMessageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not BindableMessage message || !targetType.IsAssignableTo(typeof(InlineCollection)))
        {
            if (value is null)
            {
                return null;
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        InlineCollection inlines = [.. message.Parts.Select(ConvertPart)];
        return inlines;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static Inline ConvertPart(BindableMessagePart part)
    {
        IBrush foregroundBrush;
        IBrush? backgroundBrush;
        if (part.IsBackgroundColor)
        {
            foregroundBrush = new SolidColorBrush(Colors.White);
            backgroundBrush = new SolidColorBrush(part.Color);
        }
        else
        {
            foregroundBrush = new SolidColorBrush(part.Color);
            backgroundBrush = null;
        }
        return new Run()
        {
            Text = part.Text,
            Foreground = foregroundBrush,
            Background = backgroundBrush
        };
    }
}
