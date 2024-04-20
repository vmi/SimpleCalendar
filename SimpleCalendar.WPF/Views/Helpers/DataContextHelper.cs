using System.Windows;

namespace SimpleCalendar.WPF.Views.Helpers
{
    public class DataContextHelper
    {
        public static Type GetDataContext(DependencyObject d) => (Type) d.GetValue(DataContextProperty);
        public static void SetDataContext(DependencyObject d, Type value) => d.SetValue(DataContextProperty, value);
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached(
            "DataContext",
            typeof(Type),
            typeof(DataContextHelper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable, OnDataContextChanged)
            );

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe && e.NewValue is Type type)
            {
                if ((fe.DataContext = ServiceRegistry.GetService(type)) == null)
                {
                    throw new ArgumentException($"Failed to get instance of type {type}");
                }
            }
            else
            {
                throw new ArgumentException($"{e.NewValue} is not type");
            }
        }
    }
}
