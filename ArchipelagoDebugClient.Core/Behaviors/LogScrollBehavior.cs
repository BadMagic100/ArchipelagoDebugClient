using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace ArchipelagoDebugClient.Behaviors;

public class LogScrollBehavior : Behavior<ScrollViewer>
{
    private bool shouldScroll;
    private bool scrolling;

    protected override void OnAttached()
    {
        base.OnAttached();

        scrolling = false;
        SetScrollRequired();
        AssociatedObject!.ScrollChanged += ScrollChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.ScrollChanged -= ScrollChanged;
        
        base.OnDetaching();
    }

    private void ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (scrolling)
        {
            return;
        }

        if (e.ExtentDelta.Y != 0 || e.ViewportDelta.Y != 0)
        {
            // these events are caused by a layout update so we need to (possibly) scroll
            if (shouldScroll)
            {
                scrolling = true;
                AssociatedObject!.ScrollToEnd();
                scrolling = false;
            }
        }
        else
        {
            // user scrolled, determine & store if we are at the bottom
            SetScrollRequired();
        }
    }

    private void SetScrollRequired()
    {
        shouldScroll = AssociatedObject!.ScrollBarMaximum.Y - AssociatedObject!.Offset.Y < 0.1;
    }
}
