using ArchipelagoDebugClient.Models;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;
public class SlotDataViewModel : ViewModelBase
{
    ObservableCollection<ObjectHierarchy> slotDataFields = [];
    public ObservableCollection<ObjectHierarchy> SlotDataFields => slotDataFields;

    public HierarchicalTreeDataGridSource<ObjectHierarchy> HierarchySource { get; }

    public SlotDataViewModel()
    {
        HierarchySource = new HierarchicalTreeDataGridSource<ObjectHierarchy>(slotDataFields)
        {
            Columns = 
            {
                new HierarchicalExpanderColumn<ObjectHierarchy>(
                    new TextColumn<ObjectHierarchy, string>("Name", x => x.Name, width: GridLength.Star), x => x.Children),
                new TextColumn<ObjectHierarchy, string>("Type", x => x.Type),
                new TextColumn<ObjectHierarchy, object?>("Value", x => x.Value)
            }
        };
    }
}
