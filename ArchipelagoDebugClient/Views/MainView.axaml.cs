using Archipelago.MultiClient.Net;
using Avalonia.Controls;

namespace ArchipelagoDebugClient.Views;

public partial class MainView : UserControl
{
    public static ArchipelagoSession? session;

    public MainView()
    {
        InitializeComponent();
    }
}
