using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WPF.Services;
using SimpleCalendar.WPF.Utilities;
using static SimpleCalendar.WPF.Services.HolidayUpdaterService;

namespace SimpleCalendar.WPF.Models
{
    public partial class DayItemInformationModel : ObservableObject
    {
        private readonly HolidayUpdaterService _holidayUpdaterService;

        private readonly Dictionary<DateOnly, DayItem> _dateToDayItem = [];

        private readonly ReaderWriterLockSlim _lock = new();

        [ObservableProperty]
        private DateTime _lastModified = DateTime.MinValue;

        public DayItemInformationModel(HolidayUpdaterService holidayUpdaterService)
        {
            _holidayUpdaterService = holidayUpdaterService;
            LoadSettings();
        }

        public void LoadSettings(Dispatcher? dispatcher = null)
        {
            try
            {
                _lock.EnterWriteLock();

                // 既存情報の消去
                _dateToDayItem.Clear();

                // 祝祭日の読み込み
                SettingFiles.Holidays.ReadCsvFile(csvLine =>
                {
                    string dateStr = csvLine[0];
                    if (string.IsNullOrEmpty(dateStr))
                    {
                        return;
                    }
                    var date = DateOnly.Parse(dateStr);
                    string label = csvLine[1];
                    DayItem dayItem = new(date.Day, DayType.HOLIDAY, label);
                    _dateToDayItem.Add(date, dayItem);
                });

                // 特別日の読み込み
                SettingFiles.Specialdays.ReadCsvFile(csvLine =>
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
                    if (_dateToDayItem.TryGetValue(date, out DayItem? prevDayItem))
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
                    _dateToDayItem[date] = newDayItem;
                });
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(() => LastModified = DateTime.Now);
            }
            else
            {
                LastModified = DateTime.Now;
            }
        }

        public async Task<HolidayUpdaterStatus> UpdateHolidays(Dispatcher? dispatcher, StatusChanged statusChanged)
        {
            HolidayUpdaterStatus result = await _holidayUpdaterService.UpdateAsync(statusChanged).ConfigureAwait(false);
            if (result != HolidayUpdaterStatus.DOWNLOADED) { return result; }
            await Task.Run(() => LoadSettings(dispatcher)).ConfigureAwait(false);
            await statusChanged(HolidayUpdaterStatus.UPDATED).ConfigureAwait(false);
            return HolidayUpdaterStatus.UPDATED;
        }

        public DayItem GetDayItem(int year, int month, int day, int dow)
        {
            var date = new DateOnly(year, month, day);
            DayItem? dayItem;
            try
            {
                _lock.EnterReadLock();
                _dateToDayItem.TryGetValue(date, out dayItem);
            }
            finally
            {
                _lock.ExitReadLock();
            }
            return dayItem ?? new DayItem(day, (DayType)dow);
        }
    }
}
