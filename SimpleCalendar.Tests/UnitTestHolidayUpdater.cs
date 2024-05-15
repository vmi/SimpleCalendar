using SimpleCalendar.WPF.Services;
using SimpleCalendar.WPF.Utilities;

namespace SimpleCalendar.Tests
{
    public class UnitTestHolidayUpdater : IDisposable
    {
        private string _tmpDir;

        public UnitTestHolidayUpdater()
        {
            _tmpDir = Path.Combine(Path.GetTempPath(), $"SimpleCalendar.{Path.GetRandomFileName()}");
            Directory.CreateDirectory(_tmpDir);
        }

        public void Dispose() => Directory.Delete(_tmpDir, true);

        [Fact]
        public async Task testHolidaysUpdater()
        {
            SettingFiles.UserSettingBaseDir = _tmpDir;
            SettingFiles.LocalSettingBaseDir = _tmpDir;
            var sus = new HolidayUpdaterService();
            string headerFile = SettingFiles.Holidays.ExtPath(SettingFiles.HEADER);
            if (File.Exists(headerFile))
            {
                File.Delete(headerFile);
            }
            HolidayUpdaterStatus status = await sus.UpdateAsync();
            Assert.Equal(HolidayUpdaterStatus.UPDATED, status);
            HolidayUpdaterStatus status2 = await sus.UpdateAsync();
            Assert.Equal(HolidayUpdaterStatus.NO_UPDATE_REQUIRED, status2);
        }
    }
}
