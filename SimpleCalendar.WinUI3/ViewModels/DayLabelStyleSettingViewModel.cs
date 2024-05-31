using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SimpleCalendar.WinUI3.Utilities;
using Windows.UI;

namespace SimpleCalendar.WinUI3.ViewModels
{
    public partial class DayLabelStyleSettingViewModel : ObservableObject
    {
        public Brush Red { get; } = new SolidColorBrush(Colors.Red);
        public Brush Blue { get; } = new SolidColorBrush(Colors.Blue);
        public Brush Magenta { get; } = new SolidColorBrush(Colors.Magenta);
        public Brush OrangeRed { get; } = new SolidColorBrush(Colors.OrangeRed);
        public Brush LightSkyBlue { get; } = new SolidColorBrush(Colors.LightSkyBlue);
        public Brush LightGreen { get; } = new SolidColorBrush(Colors.LightGreen);
        public Brush ControlTextBrush { get; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
        public Brush DefaultBackgroundBrush { get; } = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];
        public Brush DefaultBorderBrush { get; } = (Brush)Application.Current.Resources["ControlElevationBorderBrush"];

        private Brush _sundayBrush;
        public Brush SundayBrush { get => _sundayBrush; private set => SetProperty(ref _sundayBrush, value); }

        private Brush _mondayBrush;
        public Brush MondayBrush { get => _mondayBrush; private set => SetProperty(ref _mondayBrush, value); }

        private Brush _tuesdayBrush;
        public Brush TuesdayBrush { get => _tuesdayBrush; private set => SetProperty(ref _tuesdayBrush, value); }

        private Brush _wednesdayBrush;
        public Brush WednesdayBrush { get => _wednesdayBrush; private set => SetProperty(ref _wednesdayBrush, value); }

        private Brush _thursdayBrush;
        public Brush ThursdayBrush { get => _thursdayBrush; private set => SetProperty(ref _thursdayBrush, value); }

        private Brush _fridayBrush;
        public Brush FridayBrush { get => _fridayBrush; private set => SetProperty(ref _fridayBrush, value); }

        private Brush _saturdayBrush;
        public Brush SaturdayBrush { get => _saturdayBrush; private set => SetProperty(ref _saturdayBrush, value); }

        private Brush _holidayBrush;
        public Brush HolidayBrush { get => _holidayBrush; private set => SetProperty(ref _holidayBrush, value); }

        private Brush _specialday1Brush;
        public Brush Specialday1Brush { get => _specialday1Brush; private set => SetProperty(ref _specialday1Brush, value); }

        private Brush _specialday2Brush;
        public Brush Specialday2Brush { get => _specialday2Brush; private set => SetProperty(ref _specialday2Brush, value); }

        private Brush _specialday3Brush;
        public Brush Specialday3Brush { get => _specialday3Brush; private set => SetProperty(ref _specialday3Brush, value); }

        private Brush _todayBrush;
        public Brush TodayBrush { get => _todayBrush; private set => SetProperty(ref _todayBrush, value); }

        private Brush _mouseOverBrush;
        public Brush MouseOverBrush { get => _mouseOverBrush; private set => SetProperty(ref _mouseOverBrush, value); }

        public DayLabelStyleSettingViewModel()
        {
            SundayBrush = Red;
            MondayBrush = ControlTextBrush;
            TuesdayBrush = ControlTextBrush;
            WednesdayBrush = ControlTextBrush;
            ThursdayBrush = ControlTextBrush;
            FridayBrush = ControlTextBrush;
            SaturdayBrush = Blue;
            LoadSetting();
        }

        private static Brush ToBrush(string brushName, Brush defaultBrush)
        {
            PropertyInfo propInfo = typeof(Colors).GetProperty(brushName, typeof(Color));
            if (propInfo == null) return defaultBrush;
            var color = (Color)propInfo.GetValue(null);
            return new SolidColorBrush(color);
        }

        public void LoadSetting()
        {
            SettingFiles.Styles.ReadCsvFile(csvLine =>
            {
                if (csvLine.ColumnCount == 0 || string.IsNullOrEmpty(csvLine[0])) { return; }
                string dTypeName = csvLine[0].ToUpper();
                string brushName = csvLine.ColumnCount >= 2 ? csvLine[1] ?? "" : "";
                switch (dTypeName)
                {
                    case "SUNDAY": SundayBrush = ToBrush(brushName, Red); break;
                    case "MONDAY": MondayBrush = ToBrush(brushName, ControlTextBrush); break;
                    case "TUESDAY": TuesdayBrush = ToBrush(brushName, ControlTextBrush); break;
                    case "WEDNESDAY": WednesdayBrush = ToBrush(brushName, ControlTextBrush); break;
                    case "THURSDAY": ThursdayBrush = ToBrush(brushName, ControlTextBrush); break;
                    case "FRIDAY": FridayBrush = ToBrush(brushName, ControlTextBrush); break;
                    case "SATURDAY": SaturdayBrush = ToBrush(brushName, Blue); break;
                    case "HOLIDAY": HolidayBrush = ToBrush(brushName, Magenta); break;
                    case "SPECIALDAY1": Specialday1Brush = ToBrush(brushName, OrangeRed); break;
                    case "SPECIALDAY2": Specialday2Brush = ToBrush(brushName, OrangeRed); break;
                    case "SPECIALDAY3": Specialday3Brush = ToBrush(brushName, OrangeRed); break;
                    case "TODAY": TodayBrush = ToBrush(brushName, LightSkyBlue); break;
                    case "MOUSEOVER": MouseOverBrush = ToBrush(brushName, LightGreen); break;
                    default: break;
                }
            });
        }
    }
}
