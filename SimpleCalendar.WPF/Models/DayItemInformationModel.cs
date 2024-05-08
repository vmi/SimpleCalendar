using SimpleCalendar.WPF.Services;

namespace SimpleCalendar.WPF.Models
{
    public class DayItemInformationModel
    {
        private readonly SettingsService settingsService;
        private readonly Dictionary<DateOnly, DayItem> dateToDayItem = [];

        public DayItemInformationModel(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            LoadSettings();
        }

        public void LoadSettings()
        {
            // 既存情報の消去
            dateToDayItem.Clear();

            // 祝祭日の読み込み
            settingsService.ReadCsvFile(settingsService.HolydaysCsv, csvLine =>
            {
                string dateStr = csvLine[0];
                if (string.IsNullOrEmpty(dateStr))
                {
                    return;
                }
                var date = DateOnly.Parse(dateStr);
                string label = csvLine[1];
                DayItem dayItem = new(date.Day, DayType.HOLIDAY, label);
                dateToDayItem.Add(date, dayItem);
            });

            // 特別日の読み込み
            settingsService.ReadCsvFile(settingsService.SpecialDaysCsv, csvLine =>
            {
                string dateStr = csvLine[0];
                if (string.IsNullOrEmpty(dateStr))
                {
                    return;
                }
                var date = DateOnly.Parse(dateStr);
                string dTypeStr = csvLine[1];
                DayType dType = Enum.Parse<DayType>(dTypeStr);
                string label = csvLine[2];
                if (dateToDayItem.TryGetValue(date, out DayItem? prevDayItem))
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
            return dateToDayItem.TryGetValue(date, out DayItem? dayItem) ? dayItem : new DayItem(day, (DayType)dow);
        }
    }
}
