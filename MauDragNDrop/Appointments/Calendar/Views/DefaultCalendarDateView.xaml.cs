using CommunityToolkit.Mvvm.Messaging;
using DevExpress.Maui.Scheduler;
using MauDragNDrop.Functionality;

namespace MauDragNDrop.Calendar.Views;

public partial class DefaultCalendarDateView : BaseDropTarget
{
    private CellViewModel ViewModel => BindingContext is CellViewModel viewModel ? viewModel : null;

    private Color DefaultBackgroundColor = Colors.White;
    private static readonly Color DefaultHighlightColor = new (230, 215, 255);
    private static readonly Color DragInProgressColor = new (216, 195, 250);

    public DefaultCalendarDateView()
	{
		InitializeComponent();

        WeakReferenceMessenger.Default.Register<AppointmentDragStartedMessage>(this, (recipient, message) =>
        {
            BackgroundColor = DragInProgressColor;
        });

        WeakReferenceMessenger.Default.Register<AppointmentDragEndedMessage>(this, (recipient, message) =>
        {
            BackgroundColor = DefaultBackgroundColor;
        });
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        DayLabel.BackgroundColor = ViewModel?.IsToday ?? false ? Colors.Purple : Colors.Transparent;
        DayLabel.TextColor = ViewModel?.IsToday ?? false ? Colors.White : null;
        BackgroundColor = DefaultBackgroundColor;
    }

    protected override void OnDragEnd()
    {
        // on drag finished
        BackgroundColor = DragInProgressColor;
        DayLabel.FontSize = 14;
        DayLabel.TextColor = ViewModel?.IsToday ?? false ? Colors.White : null;
    }

    protected override void OnDragLeave()
    {
        // on drag left
        BackgroundColor = DragInProgressColor;
        DaySpan.FontSize = 14;
        DayLabel.TextColor = ViewModel?.IsToday ?? false ? Colors.White : null;
    }

    protected override void OnDragOver()
    {
        // on drag over
        BackgroundColor = DefaultHighlightColor;
        DaySpan.FontSize = 20; 
        DayLabel.TextColor = ViewModel?.IsToday ?? false ? Colors.White : null;
    }

    protected override void OnDragStart()
    {
        // on drag started
        BackgroundColor = Colors.Silver;
        DaySpan.FontSize = 20;
        DayLabel.TextColor = ViewModel?.IsToday ?? false ? Colors.White : null;
    }

    protected override void OnDrop(object dropPacket)
    {
        // dropped upon with the packet
        BackgroundColor = DefaultBackgroundColor;
        DayLabel.TextColor = ViewModel?.IsToday ?? false ? Colors.White : null;
    }
}