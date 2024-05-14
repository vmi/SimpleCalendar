using System.Diagnostics;
using SimpleCalendar.WPF.Utilities;

namespace SimpleCalendar.Tests
{
    public class UnitTestSettings
    {
        [Fact]
        public void TestSettingsServce()
        {
            SettingFiles.UserSettingBaseDir = Path.GetFullPath(@".\Roaming");
            SettingFiles.AppName = $"{nameof(SimpleCalendar)}.{nameof(SimpleCalendar.Tests)}";
            HashSet<string> act = [];
            SettingFiles.Styles.ReadCsvFile(csvLine =>
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
