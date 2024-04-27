using SimpleCalendar.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SimpleCalendar.WPF.Views
{
    public partial class CalendarMonthView : UserControl
    {
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset),
            typeof(int),
            typeof(CalendarMonthView),
            new PropertyMetadata(0, OnOffsetPropertyChanged));

        public int Offset { get => (int) GetValue(OffsetProperty); set => SetValue(OffsetProperty, value); }

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
            Loaded += CalendarMonthView_Loaded;
        }

        private void CalendarMonthView_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            var dlStyles = ((CalendarMonthViewModel) DataContext).DayLabelStyleViewModel.DayLabelStyles;
            var baseStyle = new Style();
            var lStyle = new Style
            {
                BasedOn = (Style) Resources["DayLabel"],
                TargetType = typeof(DayOfWeekLabel)
            };
            var dlStyle = new Style
            {
                BasedOn = (Style) Resources["DayLabel"],
                TargetType = typeof(DayLabel)
            };
            foreach (var (k, v) in dlStyles)
            {
                var dayType = k.ToUpper();
                Trigger? lsTrigger = null;
                switch (dayType)
                {
                    case "SUNDAY":
                    case "MONDAY":
                    case "TUESDAY":
                    case "WEDNESDAY":
                    case "THURSDAY":
                    case "FRIDAY":
                    case "SATURDAY":
                        lsTrigger = new Trigger()
                        {
                            Property = DayOfWeekLabel.DayOfWeekProperty,
                            Value = dayType
                        };
                        lStyle.Triggers.Add(lsTrigger);
                        break;
                }
                var dlsTrigger = new Trigger();
                dlStyle.Triggers.Add(dlsTrigger);
                switch (dayType)
                {
                    case "TODAY":
                        dlsTrigger.Property = DayLabel.IsTodayProperty;
                        dlsTrigger.Value = true;
                        break;

                    case "FOCUS":
                        dlsTrigger.Property = DayLabel.IsMouseOverProperty;
                        dlsTrigger.Value = true;
                        break;

                    default:
                        dlsTrigger.Property = DayLabel.DayTypeProperty;
                        dlsTrigger.Value = k;
                        break;
                }
                if (v.Foreground != null)
                {
                    var fg = new Setter
                    {
                        Property = Label.ForegroundProperty,
                        Value = v.Foreground
                    };
                    lsTrigger?.Setters.Add(fg);
                    dlsTrigger.Setters.Add(fg);
                }
                if (v.Background != null)
                {
                    var bg = new Setter
                    {
                        Property = Label.BackgroundProperty,
                        Value = v.Background
                    };
                    lsTrigger?.Setters.Add(bg);
                    dlsTrigger.Setters.Add(bg);
                }
            }
            baseStyle.Resources.Add(lStyle.TargetType, lStyle);
            baseStyle.Resources.Add(dlStyle.TargetType, dlStyle);
            DaysGrid.Style = baseStyle;
            */
        }
    }
}
