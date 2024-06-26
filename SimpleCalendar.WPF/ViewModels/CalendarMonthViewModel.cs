using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WPF.Models;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class CalendarMonthViewModel : ObservableObject
    {
        private readonly DaysOfMonthModel _daysOfMonthModel;

        public MainWindowViewModel CurrentMonth { get; init; }

        public DayLabelStyleSettingViewModel DayLabelStyleSetting { get; init; }

        [ObservableProperty]
        private int _offset;

        public YearMonth YearMonth { get; private set; }

        public DaysMatrix DaysMatrix { get; private set; }

        public CalendarMonthViewModel(DaysOfMonthModel daysOfMonthModel, MainWindowViewModel currentMonth, DayLabelStyleSettingViewModel dayLabelStyleSetting)
        {
            _daysOfMonthModel = daysOfMonthModel;
            _daysOfMonthModel.PropertyChanged += DaysOfMonthModel_PropertyChanged;

            CurrentMonth = currentMonth;
            CurrentMonth.PropertyChanged += CurrentMonth_PropertyChanged;

            DayLabelStyleSetting = dayLabelStyleSetting;

            YearMonth = currentMonth.BaseYearMonth;
            _offset = 0;
            DaysMatrix = daysOfMonthModel.GetDaysMatrix(YearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(Offset));
            OnPropertyChanged(nameof(DaysMatrix));
        }

        private void DaysOfMonthModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateDerivedProperties(CurrentMonth.BaseYearMonth);
        }

        private void CurrentMonth_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is MainWindowViewModel curMonth)
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

        partial void OnOffsetChanged(int oldValue, int newValue)
        {
            UpdateDerivedProperties(CurrentMonth.BaseYearMonth);
        }

        private void UpdateDerivedProperties(YearMonth baseYearMonth)
        {
            YearMonth = baseYearMonth.AddMonths(Offset);
            DaysMatrix = _daysOfMonthModel.GetDaysMatrix(YearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(DaysMatrix));
        }
    }
}
