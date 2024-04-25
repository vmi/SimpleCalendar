using SimpleCalendar.WPF.Services;

namespace SimpleCalendar.WPF.Models
{
    public class DayItemInformationModel(SettingsService settingsService)
    {
        private readonly Dictionary<DayType, DayTypeStyle> dayTypeStyles = [];
        private readonly Dictionary<DateOnly, DayItem> dateToDayItem = [];

        public void LoadSettings()
        {
            // 既存情報の消去
            dayTypeStyles.Clear();
            dateToDayItem.Clear();

            // スタイル情報の読み込み
            settingsService.ReadCsvFile(settingsService.StylesCsv, csvLine =>
            {
                string dTypeName = csvLine[0];
                if (String.IsNullOrEmpty(dTypeName))
                {
                    return;
                }
                DayType dType = Enum.Parse<DayType>(dTypeName);
                string fg = csvLine[1];
                string bg = csvLine[2];
                string bd = csvLine[3];
                dayTypeStyles[dType] = new DayTypeStyle(fg, bg, bd);
            });

            // 祝祭日の読み込み
            settingsService.ReadCsvFile(settingsService.HolydaysCsv, csvLine =>
            {
                var dateStr = csvLine[0];
                if (String.IsNullOrEmpty(dateStr))
                {
                    return;
                }
                var date = DateOnly.Parse(dateStr);
                var label = csvLine[1];
                DayItem dayItem = new(date.Day, DayType.HOLIDAY, label);
                dateToDayItem.Add(date, dayItem);
            });

            // 特別日の読み込み
            settingsService.ReadCsvFile(settingsService.SpecialDaysCsv, csvLine =>
            {
                string dateStr = csvLine[0];
                if (String.IsNullOrEmpty(dateStr))
                {
                    return;
                }
                var date = DateOnly.Parse(dateStr);
                var dTypeStr = csvLine[1];
                DayType dType = Enum.Parse<DayType>(dTypeStr);
                var label = csvLine[2];
                if (dateToDayItem.TryGetValue(date, out var prevDayItem))
                {
                    // DayTypeの優先度は HOLIDAY < SPECIALDAY1 < SPECIALDAY2 < SPECIALDAY3 とする
                    if (dType < prevDayItem.DayType)
                    {
                        dType = prevDayItem.DayType;
                    }
                    // 日付が重複する場合はラベルを結合する
                    label = $"{prevDayItem.Label}\n{label}";
                }
                DayItem newDayItem = new(date.Day, dType, label);
                dateToDayItem.Add(date, newDayItem);
            });
        }

        public DayItem GetDayItem(int year, int month, int day, int dow)
        {
            var date = new DateOnly(year, month, day);
            return dateToDayItem[date] ?? new DayItem(day, (DayType) dow);
        }
    }
}
