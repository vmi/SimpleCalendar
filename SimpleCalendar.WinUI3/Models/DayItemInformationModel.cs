using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WinUI3.Services;
using SimpleCalendar.WinUI3.Utilities;
using static SimpleCalendar.WinUI3.Services.HolidayUpdaterService;

namespace SimpleCalendar.WinUI3.Models
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

        public void LoadSettings()
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
                    if (_dateToDayItem.TryGetValue(date, out DayItem prevDayItem))
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
            LastModified = DateTime.Now;
        }

        public async Task<HolidayUpdaterStatus> UpdateHolidays(StatusChanged statusChanged)
        {
            return await _holidayUpdaterService.UpdateAsync(statusChanged).ConfigureAwait(false);
        }

        public DayItem GetDayItem(int year, int month, int day, int dow)
        {
            var date = new DateOnly(year, month, day);
            try
            {
                _lock.EnterReadLock();
                if (_dateToDayItem.TryGetValue(date, out DayItem dayItem))
                {
                    return dayItem;
                }
                else
                {
                    return new DayItem(day, (DayType)dow);
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
