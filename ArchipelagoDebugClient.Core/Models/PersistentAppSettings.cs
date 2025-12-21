using System.Text.Json.Serialization;

namespace ArchipelagoDebugClient.Models;

public class PersistentAppSettings
{
    public Theme Theme { get; set; } = Theme.System;
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [typeof(JsonStringEnumConverter<Theme>)]
)]
[JsonSerializable(typeof(PersistentAppSettings))]
internal partial class AppSettingsSerializationContext : JsonSerializerContext { }