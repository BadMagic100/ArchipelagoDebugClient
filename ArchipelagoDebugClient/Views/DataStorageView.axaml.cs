using Archipelago.MultiClient.Net.Helpers;
using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoDebugClient.Views;

public partial class DataStorageView : UserControl
{
    public DataStorageView()
    {
        InitializeComponent();
    }

    private void OnWatchClicked(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(KeyField.Text))
        {
            return;
        }
        
        if (DataContext is DataStorageViewModel vm && !vm.WatchedHierarchies.Any(h => h.Name == KeyField.Text))
        {
            JToken response = MainView.session!.DataStorage[KeyField.Text].To<JToken>();
            UpdateViewModel(KeyField.Text, response);
            MainView.session!.DataStorage[KeyField.Text].OnValueChanged += WatchKey(KeyField.Text);
        }
    }

    private DataStorageHelper.DataStorageUpdatedHandler WatchKey(string key)
    {
        return (oldValue, newValue, args) => OnWatchedKeyChanged(key, oldValue, newValue, args);
    }

    private void OnWatchedKeyChanged(string key, JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        UpdateViewModel(key, newValue);
    }

    private void UpdateViewModel(string key, JToken token)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (DataContext is DataStorageViewModel vm)
            {
                ObjectHierarchy? current = vm.WatchedHierarchies.FirstOrDefault(x => x.Name == key);
                ObjectHierarchy next = new(key, token);
                if (current == null)
                {
                    vm.WatchedHierarchies.Add(next);
                }
                else
                {
                    int index = vm.WatchedHierarchies.IndexOf(current);
                    vm.WatchedHierarchies.RemoveAt(index);
                    vm.WatchedHierarchies.Insert(index, next);
                }
            }
        });
    }
}