using SimpleCalendar.WPF.Services;

namespace SimpleCalendar.WPF.Models
{
    public class DaysOfMonthModel(DayItemInformationModel dayIteminformationModel)
    {
        private readonly Dictionary<int, Dictionary<int, DayItem[][]>> _daysCache = [];

        public DayItem[][] GetDays(YearMonth yearMonth)
        {
            lock (this)
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

        private void FillDays(int year, int month, DayItem[][] days)
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
                        days[w][dow] = dayIteminformationModel.GetDayItem(year, month, day, dow);
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
