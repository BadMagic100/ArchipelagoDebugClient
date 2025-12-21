using Archipelago.MultiClient.Net;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;
public class SlotDataViewModel : ViewModelBase
{
    public ObservableCollection<ObjectHierarchy> SlotDataFields { get; } = [];

    public HierarchicalTreeDataGridSource<ObjectHierarchy> HierarchySource { get; }

    public SlotDataViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        HierarchySource = new HierarchicalTreeDataGridSource<ObjectHierarchy>(SlotDataFields)
        {
            Columns = 
            {
                new HierarchicalExpanderColumn<ObjectHierarchy>(
                    new TextColumn<ObjectHierarchy, string>("Name", x => x.Name, width: GridLength.Star), x => x.Children),
                new TextColumn<ObjectHierarchy, string>("Type", x => x.Type),
                new TextColumn<ObjectHierarchy, object?>("Value", x => x.Value)
            }
        };

        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private async void OnSessionChanged(ArchipelagoSession? session)
    {
        if (session != null)
        {
            JObject slotData = await session.DataStorage.GetSlotDataAsync<JObject>();
            SlotDataFields.AddRange(ObjectHierarchy.GetHierarchyLists(slotData));
        }
        else
        {
            SlotDataFields.Clear();
        }
    }
}
