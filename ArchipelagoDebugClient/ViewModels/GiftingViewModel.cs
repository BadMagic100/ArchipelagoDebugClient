using Archipelago.Gifting.Net.Traits;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

    private ObservableCollection<string> messages = [];
    public ObservableCollection<string> Messages => messages;

    private ObservableCollection<ObservableTrait> currentTraits = [];
    public ObservableCollection<ObservableTrait> CurrentTraits => currentTraits;

    private string targetName = "";
    public string TargetName
    {
        get => targetName;
        set => this.RaiseAndSetIfChanged(ref targetName, value);
    }
}
