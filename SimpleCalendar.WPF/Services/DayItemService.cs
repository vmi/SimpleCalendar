using SimpleCalendar.WPF.Models;

namespace SimpleCalendar.WPF.Services
{
    internal class DateMap
    {
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, DayItem>>> map = [];

        public void Add(int year, int month, int day, DayItem dayItem)
        {
            if (!map.TryGetValue(year, out var m2d))
            {
                m2d = map[year] = [];
            }
            if (!m2d.TryGetValue(month, out var d2i))
            {
                d2i = m2d[month] = [];
            }
            d2i[day] = dayItem;
        }

        public DayItem? Get(int year, int month, int day)
        {
            if (!map.TryGetValue(year, out var m2d))
            {
                return null;
            }
            if (!m2d.TryGetValue(month, out var d2i))
            {
                return null;
            }
            if (!d2i.TryGetValue(day, out var item))
            {
                return null;
            }
            return item;
        }
    }

    public class DayItemService
    {
        private readonly DayType[] dayTypes = new DayType[DayItem.DAYS_IN_WEEK];
        private readonly DateMap map = new();

        public DayItemService()
        {
            for (int i = 0; i < dayTypes.Length; i++)
            {
                dayTypes[i] = i switch
                {
                    0 => DayType.SUNDAY,
                    6 => DayType.SATURDAY,
                    _ => DayType.WEEKDAY,
                };
            }
        }

        public DayItem GetDayItem(int year, int month, int day, int dow = -1)
        {
            var item = map.Get(year, month, day);
            if (item != null)
            {
                return item;
            }
            if (dow < 0)
            {
                DateOnly d = new(year, month, day);
                dow = (int) d.DayOfWeek;
            }
            return new(day, dayTypes[dow]);
        }
    }
}
