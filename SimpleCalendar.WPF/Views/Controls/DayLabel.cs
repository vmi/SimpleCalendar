using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SimpleCalendar.WPF.Views.Controls
{
    public class DayLabel : Label
    {
        public static readonly DependencyProperty DayItemProperty = DependencyProperty.Register(
            nameof(DayItem),
            typeof(DayItem),
            typeof(DayLabel),
            new UIPropertyMetadata(OnDayItemPropertyChanged));

        public DayItem DayItem { get => (DayItem) GetValue(DayItemProperty); set => SetValue(DayItemProperty, value); }

        private static void OnDayItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DayLabel obj && e.NewValue is DayItem dayItem)
            {
                obj.Content = dayItem.DayString;
                obj.DayType = dayItem.DayType.ToString();
                if (!string.IsNullOrEmpty(dayItem.Label))
                {
                    obj.ToolTip = dayItem.Label;
                }

            }
        }

        private static readonly DependencyPropertyKey DayTypePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(DayType),
            typeof(string),
            typeof(DayLabel),
            new UIPropertyMetadata());

        public static readonly DependencyProperty DayTypeProperty = DayTypePropertyKey.DependencyProperty;

        public string DayType { get => (string) GetValue(DayTypeProperty); private set => SetValue(DayTypePropertyKey, value); }

        public static readonly DependencyProperty IsTodayProperty = DependencyProperty.Register(
            nameof(IsToday),
            typeof(bool),
            typeof(DayLabel),
            new UIPropertyMetadata(false));

        public bool IsToday { get => (bool) GetValue(IsTodayProperty); set => SetValue(IsTodayProperty, value); }

        private class IsTodayConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values[0] is DateOnly today && values[1] is YearMonth yearMonth && values[2] is DayItem dayItem)
                {
                    return today.Year == yearMonth.Year && today.Month == yearMonth.Month && today.Day == dayItem.Day;
                }
                else
                {
                    return false;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private readonly IsTodayConverter isTodayConverter = new();

        public DayLabel() : base()
        {
            Loaded += DayLabel_Loaded;
        }

        private void DayLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalendarMonthViewModel calMon)
            {
                var multiBinding = new MultiBinding()
                {
                    Converter = isTodayConverter
                };
                var curMon = calMon.CurrentMonthViewModel;
                multiBinding.Bindings.Add(new Binding("Today") { Source = curMon });
                multiBinding.Bindings.Add(new Binding("YearMonth") { Source = calMon });
                multiBinding.Bindings.Add(new Binding(nameof(DayItem)) { RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.Self } });
                SetBinding(IsTodayProperty, multiBinding);
            }
        }
    }
}
