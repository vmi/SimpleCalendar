using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WPF.Services;
using System.Windows;
using System.Windows.Media;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class DayLabelStyleSettingViewModel : ObservableObject
    {
        private readonly SettingsService settingsService;
        private readonly BrushConverter brushConverter = new();

        private Brush _sundayBrush = Brushes.Red;
        public Brush SundayBrush { get => _sundayBrush; private set => SetProperty(ref _sundayBrush, value); }

        private Brush _mondayBrush = SystemColors.ControlTextBrush;
        public Brush MondayBrush { get => _mondayBrush; private set => SetProperty(ref _mondayBrush, value); }

        private Brush _tuesdayBrush = SystemColors.ControlTextBrush;
        public Brush TuesdayBrush { get => _tuesdayBrush; private set => SetProperty(ref _tuesdayBrush, value); }

        private Brush _wednesdayBrush = SystemColors.ControlTextBrush;
        public Brush WednesdayBrush { get => _wednesdayBrush; private set => SetProperty(ref _wednesdayBrush, value); }

        private Brush _thursdayBrush = SystemColors.ControlTextBrush;
        public Brush ThursdayBrush { get => _thursdayBrush; private set => SetProperty(ref _thursdayBrush, value); }

        private Brush _fridayBrush = SystemColors.ControlTextBrush;
        public Brush FridayBrush { get => _fridayBrush; private set => SetProperty(ref _fridayBrush, value); }

        private Brush _saturdayBrush = Brushes.Blue;
        public Brush SaturdayBrush { get => _saturdayBrush; private set => SetProperty(ref _saturdayBrush, value); }

        private Brush _holidayBrush = Brushes.Magenta;
        public Brush HolidayBrush { get => _holidayBrush; private set => SetProperty(ref _holidayBrush, value); }

        private Brush _specialday1Brush = Brushes.OrangeRed;
        public Brush Specialday1Brush { get => _specialday1Brush; private set => SetProperty(ref _specialday1Brush, value); }

        private Brush _specialday2Brush = Brushes.OrangeRed;
        public Brush Specialday2Brush { get => _specialday2Brush; private set => SetProperty(ref _specialday2Brush, value); }

        private Brush _specialday3Brush = Brushes.OrangeRed;
        public Brush Specialday3Brush { get => _specialday3Brush; private set => SetProperty(ref _specialday3Brush, value); }

        private Brush _todayBrush = Brushes.LightSkyBlue;
        public Brush TodayBrush { get => _todayBrush; private set => SetProperty(ref _todayBrush, value); }

        private Brush _mouseOverBrush = Brushes.LightGreen;
        public Brush MouseOverBrush { get => _mouseOverBrush; private set => SetProperty(ref _mouseOverBrush, value); }

        public DayLabelStyleSettingViewModel(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            LoadSetting();
        }

        private Brush ToBrush(string? brushName, Brush defaultBrush)
        {
            if (!String.IsNullOrEmpty(brushName) && brushConverter.ConvertFromString(brushName) is Brush brush)
            {
                return brush;
            }
            else
            {
                return defaultBrush;
            }
        }

        [RelayCommand]
        private void LoadSetting()
        {
            settingsService.ReadCsvFile(settingsService.StylesCsv, csvLine =>
            {
                if (csvLine.ColumnCount == 0 || String.IsNullOrEmpty(csvLine[0])) { return; }
                string dTypeName = csvLine[0].ToUpper();
                string brushName = csvLine.ColumnCount >= 2 ? csvLine[1] ?? "" : "";
                switch (dTypeName)
                {
                    case "SUNDAY": SundayBrush = ToBrush(brushName, Brushes.Red); break;
                    case "MONDAY": MondayBrush = ToBrush(brushName, SystemColors.ControlTextBrush); break;
                    case "TUESDAY": TuesdayBrush = ToBrush(brushName, SystemColors.ControlTextBrush); break;
                    case "WEDNESDAY": WednesdayBrush = ToBrush(brushName, SystemColors.ControlTextBrush); break;
                    case "THURSDAY": ThursdayBrush = ToBrush(brushName, SystemColors.ControlTextBrush); break;
                    case "FRIDAY": FridayBrush = ToBrush(brushName, SystemColors.ControlTextBrush); break;
                    case "SATURDAY": SaturdayBrush = ToBrush(brushName, Brushes.Blue); break;
                    case "HOLIDAY": HolidayBrush = ToBrush(brushName, Brushes.Magenta); break;
                    case "SPECIALDAY1": Specialday1Brush = ToBrush(brushName, Brushes.OrangeRed); break;
                    case "SPECIALDAY2": Specialday2Brush = ToBrush(brushName, Brushes.OrangeRed); break;
                    case "SPECIALDAY3": Specialday3Brush = ToBrush(brushName, Brushes.OrangeRed); break;
                    case "TODAY": TodayBrush = ToBrush(brushName, Brushes.LightSkyBlue); break;
                    case "MOUSEOVER": MouseOverBrush = ToBrush(brushName, Brushes.LightGreen); break;
                    default: break;
                }
            });
        }
    }
}
