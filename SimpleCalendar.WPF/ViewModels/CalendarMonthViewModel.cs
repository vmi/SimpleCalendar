using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.Services;
using System.ComponentModel;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class CalendarMonthViewModel : ObservableObject
    {
#if DEBUG
        private static int NextId = 0;
        public int Id = ++NextId;
#endif

        public CurrentMonthViewModel CurrentMonthViewModel { get; init; }

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
            _days = GetDays(_yearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(Days));
        }

        // for design mode only
        public CalendarMonthViewModel() : this(new CurrentMonthViewModel()) { }

        public CalendarMonthViewModel(CurrentMonthViewModel currentMonthViewModel)
        {
            CurrentMonthViewModel = currentMonthViewModel;
            CurrentMonthViewModel.PropertyChanged += CurrentMonthViewModel_PropertyChanged;
            _yearMonth = currentMonthViewModel.BaseYearMonth;
            _offset = 0;
            _days = GetDays(_yearMonth);
            OnPropertyChanged(nameof(YearMonth));
            OnPropertyChanged(nameof(Offset));
            OnPropertyChanged(nameof(Days));
        }

        private static readonly Dictionary<int, Dictionary<int, DayItem[][]>> _daysCache = [];

        private static DayItem[][] GetDays(YearMonth yearMonth)
        {
            lock (_daysCache)
            {
                var year = yearMonth.Year;
                var month = yearMonth.Month;
                if (!_daysCache.TryGetValue(year, out Dictionary<int, DayItem[][]>? dss))
                {
                    dss = _daysCache[year] = [];
                }
                if (!dss.TryGetValue(month, out DayItem[][]? ds))
                {
                    ds = dss[month] = new DayItem[DayItem.MAX_WEEKS_IN_MONTH][];
                    for (int i = 0; i < ds.Length; i++)
                    {
                        ds[i] = new DayItem[DayItem.DAYS_IN_WEEK];
                    }
                    FillDays(year, month, ds);
                }
                return ds;
            }
        }

        private static void FillDays(int year, int month, DayItem[][] days)
        {
            DateOnly firstDay = new(year, month, 1);
            int firstDayOfWeek = (int) firstDay.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int day = -firstDayOfWeek;
            for (int w = 0; w < DayItem.MAX_WEEKS_IN_MONTH; w++)
            {
                for (int dow = 0; dow < DayItem.DAYS_IN_WEEK; dow++)
                {
                    day++;
                    if (1 <= day && day <= daysInMonth)
                    {
                        days[w][dow] = DayItemService.Instance.GetDayItem(year, month, day, dow);
                    }
                    else
                    {
                        days[w][dow] = DayItem.EMPTY;
                    }
                }
            }
        }
    }
}
