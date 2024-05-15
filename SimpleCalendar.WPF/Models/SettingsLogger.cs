using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SimpleCalendar.WPF.Models
{

    public record LogEntry(DateTime DateTime, string Message)
    {
        public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public string Text { get => $"[{DateTime.ToString(DateTimeFormat)}] {Message}"; }
    }

    public class SettingsLogger
    {
        public ObservableCollection<LogEntry> LogEntries { get; } = [];

        public void Log(string message)
        {
            var entry = new LogEntry(DateTime.Now, message);
            Dispatcher.CurrentDispatcher.Invoke(() => LogEntries.Add(entry));
        }
    }
}
