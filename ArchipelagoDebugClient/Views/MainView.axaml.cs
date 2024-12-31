using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using ArchipelagoDebugClient.ViewModels;
using Avalonia.Controls;
using System.Linq;

namespace ArchipelagoDebugClient.Views;

public partial class MainView : UserControl
{
    public static ArchipelagoSession? session;

    public MainView()
    {
        InitializeComponent();
    }

    private void OnConnectClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AddressField.Text) || string.IsNullOrWhiteSpace(SlotField.Text))
        {
            ShowErrorMessage("Server address and slot name must not be blank");
            return;
        }
        string? password = string.IsNullOrWhiteSpace(PasswordField.Text) ? null : PasswordField.Text;

        session = ArchipelagoSessionFactory.CreateSession(AddressField.Text);
        LoginResult infoLogin = session.TryConnectAndLogin("", SlotField.Text, ItemsHandlingFlags.NoItems,
            tags: ["Tracker"], password: password, requestSlotData: false);

        if (infoLogin is LoginSuccessful success)
        {
            int playerCount = session.Players.AllPlayers.Count(p => !p.IsGroup);
            string game = session.Players.ActivePlayer.Game;
            bool isRaceMode = session.DataStorage.GetRaceMode();
            session.Socket.DisconnectAsync();
            session = null;

            // always includes player for server
            if (playerCount > 3)
            {
                ShowErrorMessage("Debug client only supports connecting to games with 2 or fewer players to prevent abuse");
                return;
            }
            if (isRaceMode)
            {
                ShowErrorMessage("Debug client cannot be used in race mode to prevent abuse");
                return;
            }

            session = ArchipelagoSessionFactory.CreateSession(AddressField.Text);
            LoginResult realLogin = session.TryConnectAndLogin(game, SlotField.Text, ItemsHandlingFlags.NoItems,
                password: password, requestSlotData: false);
            if (realLogin is LoginSuccessful loginSuccessful)
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.sessionProvider.Session = session;
                }
                DialogHost.IsOpen = false;
            }
            else if (realLogin is LoginFailure loginFailure)
            {
                session = null;
                ShowErrorForFailedLogin(loginFailure);
            }
            else
            {
                ShowErrorMessage("Unexpected login result");
            }
        }
        else if (infoLogin is LoginFailure failure)
        {
            session = null;
            ShowErrorForFailedLogin(failure);
        }
        else
        {
            ShowErrorMessage("Unexpected login result");
        }
    }

    private void ShowErrorForFailedLogin(LoginFailure failure)
    {
        string errors = string.Join(", ", failure.Errors);
        string errorCodes = string.Join(errors, failure.ErrorCodes);
        string message = $"Login failed.\nErrors were: {errors}\nError codes were: {errorCodes}";
        ShowErrorMessage(message);
    }

    private void ShowErrorMessage(string message)
    {
        ErrorText.Text = message;
        ErrorText.IsVisible = true;
    }
}
