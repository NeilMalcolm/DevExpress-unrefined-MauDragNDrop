using Microsoft.Maui.Graphics;
using System.Windows.Input;

namespace MauDragNDrop.Functionality;

public abstract class BaseDraggableView : ContentView
{
    DragGestureRecognizer _dragGestureRecognizer;

    public static BindableProperty OnDragStartedCommandProperty = BindableProperty.Create
        (
            nameof(OnDragStartedCommand),
            typeof(ICommand),
            typeof(BaseDraggableView),
            null
        );

    public ICommand OnDragStartedCommand
    {
        get => (ICommand)GetValue(OnDragStartedCommandProperty);
        set => SetValue(OnDragStartedCommandProperty, value);
    }

    public static BindableProperty OnDragEndedCommandProperty = BindableProperty.Create
        (
            nameof(OnDragEndedCommand),
            typeof(ICommand),
            typeof(BaseDraggableView),
            null
        );

    public ICommand OnDragEndedCommand
    {
        get => (ICommand)GetValue(OnDragEndedCommandProperty);
        set => SetValue(OnDragEndedCommandProperty, value);
    }

    public BaseDraggableView()
    {
        SetGestureRecognizer();

    }

    public string DropPacketName { get; set; } = "DropPacket";

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        SetGestureRecognizer();
    }

    void SetGestureRecognizer()
    {
        ResetAndAddDragGestureRecognizer();
    }

    void ResetAndAddDragGestureRecognizer()
    {
        RemoveGestureRecognizer();

        _dragGestureRecognizer = new DragGestureRecognizer(); ;
        _dragGestureRecognizer.DragStarting += _dragGestureRecognizer_DragStarting;
        _dragGestureRecognizer.DropCompleted += _dragGestureRecognizer_DropCompleted;

        GestureRecognizers.Add(_dragGestureRecognizer);
    }

    void RemoveGestureRecognizer()
    {
        if (_dragGestureRecognizer == null)
        {
            return;
        }

        _dragGestureRecognizer.DragStarting -= _dragGestureRecognizer_DragStarting;
        _dragGestureRecognizer.DropCompleted -= _dragGestureRecognizer_DropCompleted;
        
        GestureRecognizers.Remove(_dragGestureRecognizer);
        _dragGestureRecognizer = null;
    }

    private void _dragGestureRecognizer_DropCompleted(object sender, DropCompletedEventArgs e)
    {
        OnDragEndedCommand?.Execute(BindingContext);
    }

    private void _dragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
    {
        OnDragStartedCommand?.Execute(BindingContext);
        e.Data.Properties.Add(DropPacketName, BindingContext);
    }
}