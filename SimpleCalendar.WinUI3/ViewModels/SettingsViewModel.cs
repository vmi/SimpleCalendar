using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WinUI3.Models;
using SimpleCalendar.WinUI3.Services;
using SimpleCalendar.WinUI3.Utilities;

namespace SimpleCalendar.WinUI3.ViewModels
{
    public record LogEntry(DateTime DateTime, string Message)
    {
        public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public string Text { get => $"[{DateTime.ToString(DateTimeFormat)}] {Message}"; }
    }

    public partial class SettingsViewModel
    {
        private readonly DayItemInformationModel _dayItemInformationModel;
        private readonly FileSystemWatcher _watcher;
        private int _reloadCount = 0;

        public DayLabelStyleSettingViewModel DayLabelStyleSettingModel { get; }

        public ObservableCollection<LogEntry> LogEntries { get; } = [];

        public SettingsViewModel(DayItemInformationModel dayItemInformationModel, DayLabelStyleSettingViewModel dayLabelStyleSettingViewModel)
        {
            //BindingOperations.EnableCollectionSynchronization(LogEntries, new object());
            _dayItemInformationModel = dayItemInformationModel;
            DayLabelStyleSettingModel = dayLabelStyleSettingViewModel;
            _watcher = new FileSystemWatcher(Path.Combine(SettingFiles.UserSettingBaseDir, SettingFiles.AppName));
            _watcher.Filters.Add(SettingFiles.Holidays.SettingFilename);
            _watcher.Filters.Add(SettingFiles.Specialdays.SettingFilename);
            _watcher.Filters.Add(SettingFiles.Styles.SettingFilename);
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += SettingFiles_Changed;
            _watcher.EnableRaisingEvents = true;
        }

        public void Log(string message)
        {
            var entry = new LogEntry(DateTime.Now, message);
            LogEntries.Add(entry);
        }

        private void SettingFiles_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;
            // ファイル更新を認識してから1秒以内の更新は1つにまとめる
            int c = Interlocked.Increment(ref _reloadCount);
            Task.Delay(1000).ContinueWith(task =>
            {
                if (c == _reloadCount)
                {
                    _dayItemInformationModel.LoadSettings();
                    DayLabelStyleSettingModel.LoadSetting();
                    Log($"設定ファイルを再読み込みしました");
                }
            });
        }

        private Task statusChanged(HolidayUpdaterStatus status, params object[] args)
        {
            switch (status)
            {
                case Services.HolidayUpdaterStatus.IN_PROGRESS:
                    Log("祝日ファイルの更新を確認中");
                    break;
                case Services.HolidayUpdaterStatus.NO_UPDATE_REQUIRED:
                    DateTimeOffset lm = (DateTimeOffset)args[0];
                    Log($"祝日ファイルは最新です (最終更新日時: {lm.ToLocalTime():yyyy-MM-dd(ddd) HH:mm:ss zzz})");
                    break;
                case Services.HolidayUpdaterStatus.DOWNLOADED:
                    Log("祝日ファイルを最新化しました");
                    break;
                case Services.HolidayUpdaterStatus.ERROR:
                    string error = args.Length >= 1 ? $" (エラー情報: {args[0]})" : "";
                    Log($"祝日ファイルの取得に失敗しました{error}");
                    break;
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task UpdateHolidays(object sender)
        {
            await _dayItemInformationModel.UpdateHolidays(statusChanged);
        }
    }
}
