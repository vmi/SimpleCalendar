using SimpleCalendar.WPF;
using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.ViewModels;

namespace SimpleCalendar.Tests
{
    public class UnitTestViewModel
    {
        [Fact]
        public void TestSR()
        {
            DayItemInformationModel? diim = ServiceRegistry.GetService<DayItemInformationModel>();
            Assert.NotNull(diim);
            DaysOfMonthModel? doms = ServiceRegistry.GetService<DaysOfMonthModel>();
            Assert.NotNull(doms);
            MainWindowViewModel? curMon = ServiceRegistry.GetService<MainWindowViewModel>();
            Assert.NotNull(curMon);
            CalendarMonthViewModel? calMon = ServiceRegistry.GetService<CalendarMonthViewModel>();
            Assert.NotNull(calMon);
        }

        [Fact]
        public void Test1()
        {
            CalendarMonthViewModel? vm = ServiceRegistry.GetService<CalendarMonthViewModel>();
            Assert.NotNull(vm);
            vm.CurrentMonth.BaseYearMonth = new(2024, 1);
            vm.Offset = 3;
            Assert.Equal(2024, vm.YearMonth.Year);
            Assert.Equal(4, vm.YearMonth.Month);
            Assert.Equal(1, vm.DaysMatrix[0, 1].Day);
            Assert.Equal(30, vm.DaysMatrix[4, 2].Day);
            Assert.Equal(DayType.SATURDAY, vm.DaysMatrix[1, 6].DayType);
        }
    }
}
