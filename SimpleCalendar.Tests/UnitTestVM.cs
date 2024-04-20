using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.ViewModels;

namespace SimpleCalendar.Tests
{
    public class UnitTestVM
    {
        [Fact]
        public void Test1()
        {
            var vm = new CalendarMonthViewModel();
            vm.CurrentMonthViewModel.BaseYearMonth = new(2024, 1);
            vm.Offset = 3;
            Assert.Equal(2024, vm.YearMonth.Year);
            Assert.Equal(4, vm.YearMonth.Month);
            Assert.Equal(1, vm.Days[0][1].Day);
            Assert.Equal(30, vm.Days[4][2].Day);
            Assert.Equal(DayType.SATURDAY, vm.Days[1][6].DayType);
        }
    }
}
