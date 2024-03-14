using MauDragNDrop.Appointments.Services;
using System.Diagnostics;

namespace MauDragNDrop.Calendar.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
        InitializeComponent();
		BindingContext = new MainViewModel(new AppointmentService());
        //MonthView.PropertyChanging += MonthView_PropertyChanging;
    }

    private void MonthView_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        Debug.WriteLine($"PropertyChanging: {e.PropertyName}");
    }
    
    private void MonthView_Tap(object sender, DevExpress.Maui.Scheduler.SchedulerGestureEventArgs e)
    {
        (BindingContext as MainViewModel).DaySelectedCommand.Execute(DateOnly.FromDateTime(e.IntervalInfo.Start));
    }

    private void MonthView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"PropertyChanged: {e.PropertyName}");
    }

    private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
    {

    }
}

