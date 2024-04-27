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
                obj.Content = (DayType) e.NewValue switch
                {
                    DayType.SUNDAY => "日",// TODO 国際化を検討
                    DayType.MONDAY => "月",
                    DayType.TUESDAY => "火",
                    DayType.WEDNESDAY => "水",
                    DayType.THURSDAY => "木",
                    DayType.FRIDAY => "金",
                    DayType.SATURDAY => "土",
                    _ => "",
                };
            }
        }
    }
}
