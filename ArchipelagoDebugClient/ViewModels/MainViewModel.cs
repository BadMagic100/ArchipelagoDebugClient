﻿using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using ArchipelagoDebugClient.Services;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ArchipelagoDebugClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MessageLogViewModel MessageLog { get; }
    public DeathLinkViewModel DeathLink { get; }
    public GiftingViewModel Gifting { get; }
    public DataStorageViewModel DataStorage { get; }
    public SlotDataViewModel SlotData { get; }

    private string _address = "";
    public string Address
    {
        get => _address;
        set => this.RaiseAndSetIfChanged(ref _address, value);
    }

    private string _slot = "";
    public string Slot
    {
        get => _slot;
        set => this.RaiseAndSetIfChanged(ref _slot, value);
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private ObservableAsPropertyHelper<bool> _isErrorVisible;
    public bool IsErrorVisible => _isErrorVisible.Value;

    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }

    public MainViewModel(
        SessionProvider sessionProvider,
        MessageLogViewModel messageLog,
        DeathLinkViewModel deathLink,
        GiftingViewModel gifting,
        DataStorageViewModel dataStorage,
        SlotDataViewModel slotData) : base(sessionProvider)
    {
        MessageLog = messageLog;
        DeathLink = deathLink;
        Gifting = gifting;
        DataStorage = dataStorage;
        SlotData = slotData;

        _isErrorVisible = this.WhenAnyValue(x => x.ErrorMessage)
            .Select(x => !string.IsNullOrWhiteSpace(x))
            .ToProperty(this, x => x.IsErrorVisible);

        IObservable<bool> connectCanRun = this.WhenAnyValue(x => x.Address, x => x.Slot,
            (address, slot) => !string.IsNullOrWhiteSpace(address) && !string.IsNullOrWhiteSpace(slot));
        ConnectCommand = ReactiveCommand.CreateFromTask(ConnectAsync, connectCanRun);
    }

    private async Task ConnectAsync()
    {
        string? password = string.IsNullOrWhiteSpace(Password) ? null : Password;

        ArchipelagoSession session = ArchipelagoSessionFactory.CreateSession(Address);

        LoginResult infoLogin = await TryConnectAndLoginAsync(session, "", Slot, password, ["Tracker"]);

        if (infoLogin is LoginSuccessful success)
        {
            int playerCount = session.Players.AllPlayers.Count(p => !p.IsGroup);
            string game = session.Players.ActivePlayer.Game;
            bool isRaceMode = session.DataStorage.GetRaceMode();
            await session.Socket.DisconnectAsync();

            // always includes player for server
            if (playerCount > 3)
            {
                ErrorMessage = "Debug client only supports connecting to games with 2 or fewer players to prevent abuse";
                return;
            }
            if (isRaceMode)
            {
                ErrorMessage = "Debug client cannot be used in race mode to prevent abuse";
                return;
            }

            session = ArchipelagoSessionFactory.CreateSession(Address);
            LoginResult realLogin = await TryConnectAndLoginAsync(session, game, Slot, password);
            if (realLogin is LoginSuccessful loginSuccessful)
            {
                sessionProvider.Session = session;
            }
            else if (realLogin is LoginFailure loginFailure)
            {
                ErrorMessage = BuildErrorForFailedLogin(loginFailure);
            }
            else
            {
                ErrorMessage = "Unexpected login result";
            }
        }
        else if (infoLogin is LoginFailure failure)
        {
            ErrorMessage = BuildErrorForFailedLogin(failure);
        }
        else
        {
            ErrorMessage = "Unexpected login result";
        }
    }

    private async Task<LoginResult> TryConnectAndLoginAsync(ArchipelagoSession session,
        string game, string name, string? password, string[]? tags = null)
    {
        try
        {
            await session.ConnectAsync();
        }
        catch (Exception e)
        {
            return new LoginFailure($"Connection failed: {e.Message}");
        }
        return await session.LoginAsync(game, name, ItemsHandlingFlags.NoItems, 
            tags: tags, password: password, requestSlotData: false);
    }

    private string BuildErrorForFailedLogin(LoginFailure failure)
    {
        string errors = string.Join(", ", failure.Errors);
        string errorCodes = string.Join(errors, failure.ErrorCodes);
        return $"Login failed.\nErrors were: {errors}\nError codes were: {errorCodes}";
    }
}
