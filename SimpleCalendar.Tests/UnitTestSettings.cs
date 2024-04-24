using SimpleCalendar.WPF.Services;
using System.Diagnostics;

namespace SimpleCalendar.Tests
{
    public class UnitTestSettings
    {
        [Fact]
        public void TestSettingsServce()
        {
            SettingsService.UserSettingBaseDir = Path.GetFullPath(@".\Roaming");
            SettingsService.AppName = $"{nameof(SimpleCalendar)}.{nameof(SimpleCalendar.Tests)}";
            SettingsService s = new();
            HashSet<string> act = [];
            s.ReadCsvFile("styles.csv", csvLine =>
            {
                Debug.WriteLine($"Row=[{csvLine}]");
                if (!string.IsNullOrEmpty(csvLine[0]))
                {
                    act.Add(csvLine[0]);
                }
            });
            HashSet<string> exp = [
                "SUNDAY",
                "MONDAY",
                "TUESDAY",
                "WEDNESDAY",
                "THURSDAY",
                "FRIDAY",
                "SATURDAY",
                "HOLIDAY",
                "SPECIALDAY01",
                "SPECIALDAY02",
                "SPECIALDAY03",
                "Today",
                "Focus",
            ];
            Assert.Equal(act, exp);
        }
    }
}
