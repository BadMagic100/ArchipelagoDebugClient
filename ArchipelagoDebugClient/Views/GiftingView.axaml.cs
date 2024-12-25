using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using ArchipelagoDebugClient.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoDebugClient.Views;

public partial class GiftingView : UserControl
{
    public GiftingView()
    {
        InitializeComponent();
    }

    private void OnAddTraitClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is GiftingViewModel vm)
        {
            vm.CurrentTraits.Add(new ObservableTrait("", 1, 1));
        }
    }

    private async void OnSendGiftClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is GiftingViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.TargetName))
            {
                vm.Messages.Add("Failed to send: No target provided");
                return;
            }
            ArchipelagoSession session = MainView.session!;
            PlayerInfo? reciever = session.Players.Players[session.Players.ActivePlayer.Team].FirstOrDefault(p => p.Name == vm.TargetName);

            List<ObservableTrait> submittedTraits = vm.CurrentTraits.Where(t => !string.IsNullOrWhiteSpace(t.Trait)).ToList();
            GiftTrait[] converted = submittedTraits.Select(t => t.ToGiftTrait()).ToArray();
            GiftingService giftingService = new(session);

            if (await giftingService.SendGiftAsync(new GiftItem("Custom Gift", 1, 1), converted, 
                vm.TargetName, session.Players.ActivePlayer.Team))
            {
                vm.Messages.Add($"Successfully sent out the gift with traits [{string.Join(", ", submittedTraits)}]");
            }
            else
            {
                vm.Messages.Add($"Failed to send: Target {vm.TargetName} was not found or cannot accept the gift");
                return;
            }
        }
    }
    
    private void OnRemoveTraitClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ObservableTrait trait)
        {
            if (btn.FindLogicalAncestorOfType<GiftingView>()?.DataContext is GiftingViewModel vm)
            {
                vm.CurrentTraits.Remove(trait);
            }
        }
    }
}