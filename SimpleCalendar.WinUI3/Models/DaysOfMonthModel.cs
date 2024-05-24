using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleCalendar.WinUI3.Models
{
    public partial class DaysOfMonthModel : ObservableObject
    {
        private readonly DayItemInformationModel _dayIteminformationModel;
        private readonly Dictionary<int, Dictionary<int, DaysMatrix>> _daysCache = [];

        [ObservableProperty]
        private DateTime _lastModified = DateTime.MinValue;

        public DaysOfMonthModel(DayItemInformationModel dayIteminformationModel)
        {
            _dayIteminformationModel = dayIteminformationModel;
            _dayIteminformationModel.PropertyChanged += DayIteminformationModel_PropertyChanged;
        }

        private void DayIteminformationModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _daysCache.Clear();
            LastModified = DateTime.Now;
        }

        public DaysMatrix GetDaysMatrix(YearMonth yearMonth)
        {
            lock (this)
            {
                int year = yearMonth.Year;
                int month = yearMonth.Month;
                if (!_daysCache.TryGetValue(year, out Dictionary<int, DaysMatrix> dms))
                {
                    dms = _daysCache[year] = [];
                }
                if (!dms.TryGetValue(month, out DaysMatrix dm))
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
                        dm[w][dow] = _dayIteminformationModel.GetDayItem(year, month, day, dow);
                    }
                    else
                    {
                        dm[w][dow] = DayItem.EMPTY;
                    }
                }
            }
        }
    }
}
