using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Threading;
using Newtonsoft.Json;
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

    private string _editingKey = "";
    public string EditingKey
    {
        get => _editingKey;
        set => this.RaiseAndSetIfChanged(ref _editingKey, value);
    }

    private string _editingValue = "";
    public string EditingValue
    {
        get => _editingValue;
        set => this.RaiseAndSetIfChanged(ref _editingValue, value);
    }

    private bool _isEditing = false;
    public bool IsEditing
    {
        get => _isEditing;
        set => this.RaiseAndSetIfChanged(ref _isEditing, value);
    }

    private string _editorErrorMessage = "";
    public string EditorErrorMessage
    {
        get => _editorErrorMessage;
        set => this.RaiseAndSetIfChanged(ref _editorErrorMessage, value);
    }

    private bool _isEditorErrorVisible = false;
    public bool IsEditorErrorVisible
    {
        get => _isEditorErrorVisible;
        set => this.RaiseAndSetIfChanged(ref _isEditorErrorVisible, value);
    }

    public ReactiveCommand<ObjectHierarchy, Unit> OpenEditorCommand { get; }
    public ReactiveCommand<Unit, Unit> CloseEditorCommand { get; }
    public ReactiveCommand<Unit, Unit> SubmitEditCommand { get; }
    public ReactiveCommand<Unit, Unit> WatchKeyCommand { get; }

    public DataStorageViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        HierarchySource = new HierarchicalTreeDataGridSource<ObjectHierarchy>(WatchedHierarchies)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<ObjectHierarchy>(
                    new TextColumn<ObjectHierarchy, string>("Name", x => x.Name, width: GridLength.Star), x => x.Children, 
                        isExpandedSelector: x => x.Expanded),
                new TextColumn<ObjectHierarchy, string>("Type", x => x.Type),
                new TemplateColumn<ObjectHierarchy>("Value", "EditorCell")
            }
        };

        OpenEditorCommand = ReactiveCommand.CreateFromTask<ObjectHierarchy>(OpenEditor, this.WhenAnyValue(x => x.IsEditing, e => !e));
        CloseEditorCommand = ReactiveCommand.Create(CloseEditor, this.WhenAnyValue(x => x.IsEditing, e => e == true));
        SubmitEditCommand = ReactiveCommand.Create(SubmitEdit, this.WhenAnyValue(x => x.IsEditing, e => e == true));
        WatchKeyCommand = ReactiveCommand.CreateFromTask(WatchCurrentKey,
            this.WhenAnyValue(x => x.Session, x => x.Key,
                (session, key) => session != null && !string.IsNullOrWhiteSpace(key)
            )
        );

        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private async Task OpenEditor(ObjectHierarchy hierachy)
    {
        JToken token = await Session!.DataStorage[hierachy.Name].GetAsync<JToken>();
        EditingKey = hierachy.Name;
        EditingValue = token.ToString(Formatting.Indented);
        IsEditing = true;
    }

    private void CloseEditor()
    {
        IsEditing = false;
        IsEditorErrorVisible = false;
    }

    private void SubmitEdit()
    {
        try
        {
            JToken? obj = JsonConvert.DeserializeObject<JToken>(EditingValue);
            Session!.DataStorage[EditingKey] = obj;
            CloseEditor();
        }
        catch
        {
            EditorErrorMessage = "Invalid JSON";
            IsEditorErrorVisible = true;
        }
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
                next.CopyExpandedState(current);
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
