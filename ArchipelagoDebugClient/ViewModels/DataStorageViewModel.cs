using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Threading;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace ArchipelagoDebugClient.ViewModels;

public class DataStorageViewModel : ViewModelBase
{
    public ObservableCollection<ObjectHierarchy> WatchedHierarchies { get; } = [];

    public HierarchicalTreeDataGridSource<ObjectHierarchy> HierarchySource { get; }

    private string _key = "";
    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    public ReactiveCommand<Unit, Unit> WatchKeyCommand { get; }

    public DataStorageViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        HierarchySource = new HierarchicalTreeDataGridSource<ObjectHierarchy>(WatchedHierarchies)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<ObjectHierarchy>(
                    new TextColumn<ObjectHierarchy, string>("Name", x => x.Name, width: GridLength.Star), x => x.Children),
                new TextColumn<ObjectHierarchy, string>("Type", x => x.Type),
                new TextColumn<ObjectHierarchy, object?>("Value", x => x.Value)
            }
        };

        WatchKeyCommand = ReactiveCommand.CreateFromTask(WatchCurrentKey,
            this.WhenAnyValue(x => x.Session, x => x.Key,
                (session, key) => session != null && !string.IsNullOrWhiteSpace(key)
            )
        );

        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private async Task WatchCurrentKey()
    {
        if (WatchedHierarchies.Any(h => h.Name == Key))
        {
            return;
        }

        JToken current = await Session!.DataStorage[Key].GetAsync<JToken>();
        UpdateHierarchies(Key, current);
        Session.DataStorage[Key].OnValueChanged += BuildKeyWatcher(Key);
    }

    private DataStorageHelper.DataStorageUpdatedHandler BuildKeyWatcher(string key)
    {
        return (oldValue, newValue, args) => UpdateHierarchies(key, newValue);
    }

    private void UpdateHierarchies(string key, JToken token)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            ObjectHierarchy? current = WatchedHierarchies.FirstOrDefault(x => x.Name == key);
            ObjectHierarchy next = new(key, token);
            if (current == null)
            {
                WatchedHierarchies.Add(next);
            }
            else
            {
                int index = WatchedHierarchies.IndexOf(current);
                WatchedHierarchies.RemoveAt(index);
                WatchedHierarchies.Insert(index, next);
            }
        });
    }

    private void OnSessionChanged(ArchipelagoSession? session)
    {
        if (session == null)
        {
            WatchedHierarchies.Clear();
        }
    }
}
