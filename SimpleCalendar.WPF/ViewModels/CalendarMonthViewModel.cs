using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WPF.Models;
using System.ComponentModel;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class CalendarMonthViewModel : ObservableObject
    {
        private readonly DaysOfMonthModel daysOfMonthModel;

        public CurrentMonthViewModel CurrentMonthViewModel { get; init; }

        public DayLabelStyleViewModel DayLabelStyleViewModel { get; init; }

        [ObservableProperty]
        private int _offset;

        private YearMonth _yearMonth;

        public YearMonth YearMonth { get => _yearMonth; }

        private DayItem[][] _days;

        public DayItem[][] Days { get => _days; }

        partial void OnOffsetChanged(int oldValue, int newValue)
        {
            UpdateDerivedProperties(CurrentMonthViewModel.BaseYearMonth);
        }

        private void CurrentMonthViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is CurrentMonthViewModel curMonth)
            {
                switch (e.PropertyName)
                {
                    case nameof(curMonth.BaseYearMonth):
                        UpdateDerivedProperties(curMonth.BaseYearMonth);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateDerivedProperties(YearMonth baseYearMonth)
        {
            _yearMonth = baseYearMonth.AddMonths(Offset);
            _days = daysOfMonthModel.GetDays(_yearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(Days));
        }

        public CalendarMonthViewModel(DaysOfMonthModel daysOfMonthService, CurrentMonthViewModel currentMonthViewModel, DayLabelStyleViewModel dayLabelStyleViewModel)
        {
            this.daysOfMonthModel = daysOfMonthService;
            CurrentMonthViewModel = currentMonthViewModel;
            DayLabelStyleViewModel = dayLabelStyleViewModel;

            CurrentMonthViewModel.PropertyChanged += CurrentMonthViewModel_PropertyChanged;
            _yearMonth = currentMonthViewModel.BaseYearMonth;
            _offset = 0;
            _days = daysOfMonthService.GetDays(_yearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(Offset));
            OnPropertyChanged(nameof(Days));
        }
    }
}
