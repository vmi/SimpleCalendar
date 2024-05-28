using System.Runtime.Versioning;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using SimpleCalendar.WinUI3.Models;
using SimpleCalendar.WinUI3.ViewModels;

namespace SimpleCalendar.WinUI3.Views.Controls
{
    [SupportedOSPlatform("windows")]
    public class DayOfWeekLabel : Label
    {
        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register(
            nameof(DayOfWeek),
            typeof(DayType),
            typeof(DayOfWeekLabel),
            PropertyMetadata.Create(DayType.EMPTY, OnDayOfWeekPropertyChanged));

        public DayType DayOfWeek { get => (DayType)GetValue(DayOfWeekProperty); set => SetValue(DayOfWeekProperty, value); }

        private static void OnDayOfWeekPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DayOfWeekLabel obj)
            {
                DayType dayType = (DayType)e.NewValue;
                string propName = "";
                switch (dayType)
                {
                    case DayType.SUNDAY: obj.Text = "日"; propName = nameof(DayLabelStyleSettingViewModel.SundayBrush); break;
                    case DayType.MONDAY: obj.Text = "月"; propName = nameof(DayLabelStyleSettingViewModel.MondayBrush); break;
                    case DayType.TUESDAY: obj.Text = "火"; propName = nameof(DayLabelStyleSettingViewModel.TuesdayBrush); break;
                    case DayType.WEDNESDAY: obj.Text = "水"; propName = nameof(DayLabelStyleSettingViewModel.WednesdayBrush); break;
                    case DayType.THURSDAY: obj.Text = "木"; propName = nameof(DayLabelStyleSettingViewModel.ThursdayBrush); break;
                    case DayType.FRIDAY: obj.Text = "金"; propName = nameof(DayLabelStyleSettingViewModel.FridayBrush); break;
                    case DayType.SATURDAY: obj.Text = "土"; propName = nameof(DayLabelStyleSettingViewModel.SaturdayBrush); break;
                    default: obj.Text = ""; propName = null; break;
                }
                if (!string.IsNullOrEmpty(propName))
                {
                    Binding binding = new() { Path = new PropertyPath($"DayLabelStyleSetting.{propName}") };
                    obj.SetBinding(ForegroundProperty, binding);
                }
            }
        }
    }
}
