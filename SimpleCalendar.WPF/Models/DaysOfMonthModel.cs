namespace SimpleCalendar.WPF.Models
{
    public class DaysOfMonthModel(DayItemInformationModel dayIteminformationModel)
    {
        private readonly Dictionary<int, Dictionary<int, DaysMatrix>> _daysCache = [];

        public DaysMatrix GetDaysMatrix(YearMonth yearMonth)
        {
            lock (this)
            {
                int year = yearMonth.Year;
                int month = yearMonth.Month;
                if (!_daysCache.TryGetValue(year, out Dictionary<int, DaysMatrix>? dms))
                {
                    dms = _daysCache[year] = [];
                }
                if (!dms.TryGetValue(month, out DaysMatrix? dm))
                {
                    dm = dms[month] = new DaysMatrix();
                    FillDays(year, month, dm);
                }
                return dm;
            }
        }

        private void FillDays(int year, int month, DaysMatrix dm)
        {
            DateOnly firstDay = new(year, month, 1);
            int firstDayOfWeek = (int)firstDay.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int day = -firstDayOfWeek;
            for (int w = 0; w < DaysMatrix.MAX_WEEKS_IN_MONTH; w++)
            {
                for (int dow = 0; dow < DaysMatrix.DAYS_IN_WEEK; dow++)
                {
                    day++;
                    if (1 <= day && day <= daysInMonth)
                    {
                        dm[w, dow] = dayIteminformationModel.GetDayItem(year, month, day, dow);
                    }
                    else
                    {
                        dm[w, dow] = DayItem.EMPTY;
                    }
                }
            }
        }
    }
}
