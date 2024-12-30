using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net;
using ArchipelagoDebugClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ArchipelagoDebugClient.ViewModels;

public class ObservableTrait : ReactiveObject, IEquatable<ObservableTrait?>
{
    private string trait;
    public string Trait
    {
        get => trait;
        set => this.RaiseAndSetIfChanged(ref trait, value);
    }

    private double quality;
    public double Quality
    {
        get => quality;
        set => this.RaiseAndSetIfChanged(ref quality, value);
    }

    private double duration;
    public double Duration
    {
        get => duration;
        set => this.RaiseAndSetIfChanged(ref duration, value);
    }

    public ObservableTrait(string trait, double quality, double duration)
    {
        this.trait = trait;
        this.quality = quality;
        this.duration = duration;
    }

    public ObservableTrait(GiftTrait source) : this(source.Trait, source.Quality, source.Duration) { }

    public GiftTrait ToGiftTrait()
    {
        return new GiftTrait(trait, duration, quality);
    }

    public override string ToString()
    {
        return $"({trait}; Q: {quality}, D: {duration})";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableTrait);
    }

    public bool Equals(ObservableTrait? other)
    {
        return other is not null &&
               trait == other.trait &&
               quality == other.quality &&
               duration == other.duration;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(trait, quality, duration);
    }
}

public class GiftingViewModel : ViewModelBase
{
    public static readonly List<string> AvailableTraits = GiftFlag.AllFlags.Order().ToList();

    public GiftingService? _giftingService;
    public GiftingService? GiftingService
    {
        get => _giftingService;
        set => this.RaiseAndSetIfChanged(ref _giftingService, value);
    }

    public ObservableCollection<string> Messages { get; } = [];

    public ObservableCollection<ObservableTrait> CurrentTraits { get; } = [];

    private string _targetName = "";
    public string TargetName
    {
        get => _targetName;
        set => this.RaiseAndSetIfChanged(ref _targetName, value);
    }

    public ReactiveCommand<Unit, Unit> AddTraitCommand { get; }
    public ReactiveCommand<ObservableTrait, Unit> RemoveTraitCommand { get; }
    public ReactiveCommand<Unit, Unit> SendGiftCommand { get; }

    public GiftingViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        AddTraitCommand = ReactiveCommand.Create(AddBlankTrait);
        RemoveTraitCommand = ReactiveCommand.Create<ObservableTrait>(RemoveTrait);
        SendGiftCommand = ReactiveCommand.CreateFromTask(SendGiftAsync,
            this.WhenAnyValue(x => x.GiftingService, x => x.TargetName,
              (service, target) => service != null && !string.IsNullOrWhiteSpace(target)));

        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private void AddBlankTrait()
    {
        CurrentTraits.Add(new ObservableTrait("", 1, 1));
    }

    private void RemoveTrait(ObservableTrait trait)
    {
        CurrentTraits.Remove(trait);
    }

    private async Task SendGiftAsync()
    {
        List<ObservableTrait> submittedTraits = CurrentTraits.Where(t => !string.IsNullOrWhiteSpace(t.Trait)).ToList();
        GiftTrait[] converted = submittedTraits.Select(t => t.ToGiftTrait()).ToArray();

        if (await GiftingService!.SendGiftAsync(new GiftItem("Custom Gift", 1, 1), converted,
            TargetName, sessionProvider.Session!.Players.ActivePlayer.Team))
        {
            Messages.Add($"Successfully sent out the gift with traits [{string.Join(", ", submittedTraits)}]");
        }
        else
        {
            Messages.Add($"Failed to send: Target {TargetName} was not found or cannot accept the gift");
            return;
        }
    }

    private void OnSessionChanged(ArchipelagoSession? session)
    {
        if (session != null)
        {
            GiftingService = new GiftingService(session);
        }
        else
        {
            GiftingService = null;
        }
    }
}
