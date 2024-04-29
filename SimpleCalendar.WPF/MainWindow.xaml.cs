using NotifyIcon;
using SimpleCalendar.WPF.Utilities;
using SimpleCalendar.WPF.ViewModels;
using SimpleCalendar.WPF.Views.Controls;
using SimpleCalendar.WPF.Views.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
//using static System.Net.Mime.MediaTypeNames;

namespace SimpleCalendar.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ICON_NAME = "icon256.ico";
        private const uint WMAPP_NOTIFYCALLBACK = NotifyIconManager.WM_APP + 1;

        private NotifyIconManager? notifyIconManager;
        private HwndSource? source;
        private HwndSourceHook? sourceHook;
        private readonly Guid guid = Guid.NewGuid();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded; // メインウィンドウ初期化時に通知領域にアイコンを登録
            Closed += MainWindow_Closed; // メインウィンドウクローズ時に通知領域からアイコンを削除
            CalendarRoot.Loaded += CalendarRoot_Loaded; // 画面に表示されている「月」の行数および列数を登録
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown; // ウィンドウ自体を掴んで移動可能にする
            LocationChanged += MainWindow_LocationChanged; // 画面外に移動できないようにする(暫定実装)
            MouseWheel += MainWindow_MouseWheel; // マウスホイールの回転で画面を前後に移動させる (XAMLのInputBindingsがマウスホイールの回転に対応していない)
            StateChanged += MainWindow_StateChanged; // 仮実装
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }
            var icon = AssemblyHelper.Instance.LoadIcon(ICON_NAME);
            if (icon == null)
            {
                Debug.WriteLine($"Missing icon resouce: {ICON_NAME}");
                return;
            }
            var hwnd = new WindowInteropHelper(this).Handle;
            notifyIconManager = new(hwnd, guid: guid);
            source = HwndSource.FromHwnd(hwnd);
            sourceHook = new HwndSourceHook(WndProc);
            source.AddHook(sourceHook);
            notifyIconManager.Select = NotifyIcon_Select;
            notifyIconManager.Add(icon, tip: "Simple Calendar", callbackMessage: WMAPP_NOTIFYCALLBACK);
        }

        private void NotifyIcon_Select(nint hwnd)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Minimized;
                    Hide();
                    break;
                case WindowState.Minimized:
                    Show();
                    WindowState = WindowState.Normal;
                    break;
            }
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            if (notifyIconManager != null)
            {
                notifyIconManager.Delete();
                source!.RemoveHook(sourceHook);
                notifyIconManager = null;
                source = null;
                sourceHook = null;
            }
        }

        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            switch ((uint) msg)
            {
                case WMAPP_NOTIFYCALLBACK:
                    notifyIconManager?.NotifyCallback(hwnd, wParam, lParam);
                    break;
            }
            return nint.Zero;
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
                if (delta < 0)
                {
                    curMon.NextLineCommand.Execute(null);
                }
                else if (delta > 0)
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
