using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SimpleCalendar.WinUI3.Views.Helpers
{
    public class GridHelper
    {
        public static Thickness GetMargin(DependencyObject obj) => (Thickness)obj.GetValue(MarginProperty);
        public static void SetMargin(DependencyObject obj, Thickness value) { obj.SetValue(MarginProperty, value); }
        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
            "Margin",
            typeof(Thickness),
            typeof(GridHelper),
            PropertyMetadata.Create(new Thickness(), MarginChangedCallback));

        public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Grid grid)
            {
                grid.Loaded += OnMarginPropertyChanged;
                grid.
                var metadata = MarginProperty.GetMetadata(typeof(Grid));
                if (metadata != null)
                {
                    metadata.
                }
                var desc = DependencyPropertyDescriptor.FromProperty(MarginProperty, typeof(Grid));
                desc.AddValueChanged(grid, OnMarginPropertyChanged);
            }
        }

        private static void OnMarginPropertyChanged(object? sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                Thickness margin = GetMargin(grid);
                var rMargin = new Thickness(margin.Left, margin.Top, margin.Left, margin.Bottom); // margin for right edge
                var bMargin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Top); // margin for bottom edge
                var rbMargin = new Thickness(margin.Left, margin.Top, margin.Left, margin.Top); // margin for right bottom corner
                int rmax = grid.RowDefinitions.Count - 1;
                int cmax = grid.ColumnDefinitions.Count - 1;
                foreach (Control child in grid.Children.OfType<Control>())
                {
                    int r = Grid.GetRow(child);
                    int c = Grid.GetColumn(child);
                    if (r < rmax)
                    {
                        // not bottom
                        if (c < cmax)
                        {
                            // not right edge
                            child.Margin = margin;
                        }
                        else
                        {
                            // right edge
                            child.Margin = rMargin;
                        }
                    }
                    else
                    {
                        // bottom
                        if (c < cmax)
                        {
                            // bottom edge, not right bottom corner
                            child.Margin = bMargin;
                        }
                        else
                        {
                            // right bottom corner
                            child.Margin = rbMargin;
                        }
                    }
                }
            }
        }
    }
}
