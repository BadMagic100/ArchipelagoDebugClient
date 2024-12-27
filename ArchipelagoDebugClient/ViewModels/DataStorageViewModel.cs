﻿using ArchipelagoDebugClient.Models;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using System.Collections.ObjectModel;

namespace ArchipelagoDebugClient.ViewModels;

public class DataStorageViewModel : ViewModelBase
{
    ObservableCollection<ObjectHierarchy> watchedHierarchies = [];
    public ObservableCollection<ObjectHierarchy> WatchedHierarchies => watchedHierarchies;

    public HierarchicalTreeDataGridSource<ObjectHierarchy> HierarchySource { get; }

    public DataStorageViewModel()
    {
        HierarchySource = new HierarchicalTreeDataGridSource<ObjectHierarchy>(watchedHierarchies)
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
