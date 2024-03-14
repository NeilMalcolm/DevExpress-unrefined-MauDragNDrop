using MauDragNDrop.Calendar.Views;

namespace MauDragNDrop.Appointments.Calendar.Views;

public class SchedulerDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate MonthView { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return MonthView;
    }
}
