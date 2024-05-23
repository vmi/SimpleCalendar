using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleCalendar.WinUI3.ViewModels;

namespace SimpleCalendar.WinUI3.Views
{
    public sealed partial class CalendarMonthView : UserControl
    {
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset),
            typeof(int),
            typeof(CalendarMonthView),
            new PropertyMetadata(0, OnOffsetPropertyChanged));

        public int Offset { get => (int)GetValue(OffsetProperty); set => SetValue(OffsetProperty, value); }

        private static void OnOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CalendarMonthView obj && obj.DataContext is CalendarMonthViewModel vm)
            {
                vm.Offset = obj.Offset;
            }
        }

        public CalendarMonthView()
        {
            InitializeComponent();
        }
    }
}
