using SimpleCalendar.WPF.Services;
using SimpleCalendar.WPF.Utilities;

namespace SimpleCalendar.Tests
{
    public class UnitTestHolidayUpdater : IDisposable
    {
        private readonly string _tmpDir;

        public UnitTestHolidayUpdater()
        {
            _tmpDir = Path.Combine(Path.GetTempPath(), $"SimpleCalendar.{Path.GetRandomFileName()}");
            Directory.CreateDirectory(_tmpDir);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Directory.Delete(_tmpDir, true);
        }

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
            HolidayUpdaterStatus asyncStatus = HolidayUpdaterStatus.NO_UPDATE_REQUIRED;
#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
            async Task sc(HolidayUpdaterStatus status, params object[] args)
            {
                asyncStatus = status;
            }
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
            HolidayUpdaterStatus status = await sus.UpdateAsync(sc);
            Assert.Equal(HolidayUpdaterStatus.DOWNLOADED, status);
            Assert.Equal(HolidayUpdaterStatus.DOWNLOADED, asyncStatus);
            HolidayUpdaterStatus status2 = await sus.UpdateAsync(sc);
            Assert.Equal(HolidayUpdaterStatus.NO_UPDATE_REQUIRED, status2);
            Assert.Equal(HolidayUpdaterStatus.NO_UPDATE_REQUIRED, asyncStatus);
        }
    }
}
