using SimpleCalendar.WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace SimpleCalendar.WPF.Views.Controls
{
    public class DayOfWeekLabel : Label
    {
        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register(
            nameof(DayOfWeek),
            typeof(DayType),
            typeof(DayOfWeekLabel),
            new UIPropertyMetadata(DayType.EMPTY, OnDayOfWeekPropertyChanged));

        public DayType DayOfWeek { get => (DayType) GetValue(DayOfWeekProperty); set => SetValue(DayOfWeekProperty, value); }
        private static void OnDayOfWeekPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DayOfWeekLabel obj)
            {
                switch ((DayType) e.NewValue)
                {
                    case DayType.SUNDAY:
                        obj.Content = "日"; // TODO 国際化を検討
                        break;
                    case DayType.MONDAY:
                        obj.Content = "月";
                        break;
                    case DayType.TUESDAY:
                        obj.Content = "火";
                        break;
                    case DayType.WEDNESDAY:
                        obj.Content = "水";
                        break;
                    case DayType.THURSDAY:
                        obj.Content = "木";
                        break;
                    case DayType.FRIDAY:
                        obj.Content = "金";
                        break;
                    case DayType.SATURDAY:
                        obj.Content = "土";
                        break;
                    default:
                        obj.Content = "";
                        break;
                }
            }
        }
    }
}
