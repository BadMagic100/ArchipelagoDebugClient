using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoDebugClient.Services;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Kernel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ArchipelagoDebugClient.ViewModels;

public class ObservableScout : ReactiveObject
{
    public long LocationId { get; }
    public string LocationName { get; }
    public long ItemId { get; }
    public string ItemName { get; }
    public string ItemGame { get; }
    public string ReceivingPlayer { get; }

    private bool _isFound;
    public bool IsFound
    {
        get => _isFound;
        set => this.RaiseAndSetIfChanged(ref _isFound, value);
    }

    private bool _isSelected = false;
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public ObservableScout(long locationId, string locationName, long itemId,
        string itemName, string itemGame, 
        string receivingPlayer, bool isFound)
    {
        LocationId = locationId;
        LocationName = locationName;
        ItemId = itemId;
        ItemName = itemName;
        ItemGame = itemGame;
        ReceivingPlayer = receivingPlayer;
        _isFound = isFound;
    }

    public ObservableScout(ScoutedItemInfo item, bool isFound) : this(item.LocationId, 
        item.LocationDisplayName, 
        item.ItemId,
        item.ItemDisplayName,
        item.ItemGame,
        item.Player.Name,
        isFound)
    {
    }
}

public class LocationsViewModel : ViewModelBase
{
    protected SourceCache<ObservableScout, long> scoutedLocationsCache = new(x => x.LocationId);
    private ReadOnlyObservableCollection<ObservableScout> _scoutedLocations;
    public ReadOnlyObservableCollection<ObservableScout> ScoutedLocations => _scoutedLocations;

    public FlatTreeDataGridSource<ObservableScout> HierarchySource { get; }

    public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }
    public ReactiveCommand<Unit, Unit> SendCommand { get; }

    public LocationsViewModel(SessionProvider sessionProvider) : base(sessionProvider)
    {
        scoutedLocationsCache.Connect()
            .AutoRefresh(x => x.IsFound)
            .AutoRefresh(x => x.IsSelected)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _scoutedLocations)
            .Subscribe();

        HierarchySource = new FlatTreeDataGridSource<ObservableScout>(ScoutedLocations)
        {
            Columns =
            {
                new TemplateColumn<ObservableScout>(null, "CheckBoxCell",
                    options: new TemplateColumnOptions<ObservableScout>()
                    {
                        CanUserResizeColumn = false,
                        CanUserSortColumn = false
                    }),
                new TextColumn<ObservableScout, long>("Location ID", x => x.LocationId),
                new TextColumn<ObservableScout, string>("Location Name", x => x.LocationName, new GridLength(1.5, GridUnitType.Star)),
                new TextColumn<ObservableScout, long>("Item ID", x => x.ItemId),
                new TextColumn<ObservableScout, string>("Item Name", x => x.ItemName, new GridLength(1.5, GridUnitType.Star)),
                new TextColumn<ObservableScout, string>("Receiving Player", x => x.ReceivingPlayer, GridLength.Star),
                new TextColumn<ObservableScout, string>("Receiving Game", x => x.ItemGame, GridLength.Star),
                new TextColumn<ObservableScout, bool>("Found", x => x.IsFound),
            }
        };

        SelectAllCommand = ReactiveCommand.Create(SelectAll);

        IObservable<bool> canSend = this.WhenAnyValue(x => x.Session, (Func<ArchipelagoSession?, bool>)(session => session != null));
        SendCommand = ReactiveCommand.CreateFromTask(SendSelectedLocations, canSend);

        sessionProvider.OnSessionChanged += OnSessionChanged;
    }

    private async void OnSessionChanged(ArchipelagoSession? session)
    {
        if (session != null)
        {
            Dictionary<long, ScoutedItemInfo> scouts = await session.Locations.ScoutLocationsAsync(session.Locations.AllLocations.ToArray());
            scoutedLocationsCache.Edit(inner =>
            {
                foreach (ScoutedItemInfo item in scouts.Values)
                {
                    bool found = session.Locations.AllLocationsChecked.Contains(item.LocationId);
                    inner.AddOrUpdate(new ObservableScout(item, found));
                }
            });
            session.Locations.CheckedLocationsUpdated += UpdateCheckedLocations;
        }
        else
        {
            scoutedLocationsCache.Clear();
        }
    }

    private void UpdateCheckedLocations(ReadOnlyCollection<long> newCheckedLocations)
    {
        // even though the cache is threadsafe, the nested edit on the webhook thread is problematic
        Dispatcher.UIThread.Invoke(() =>
        {
            scoutedLocationsCache.Edit(inner =>
            {
                foreach (long loc in newCheckedLocations)
                {
                    Optional<ObservableScout> scout = inner.Lookup(loc);
                    if (scout.HasValue)
                    {
                        scout.Value.IsFound = true;
                        // uncheck it once received
                        scout.Value.IsSelected = false;
                    }
                }
            });
        });
    }

    private void SelectAll()
    {
        scoutedLocationsCache.Edit(inner =>
        {
            foreach (ObservableScout scout in inner.Items.Where(s => !s.IsFound))
            {
                scout.IsSelected = true;
            }
        });
    }

    private async Task SendSelectedLocations()
    {
        IEnumerable<long> locationsToSend = ScoutedLocations
            .Where(s => s.IsSelected)
            .Select(s => s.LocationId);
        await Session!.Locations.CompleteLocationChecksAsync(locationsToSend.ToArray());
    }
}
