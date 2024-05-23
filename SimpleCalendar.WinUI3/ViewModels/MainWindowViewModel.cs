using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WinUI3.Models;

namespace SimpleCalendar.WinUI3.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public readonly SettingsViewModel SettingsViewModel;

        private DateOnly _today;

        public DateOnly Today { get => _today; private set => SetProperty(ref _today, value); }

        [ObservableProperty]
        private YearMonth _baseYearMonth;

        [ObservableProperty]
        private int _rowCount;

        [ObservableProperty]
        private int _columnCount;

        public MainWindowViewModel(SettingsViewModel settingsViewModel)
        {
            Today = DateOnly.FromDateTime(DateTime.Now);
            BaseYearMonth = new(Today);
            SettingsViewModel = settingsViewModel;
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
            int pageCount = RowCount * ColumnCount;
            BaseYearMonth = BaseYearMonth.AddMonths(-pageCount);
        }

        [RelayCommand]
        private void NextPage()
        {
            int pageCount = RowCount * ColumnCount;
            BaseYearMonth = BaseYearMonth.AddMonths(pageCount);
        }
    }
}
