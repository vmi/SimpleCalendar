using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WPF.Models;
using System.ComponentModel;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class CalendarMonthViewModel : ObservableObject
    {
        private readonly DaysOfMonthModel daysOfMonthModel;

        public CurrentMonthViewModel CurrentMonth { get; init; }

        public DayLabelStyleSettingViewModel DayLabelStyleSetting { get; init; }

        [ObservableProperty]
        private int _offset;

        private YearMonth _yearMonth;

        public YearMonth YearMonth { get => _yearMonth; }

        private DayItem[][] _days;

        public DayItem[][] Days { get => _days; }

        partial void OnOffsetChanged(int oldValue, int newValue)
        {
            UpdateDerivedProperties(CurrentMonth.BaseYearMonth);
        }

        private void CurrentMonth_PropertyChanged(object? sender, PropertyChangedEventArgs e)
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

        public CalendarMonthViewModel(DaysOfMonthModel daysOfMonthModel, CurrentMonthViewModel currentMonth, DayLabelStyleSettingViewModel dayLabelStyleSetting)
        {
            this.daysOfMonthModel = daysOfMonthModel;
            CurrentMonth = currentMonth;
            DayLabelStyleSetting = dayLabelStyleSetting;

            CurrentMonth.PropertyChanged += CurrentMonth_PropertyChanged;
            _yearMonth = currentMonth.BaseYearMonth;
            _offset = 0;
            _days = daysOfMonthModel.GetDays(_yearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(Offset));
            OnPropertyChanged(nameof(Days));
        }
    }
}
