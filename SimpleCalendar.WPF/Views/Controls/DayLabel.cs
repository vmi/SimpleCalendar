using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.ViewModels;

namespace SimpleCalendar.WPF.Views.Controls
{
    public class DayLabel : Label
    {
        public static readonly DependencyProperty DayItemProperty = DependencyProperty.Register(
            nameof(DayItem),
            typeof(DayItem),
            typeof(DayLabel),
            new UIPropertyMetadata(OnDayItemPropertyChanged));

        public DayItem DayItem { get => (DayItem)GetValue(DayItemProperty); set => SetValue(DayItemProperty, value); }

        private static void OnDayItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DayLabel obj && e.NewValue is DayItem dayItem)
            {
                obj.Content = dayItem.DayString;
                obj.DayType = dayItem.DayType;
                obj.IsDayTypeEmpty = dayItem.DayType == DayType.EMPTY;
                if (string.IsNullOrEmpty(dayItem.Label))
                {
                    obj.ToolTip = null;
                }
                else
                {
                    obj.ToolTip = dayItem.Label;
                    ToolTipService.SetInitialShowDelay(obj, 0);
                }
            }
        }

        private static readonly DependencyPropertyKey DayTypePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(DayType),
            typeof(DayType),
            typeof(DayLabel),
            new UIPropertyMetadata());

        public static readonly DependencyProperty DayTypeProperty = DayTypePropertyKey.DependencyProperty;

        public DayType DayType { get => (DayType)GetValue(DayTypeProperty); private set => SetValue(DayTypePropertyKey, value); }

        private static readonly DependencyPropertyKey IsDayTypeEmptyPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsDayTypeEmpty),
            typeof(bool),
            typeof(DayLabel),
            new UIPropertyMetadata());

        public static readonly DependencyProperty IsDayTypeEmptyProperty = IsDayTypeEmptyPropertyKey.DependencyProperty;

        public bool IsDayTypeEmpty { get => (bool)GetValue(IsDayTypeEmptyProperty); private set => SetValue(IsDayTypeEmptyPropertyKey, value); }

        public static readonly DependencyProperty IsTodayProperty = DependencyProperty.Register(
            nameof(IsToday),
            typeof(bool),
            typeof(DayLabel),
            new UIPropertyMetadata(false));

        public bool IsToday { get => (bool)GetValue(IsTodayProperty); set => SetValue(IsTodayProperty, value); }

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
                MainWindowViewModel curMon = calMon.CurrentMonth;
                multiBinding.Bindings.Add(new Binding("Today") { Source = curMon });
                multiBinding.Bindings.Add(new Binding("YearMonth") { Source = calMon });
                multiBinding.Bindings.Add(new Binding(nameof(DayItem)) { Source = this });
                SetBinding(IsTodayProperty, multiBinding);
            }
        }
    }
}
