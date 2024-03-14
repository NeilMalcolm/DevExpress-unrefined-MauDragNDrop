using System.Windows.Input;

namespace MauDragNDrop.Functionality;

public interface IDropReceiver
{
    bool IsDragging { get; }
    Command<DropPacket> OnDroppedCommand { get; }
}
