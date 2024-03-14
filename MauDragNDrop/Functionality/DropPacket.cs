namespace MauDragNDrop.Functionality;

public class DropPacket : DropPacket<object, object>
{
    public DropPacket(object droppedData,
        object droppedOnData)
    {
        DroppedData = droppedData;
        DroppedOnData = droppedOnData;
    }
}

public class DropPacket<TDropped, TDroppedOn>
{
    public TDropped DroppedData { get; set; }
    public TDroppedOn DroppedOnData { get; set; }
}