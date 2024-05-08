using System.Windows;
using System.Windows.Controls;
using SimpleCalendar.WPF.ViewModels;

namespace SimpleCalendar.WPF.Views
{
    public partial class CalendarMonthView : UserControl
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
