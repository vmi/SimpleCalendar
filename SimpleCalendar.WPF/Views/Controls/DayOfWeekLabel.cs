using System.Windows;
using System.Windows.Controls;

namespace SimpleCalendar.WPF.Views.Controls
{
    public class DayOfWeekLabel : Label
    {
        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register(
            nameof(DayOfWeek),
            typeof(string),
            typeof(DayOfWeekLabel),
            new UIPropertyMetadata(""));

        public string DayOfWeek { get => (string) GetValue(DayOfWeekProperty); set => SetValue(DayOfWeekProperty, value); }
    }
}
