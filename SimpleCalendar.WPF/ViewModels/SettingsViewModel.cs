using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.Services;

namespace SimpleCalendar.WPF.ViewModels
{
    public record LogEntry(DateTime DateTime, string Message)
    {
        public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public string Text { get => $"[{DateTime.ToString(DateTimeFormat)}] {Message}"; }
    }

    public partial class SettingsViewModel(DayItemInformationModel dayItemInformationModel)
    {
        private DayItemInformationModel _dayItemInformationModel = dayItemInformationModel;

        public ObservableCollection<LogEntry> LogEntries { get; } = [];

        public void Log(string message)
        {
            var entry = new LogEntry(DateTime.Now, message);
            LogEntries.Add(entry);
        }

        [RelayCommand]
        private async Task ReloadSettings(object sender)
        {
            Dispatcher? dispatcher = (sender as DispatcherObject)?.Dispatcher;
            await Task.Run(() => _dayItemInformationModel.LoadSettings(dispatcher));
        }

        private void statusChanged(HolidayUpdaterStatus status, params object[] args)
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
                case Services.HolidayUpdaterStatus.UPDATED:
                    Log("祝日ファイルを再読み込みしました");
                    break;
                case Services.HolidayUpdaterStatus.ERROR:
                    HttpStatusCode statusCode = (HttpStatusCode)args[0];
                    Log($"祝日ファイルの取得に失敗しました (ステータスコード: {statusCode.ToString()})");
                    break;
            }
        }

        [RelayCommand]
        private async Task UpdateHolidays(object sender)
        {
            Dispatcher? dispatcher = (sender as DispatcherObject)?.Dispatcher;
            await _dayItemInformationModel.UpdateHolidays(dispatcher, async (status, args) =>
            {
                if (dispatcher != null)
                {
                    await dispatcher.InvokeAsync(() => statusChanged(status, args));
                }
                else
                {
                    statusChanged(status, args);
                }
            });
        }
    }
}
