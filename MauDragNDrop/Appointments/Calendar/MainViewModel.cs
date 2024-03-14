using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DevExpress.Xpo.DB;
using MauDragNDrop.Appointments.Services;
using MauDragNDrop.Functionality;
using System.Windows.Input;

namespace MauDragNDrop.Calendar;

public partial class MainViewModel : BaseViewModel, IDropReceiver
{
    private readonly IAppointmentService _appointmentService;

    [ObservableProperty]
    MonthOptionsViewModel _months;

    [ObservableProperty]
    List<AppointmentModel> _allAppointments;

    [ObservableProperty]
    DayListViewModel _selectedDayAppointments;

    private int FakeUserId = 1;

    public DateOnly Today = DateOnly.FromDateTime(DateTime.Now);

    [ObservableProperty]
    DateOnly _selectedDay;

    [ObservableProperty]
    private bool _isDragging;

    [ObservableProperty]
    List<ItemList> _items;

    public Command<DropPacket> OnDroppedCommand { get; }

    public MainViewModel(IAppointmentService appointmentService)
    {
        OnDroppedCommand = new Command<DropPacket>(OnDropped);
        SelectedDay = Today;
        _appointmentService = appointmentService;
        Months = new MonthOptionsViewModel(OnMonthChanged, SelectedDay);
        Task.Run(LoadData);

        Items = GetItemsList();
    }

    private List<ItemList> GetItemsList()
    {
        var list = new List<ItemList>
        {
            new ItemList
            {
                Date = DateTime.Today
            },
            new ItemList
            {
                Date = DateTime.Today.AddDays(1)
            },
            new ItemList
            {
                Date = DateTime.Today.AddDays(2)
            },
            new ItemList
            {
                Date = DateTime.Today.AddDays(3)
            }
        };
        list[0].Add("first");
        list[2].Add("third");
        list[3].Add("fourth");

        return list;
    }

    private void OnDropped(DropPacket dropPacket)
    {
        // handle
        var appointmentToMove = (AppointmentModel)dropPacket.DroppedData;
        var dateToMoveTo = (DateOnly)dropPacket.DroppedOnData;

        if (DateOnly.FromDateTime(appointmentToMove.StartTime.Date) == dateToMoveTo)
        {
            return;
        }

        var newStartTime = new DateTime(dateToMoveTo.Year,
            dateToMoveTo.Month,
            dateToMoveTo.Day,
            appointmentToMove.StartTime.Hour,
            appointmentToMove.StartTime.Minute,
            appointmentToMove.StartTime.Second,
            appointmentToMove.StartTime.Millisecond);

        var originalStartEndDelta = appointmentToMove.EndTime - appointmentToMove.StartTime;

        var newEndTime = newStartTime.Add(originalStartEndDelta);

        appointmentToMove.UpdateStartAndEndTimes(newStartTime, newEndTime);
        DaySelected(dateToMoveTo);
    }

    private async Task LoadData()
    {
        // get appointments
        AllAppointments = (await _appointmentService.GetAppointmentsForUser(FakeUserId))
            .Select(a => new AppointmentModel(a.Id, a.Start, a.End, a.Type, a.Patient?.Name))
            .ToList();

        SelectedDayAppointments = new DayListViewModel(SelectedDay, AllAppointments);
    }

    private void OnMonthChanged(DateOnly selectedMonth)
    {
        DaySelected(selectedMonth);
    }

    [RelayCommand]
    private void DaySelected(DateOnly selectedDay)
    {
        if (SelectedDay == selectedDay)
        {
            return;
        }

        SelectedDay = selectedDay;
        SelectedDayAppointments = new DayListViewModel(SelectedDay, AllAppointments);
    }

    [RelayCommand]
    private void DragEnded(AppointmentModel draggedAppointment)
    {
        WeakReferenceMessenger.Default.Send<AppointmentDragEndedMessage>(draggedAppointment);
    }

    [RelayCommand]
    private void DragStarted(AppointmentModel draggedAppointment)
    {
        WeakReferenceMessenger.Default.Send<AppointmentDragStartedMessage>(draggedAppointment);
    }
}

public class AppointmentDragStartedMessage
{
    public AppointmentModel Appointment { get; }

    public AppointmentDragStartedMessage(AppointmentModel draggedAppointment)
    {
        Appointment = draggedAppointment;
    }

    public static implicit operator AppointmentDragStartedMessage(AppointmentModel appointment)
    {
        return new(appointment);
    }
}

public class AppointmentDragEndedMessage
{
    public AppointmentModel Appointment { get; }

    public AppointmentDragEndedMessage(AppointmentModel draggedAppointment)
    {
        Appointment = draggedAppointment;
    }

    public static implicit operator AppointmentDragEndedMessage(AppointmentModel appointment)
    {
        return new(appointment);
    }
}

public class ItemList : List<string>
{
    public DateTime Date { get; set; }
    public string Title => Date.Day.ToString();
}

public partial class DayListViewModel : ObservableObject
{
    [ObservableProperty]
    IReadOnlyList<AppointmentModel> _appointmentsForDay;

    public DayListViewModel(DateOnly day, IEnumerable<AppointmentModel> appointments)
    {
        DateTime dayDate = day.ToDateTime(new TimeOnly(0, 0)).Date;

        AppointmentsForDay = appointments
            .Where(a => (!a.IsMultiDay && dayDate == a.StartTime.Date && dayDate == a.EndTime.Date)
                || (a.IsMultiDay && (a.StartTime.Date <= dayDate && a.EndTime >= dayDate)))
            .ToList();
    }
}

public class AppointmentModel : ObservableObject
{
    private string _title;
    public int Id { get; }

    private DateTime _startTime;

    public DateTime StartTime
    {
        get => _startTime;
        private set
        {
            _startTime = value;
            OnPropertyChanged();
        }
    }

    private DateTime _endTime;

    public DateTime EndTime
    {
        get => _endTime;
        private set
        {
            _endTime = value;
            OnPropertyChanged();
        }
    }

    public string Type { get; }
    public string PatientName { get; }
    public string Title => _title ??= PatientName is null ? Type : $"{Type}: {PatientName}";
    public bool IsMultiDay => StartTime.Day != EndTime.Day;

    public string AppointmentStartToEnd => GetTimeFormatted();
    public AppointmentModel(int id, DateTime startDate, DateTime endDate, string type, string patientName)
    {
        Id = id;
        StartTime = startDate;
        EndTime = endDate;
        Type = type;
        PatientName = patientName;
    }

    public void UpdateStartAndEndTimes(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    private string GetTimeFormatted()
    {
        if (!IsMultiDay)
        {
            return $"{StartTime:HH:mm} - {EndTime:HH:mm}";
        }

        return $"{StartTime:dd/MM HH:mm} - {EndTime:dd/MM HH:mm}";
    }
}