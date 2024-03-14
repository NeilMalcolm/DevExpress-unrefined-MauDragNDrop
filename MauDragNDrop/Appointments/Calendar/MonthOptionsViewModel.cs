using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace MauDragNDrop.Calendar;

public partial class MonthOptionsViewModel : ObservableObject
{
    private readonly Action<DateOnly> _onMonthChanged;

    private MonthOption _selectedMonth;

    public DateTime MinDate => new (MiddleMonth.Date.Year, MiddleMonth.Date.Month, 0);

    [ObservableProperty]
    DateTime _start;

    [ObservableProperty]
    MonthOption _prevMonth;

    [ObservableProperty]
    MonthOption _middleMonth;

    [ObservableProperty]
    MonthOption _nextMonth;

    public DateTime MaxDate => new (MiddleMonth.Date.Year, MiddleMonth.Date.Month, DateTime.DaysInMonth(MiddleMonth.Date.Year, MiddleMonth.Date.Month));
    public ICommand MonthChangedCommand { get; private set; }

    public MonthOptionsViewModel(Action<DateOnly> monthChanged, DateOnly currentDate)
    {
        _onMonthChanged = monthChanged;
        MonthChangedCommand = new Command<MonthOption>(OnMonthChanged);

        SetupMonthsFromDate(currentDate);
    }

    private void OnMonthChanged(MonthOption selectedMonthOption)
    {
        var newDate = selectedMonthOption.Date;
        Start = new DateTime(newDate.Year, newDate.Month, 1);

        SetMonthIsSelected(selectedMonthOption);
        _onMonthChanged?.Invoke(newDate);
    }

    private void SetMonthIsSelected(MonthOption option)
    {
        if (_selectedMonth == option)
        {
            return;
        }

        if (_selectedMonth != null)
        {
            _selectedMonth.IsSelected = false;
        }

        _selectedMonth = option;
        _selectedMonth.IsSelected = true;
    }

    private void SetupMonthsFromDate(DateOnly date)
    {
        PrevMonth = new MonthOption(date.AddMonths(-1));
        MiddleMonth = new MonthOption(date);
        NextMonth = new MonthOption(date.AddMonths(1));

        Start = new DateTime(MiddleMonth.Date.Year, MiddleMonth.Date.Month, 1);
        SetMonthIsSelected(MiddleMonth);
    }

    public partial class MonthOption : ObservableObject
    {
        private string _text;
        public string Text => _text ??= GetMonthNameFromDate(Date);
        public DateOnly Date { get; }

        [NotifyPropertyChangedFor(nameof(IsEnabled))]
        [ObservableProperty]
        bool _isSelected;

        public bool IsEnabled => !IsSelected;

        public MonthOption(DateOnly date)
        {
            Date = date;
        }

        private static string GetMonthNameFromDate(DateOnly date)
            => date.ToString("MMM");
    }
}
