using Archipelago.MultiClient.Net.Colors;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Resources;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using System.Linq;

namespace ArchipelagoDebugClient.Converters;

public class LogMessageConverter : AvaloniaObject, IValueConverter
{
    public static readonly DirectProperty<LogMessageConverter, Palette<Color>?> PaletteProperty =
        AvaloniaProperty.RegisterDirect<LogMessageConverter, Palette<Color>?>(
            nameof(Palette),
            o => o.Palette,
            (o, v) => o.Palette = v
        );

    private Palette<Color>? _palette;
    public Palette<Color>? Palette
    {
        get => _palette;
        set => SetAndRaise(PaletteProperty, ref _palette, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not BindableMessage message || !targetType.IsAssignableTo(typeof(InlineCollection)))
        {
            if (value is null or UnsetValueType)
            {
                return BindingOperations.DoNothing;
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

    private Inline ConvertPart(BindableMessagePart part)
    {
        Palette<Color> palette = _palette ?? ThemedPaletteDictionary.DarkPalette;

        IBrush foregroundBrush;
        IBrush? backgroundBrush;
        if (part.IsBackgroundColor)
        {
            foregroundBrush = new SolidColorBrush(palette.DefaultColor);
            backgroundBrush = new SolidColorBrush(palette[part.Color]);
        }
        else
        {
            foregroundBrush = new SolidColorBrush(palette[part.Color]);
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
