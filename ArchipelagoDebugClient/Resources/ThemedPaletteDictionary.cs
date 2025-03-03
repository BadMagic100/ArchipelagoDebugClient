using Archipelago.MultiClient.Net.Colors;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace ArchipelagoDebugClient.Resources;

public class ThemedPaletteDictionary : ResourceDictionary
{
    public static Palette<Color> DarkPalette { get; } = BuiltInPalettes.Dark
        .Transform(c => new Color(0xFF, c.R, c.G, c.B));

    // colors in the light palette are verified to be at least WCAG AA for small text and WCAG AAA for large text.
    // experimentally, meeting WCAG AAA tends to bring the colors indistinguishable from black text
    public static Palette<Color> LightPalette { get; } = new Palette<Color>(Colors.Black, new()
    {
        [PaletteColor.Red] = new Color(0xFF, 0xEE, 0x00, 0x00),
        [PaletteColor.Green] = new Color(0xFF, 0x00, 0x80, 0x00),
        [PaletteColor.Blue] = new Color(0xFF, 0x00, 0x00, 0xFF),
        [PaletteColor.Cyan] = new Color(0xFF, 0x00, 0x80, 0x98),
        [PaletteColor.Magenta] = new Color(0xFF, 0xD0, 0x00, 0xD0),
        // unfortunately a pretty ugly yellow/brown but yellow is a light (low contrast) color by definition
        [PaletteColor.Yellow] = new Color(0xFF, 0x7A, 0x7A, 0x00),
        [PaletteColor.SlateBlue] = new Color(0xFF, 0x64, 0x5A, 0xFF),
        [PaletteColor.Salmon] = new Color(0xFF, 0xB3, 0x58, 0x40),
        [PaletteColor.Plum] = new Color(0xFF, 0xB1, 0x3E, 0xB1)
    });

    public ThemedPaletteDictionary()
    {
        ThemeDictionaries[ThemeVariant.Dark] = new ResourceDictionary()
        {
            ["MultiClientPalette"] = DarkPalette
        };
        ThemeDictionaries[ThemeVariant.Light] = new ResourceDictionary()
        {
            ["MultiClientPalette"] = LightPalette
        };
    }
}
