using DevExpress.Maui.Scheduler;
using System.Windows.Input;

namespace MauDragNDrop.Functionality;

public abstract class BaseDropTarget : ContentView
{
    private DropGestureRecognizer _dropGestureRecognizer;

    public string DropPacketName { get; set; } = "DropPacket";

    public static BindableProperty OnDropCommandProperty = BindableProperty.Create
    (
        nameof(OnDropCommand),
        typeof(ICommand),
        typeof(BaseDropTarget),
        null
    );

    public Command<DropPacket> OnDropCommand
    {
        get => (Command<DropPacket>)GetValue(OnDropCommandProperty);
        set => SetValue(OnDropCommandProperty, value);
    }

    public static BindableProperty IsDraggingProperty = BindableProperty.Create
    (
        nameof(IsDraggingProperty),
        typeof(bool),
        typeof(BaseDropTarget),
        null,
        propertyChanged: OnIsDraggingChanged
    );

    public bool IsDragging
    {
        get => (bool)GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingProperty, value);
    }

    public BaseDropTarget()
	{
        ResetAndAddDragGestureRecognizer();
        SetUpBindingSources();
    }

    protected abstract void OnDragStart();
    protected abstract void OnDragEnd();
    protected abstract void OnDragOver();
    protected abstract void OnDragLeave();
    protected abstract void OnDrop(object dropPacket);

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ResetAndAddDragGestureRecognizer();
    }

    static void OnIsDraggingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BaseDropTarget dropTarget)
        {
            if (newValue is true)
            {
                dropTarget.OnDragStart();
                return;
            }

            dropTarget.OnDragEnd();
        }
    }

    void ResetAndAddDragGestureRecognizer()
    {
        RemoveGestureRecognizer();

        _dropGestureRecognizer = new DropGestureRecognizer()
        {
            AllowDrop = true
        };
        _dropGestureRecognizer.DragOver += DropGestureRecognizer_DragOver;
        _dropGestureRecognizer.DragLeave += DropGestureRecognizer_DragLeave;
        _dropGestureRecognizer.Drop += DropGestureRecognizer_Drop;

        GestureRecognizers.Add(_dropGestureRecognizer);
    }
    void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
    {
        OnDrop(e.Data.Properties[DropPacketName]);
        var dropPacket = new DropPacket(e.Data.Properties[DropPacketName],
            DateOnly.FromDateTime((BindingContext as CellViewModel).Interval.Start));
        OnDropCommand?.Execute(dropPacket);
    }

    void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
    {
        OnDragOver();
    }

    void DropGestureRecognizer_DragLeave(object sender, DragEventArgs e)
    {
        OnDragLeave();
    }

    void SetUpBindingSources()
    {
        // Set Bindings for IDropReceiver
        var source = new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext,
            typeof(IDropReceiver), ancestorLevel: 1); // 1 to find closest.

        SetBinding(IsDraggingProperty, new Binding()
        {
            Source = source,
            Path = nameof(IDropReceiver.IsDragging),
        });

        SetBinding(OnDropCommandProperty, new Binding()
        {
            Source = source,
            Path = nameof(IDropReceiver.OnDroppedCommand)
        });
    }

    void RemoveGestureRecognizer()
    {
        if (_dropGestureRecognizer is null)
        {
            return;
        }

        // Cleanup
        _dropGestureRecognizer.DragOver -= DropGestureRecognizer_DragOver;
        _dropGestureRecognizer.DragLeave -= DropGestureRecognizer_DragLeave;
        _dropGestureRecognizer.Drop -= DropGestureRecognizer_Drop;

        GestureRecognizers.Remove(_dropGestureRecognizer);
        _dropGestureRecognizer = null;
    }
}