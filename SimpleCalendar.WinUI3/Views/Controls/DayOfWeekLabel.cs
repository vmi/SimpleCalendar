using System.Runtime.Versioning;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleCalendar.WinUI3.Models;

namespace SimpleCalendar.WinUI3.Views.Controls
{
    [SupportedOSPlatform("windows")]
    public class DayOfWeekLabel : Control
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
                obj.Content = (DayType)e.NewValue switch
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

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(DayType),
            typeof(DayOfWeekLabel),
            PropertyMetadata.Create(""));

        public string Content { get => (string)GetValue(ContentProperty); set => SetValue(ContentProperty, value); }
    }
}
