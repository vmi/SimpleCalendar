namespace SimpleCalendar.WPF.Models
{
    public readonly record struct YearMonth
    {
        public static YearMonth ThisMonth()
        {
            DateTime now = DateTime.Now;
            return new(now.Year, now.Month);
        }

        public int Year { get; init; }
        public int Month { get; init; }

        public YearMonth(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public YearMonth(DateOnly dateOnly)
        {
            Year = dateOnly.Year;
            Month = dateOnly.Month;
        }

        public YearMonth AddMonths(int months)
        {
            return new(((DateOnly)this).AddMonths(months));
        }

        public static implicit operator DateOnly(YearMonth yearMonth)
        {
            return new(yearMonth.Year, yearMonth.Month, 1);
        }

        public override string ToString()
        {
            return $"{Year:0000}-{Month:00}";
        }
    }

    public enum DayType
    {
        EMPTY = -1,

        SUNDAY,    // = 0 - DayOfWeek Enum (https://learn.microsoft.com/ja-jp/dotnet/api/system.dayofweek?view=net-8.0)
        MONDAY,    // = 1
        TUESDAY,   // = 2
        WEDNESDAY, // = 3
        THURSDAY,  // = 4
        FRIDAY,    // = 5
        SATURDAY,  // = 6

        HOLIDAY,
        SPECIALDAY1,
        SPECIALDAY2,
        SPECIALDAY3,
    }

    public class DayItem
    {
        public const int DAYS_IN_WEEK = 7;
        public const int MAX_WEEKS_IN_MONTH = 6;

        public const int EMPTY_DAY = -1;
        public const string NO_LABEL = "";
        public static readonly DayItem EMPTY = new(EMPTY_DAY, DayType.EMPTY, NO_LABEL);

        public int Day { get; }
        public string DayString { get; }
        public DayType DayType { get; }
        public string Label { get; }

        public DayItem(int day, DayType dayType, string label = NO_LABEL)
        {
            if (day >= 0)
            {
                Day = day;
                DayString = day.ToString();
            }
            else
            {
                Day = EMPTY_DAY;
                DayString = "";

            }
            DayType = dayType;
            Label = label;
        }
    }
}
