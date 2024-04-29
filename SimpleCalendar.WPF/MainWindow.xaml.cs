using SimpleCalendar.WPF.ViewModels;
using SimpleCalendar.WPF.Views.Controls;
using SimpleCalendar.WPF.Views.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleCalendar.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            CalendarRoot.Loaded += CalendarRoot_Loaded;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            LocationChanged += MainWindow_LocationChanged;
            MouseWheel += MainWindow_MouseWheel;
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine(WindowState);
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is CurrentMonthViewModel curMon)
            {
                var delta = e.Delta;
                if (delta < 0 )
                {
                    curMon.NextLineCommand.Execute(null);
                }
                else if (delta > 0 )
                {
                    curMon.PrevLineCommand.Execute(null);
                }
            }
        }

        private void MainWindow_LocationChanged(object? sender, EventArgs e)
        {
            AdjustWindowPosition();
        }

        private void AdjustWindowPosition()
        {
            var vl = SystemParameters.VirtualScreenLeft;
            var vt = SystemParameters.VirtualScreenTop;
            var vw = SystemParameters.VirtualScreenWidth;
            var vh = SystemParameters.VirtualScreenHeight;
            var wa = SystemParameters.WorkArea;
            var pw = SystemParameters.PrimaryScreenWidth;
            var ph = SystemParameters.PrimaryScreenHeight;
            var leftMin = vl < 0 ? vl : wa.Left;
            var rightMax = vl + vw > pw ? vl + vw : wa.Right;
            var topMin = vt < 0 ? vt : wa.Top;
            var bottomMax = vt + vh > ph ? vt + vh : wa.Bottom;
            if (Left < leftMin)
            {
                Left = leftMin;
            }
            else if (Left + ActualWidth > rightMax)
            {
                Left = rightMax - ActualWidth;
            }
            if (Top < topMin)
            {
                Top = topMin;
            }
            else if (Top + ActualHeight > bottomMax)
            {
                Top = bottomMax - ActualHeight;
            }
        }

        private void CalendarRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid && DataContext is CurrentMonthViewModel curMon)
            {
                curMon.RowCount = grid.ColumnDefinitions.Count;
                curMon.ColumnCount = grid.ColumnDefinitions.Count;
            }
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            if (DataContext is CurrentMonthViewModel curMon)
            {
                curMon.UpdateTodayCommand.Execute(null);
            }
            AdjustWindowPosition(); // adjust the window position if the window is outside of the screen due to a change in the display connection.
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
