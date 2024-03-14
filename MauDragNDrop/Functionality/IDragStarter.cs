using System.Windows.Input;

namespace MauDragNDrop.Functionality;

public interface IDragStarter
{
    public ICommand OnDragStartCommand { get; }
    public ICommand OnDragEndCommand { get; }
}
