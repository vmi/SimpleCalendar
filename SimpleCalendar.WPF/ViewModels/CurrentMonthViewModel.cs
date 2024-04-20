using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WPF.Models;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class CurrentMonthViewModel : ObservableObject
    {
        private DateOnly _today;

        public DateOnly Today { get => _today; private set => SetProperty(ref _today, value); }

        [ObservableProperty]
        private YearMonth _baseYearMonth;

        [ObservableProperty]
        private int _rowCount;

        [ObservableProperty]
        private int _columnCount;

        public CurrentMonthViewModel()
        {
            Today = DateOnly.FromDateTime(DateTime.Now);
            BaseYearMonth = new(Today);
        }

        [RelayCommand]
        private void UpdateToday()
        {
            Today = DateOnly.FromDateTime(DateTime.Now);
        }

        [RelayCommand]
        private void ResetPage()
        {
            BaseYearMonth = new YearMonth(Today);
        }

        [RelayCommand]
        private void PrevMonth()
        {
            BaseYearMonth = BaseYearMonth.AddMonths(-1);
        }

        [RelayCommand]
        private void NextMonth()
        {
            BaseYearMonth = BaseYearMonth.AddMonths(1);
        }

        [RelayCommand]
        private void PrevLine()
        {
            BaseYearMonth = BaseYearMonth.AddMonths(-ColumnCount);
        }

        [RelayCommand]
        private void NextLine()
        {
            BaseYearMonth = BaseYearMonth.AddMonths(ColumnCount);
        }

        [RelayCommand]
        private void PrevPage()
        {
            var pageCount = RowCount * ColumnCount;
            BaseYearMonth = BaseYearMonth.AddMonths(-pageCount);
        }

        [RelayCommand]
        private void NextPage()
        {
            var pageCount = RowCount * ColumnCount;
            BaseYearMonth = BaseYearMonth.AddMonths(pageCount);
        }
    }
}
