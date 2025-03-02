using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ArchipelagoDebugClient;

internal class SystemTextJsonSuspensionDriver<T>(string file, JsonTypeInfo<T> typeInfo) : ISuspensionDriver
{
    public IObservable<Unit> InvalidateState()
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
        return Observable.Return(Unit.Default);
    }

    public IObservable<object> LoadState()
    {
        return File.ReadAllTextAsync(file).ToObservable()
            .Select<string, object>(json => JsonSerializer.Deserialize(json, typeInfo)!);
    }

    public IObservable<Unit> SaveState(object state)
    {
        return Observable.FromAsync(async () =>
        {
            using Stream stream = File.OpenWrite(file);
            await JsonSerializer.SerializeAsync(stream, (T)state, typeInfo);
            return Unit.Default;
        });
    }
}
