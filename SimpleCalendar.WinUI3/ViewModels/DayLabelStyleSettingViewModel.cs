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
        //private readonly BrushConverter _brushConverter = new();
        public static readonly Brush Red;
        public static readonly Brush Blue;
        public static readonly Brush Magenta;
        public static readonly Brush OrangeRed;
        public static readonly Brush LightSkyBlue;
        public static readonly Brush LightGreen;
        public static readonly Brush ControlTextBrush;
        public static readonly Brush DefaultBackgroundBrush;

        static DayLabelStyleSettingViewModel()
        {
            Red = new SolidColorBrush(Colors.Red);
            Blue = new SolidColorBrush(Colors.Blue);
            Magenta = new SolidColorBrush(Colors.Magenta);
            OrangeRed = new SolidColorBrush(Colors.OrangeRed);
            LightSkyBlue = new SolidColorBrush(Colors.LightSkyBlue);
            LightGreen = new SolidColorBrush(Colors.LightGreen);
            ControlTextBrush = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            DefaultBackgroundBrush = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];
        }

        private Brush _sundayBrush = Red;
        public Brush SundayBrush { get => _sundayBrush; private set => SetProperty(ref _sundayBrush, value); }

        private Brush _mondayBrush = ControlTextBrush;
        public Brush MondayBrush { get => _mondayBrush; private set => SetProperty(ref _mondayBrush, value); }

        private Brush _tuesdayBrush = ControlTextBrush;
        public Brush TuesdayBrush { get => _tuesdayBrush; private set => SetProperty(ref _tuesdayBrush, value); }

        private Brush _wednesdayBrush = ControlTextBrush;
        public Brush WednesdayBrush { get => _wednesdayBrush; private set => SetProperty(ref _wednesdayBrush, value); }

        private Brush _thursdayBrush = ControlTextBrush;
        public Brush ThursdayBrush { get => _thursdayBrush; private set => SetProperty(ref _thursdayBrush, value); }

        private Brush _fridayBrush = ControlTextBrush;
        public Brush FridayBrush { get => _fridayBrush; private set => SetProperty(ref _fridayBrush, value); }

        private Brush _saturdayBrush = Blue;
        public Brush SaturdayBrush { get => _saturdayBrush; private set => SetProperty(ref _saturdayBrush, value); }

        private Brush _holidayBrush = Magenta;
        public Brush HolidayBrush { get => _holidayBrush; private set => SetProperty(ref _holidayBrush, value); }

        private Brush _specialday1Brush = OrangeRed;
        public Brush Specialday1Brush { get => _specialday1Brush; private set => SetProperty(ref _specialday1Brush, value); }

        private Brush _specialday2Brush = OrangeRed;
        public Brush Specialday2Brush { get => _specialday2Brush; private set => SetProperty(ref _specialday2Brush, value); }

        private Brush _specialday3Brush = OrangeRed;
        public Brush Specialday3Brush { get => _specialday3Brush; private set => SetProperty(ref _specialday3Brush, value); }

        private Brush _todayBrush = LightSkyBlue;
        public Brush TodayBrush { get => _todayBrush; private set => SetProperty(ref _todayBrush, value); }

        private Brush _mouseOverBrush = LightGreen;
        public Brush MouseOverBrush { get => _mouseOverBrush; private set => SetProperty(ref _mouseOverBrush, value); }

        public DayLabelStyleSettingViewModel()
        {
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
