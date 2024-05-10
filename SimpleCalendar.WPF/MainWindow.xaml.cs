using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using NotifyIcon;
using SimpleCalendar.WPF.Utilities;
using SimpleCalendar.WPF.ViewModels;
using SimpleCalendar.WPF.Views;
//using static System.Net.Mime.MediaTypeNames;

namespace SimpleCalendar.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Guid s_guid = Guid.Parse("{060F633E-8DB1-46D0-A1A4-9DCF5F9DB46C}");

        private const string ICON_NAME = "icon256.ico";
        private const uint WMAPP_NOTIFYCALLBACK = NotifyIconManager.WM_APP + 1;

        private NotifyIconManager? _notifyIconManager;
        private HwndSource? _source;
        private HwndSourceHook? _sourceHook;

        private Window? _help;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded; // メインウィンドウ初期化時に通知領域にアイコンを登録
            Closed += MainWindow_Closed; // メインウィンドウクローズ時に通知領域からアイコンを削除
            Activated += MainWindow_Activated; // アクティブ化時に「今日」を最新化する
            CalendarRoot.Loaded += CalendarRoot_Loaded; // 画面に表示されている「月」の行数および列数を登録
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown; // ウィンドウ自体を掴んで移動可能にする
            LocationChanged += MainWindow_LocationChanged; // 画面外に移動できないようにする(暫定実装)
            MouseWheel += MainWindow_MouseWheel; // マウスホイールの回転で画面を前後に移動させる (XAMLのInputBindingsがマウスホイールの回転に対応していない)
            StateChanged += MainWindow_StateChanged; // 最小化時にHide()を実行してタスクバーから見えなくする
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }
            Icon? icon = AssemblyHelper.Instance.LoadIcon(ICON_NAME);
            if (icon == null)
            {
                Debug.WriteLine($"Missing icon resouce: {ICON_NAME}");
                return;
            }
            nint hwnd = new WindowInteropHelper(this).Handle;
            _notifyIconManager = new(hwnd, guid: s_guid);
            _source = HwndSource.FromHwnd(hwnd);
            _sourceHook = new HwndSourceHook(WndProc);
            _source.AddHook(_sourceHook);
            _notifyIconManager.Select = NotifyIcon_Select;
            _notifyIconManager.Add(icon, tip: "Simple Calendar", callbackMessage: WMAPP_NOTIFYCALLBACK);
        }

        private void NotifyIcon_Select(nint hwnd)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Minimized;
                    break;
                case WindowState.Minimized:
                    if (DataContext is CurrentMonthViewModel curMon)
                    {
                        curMon.UpdateTodayCommand.Execute(null);
                    }
                    Show();
                    WindowState = WindowState.Normal;
                    break;
            }
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            if (_notifyIconManager != null)
            {
                _notifyIconManager.Delete();
                _source!.RemoveHook(_sourceHook);
                _notifyIconManager = null;
                _source = null;
                _sourceHook = null;
            }
        }

        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            switch ((uint)msg)
            {
                case WMAPP_NOTIFYCALLBACK:
                    _notifyIconManager?.NotifyCallback(hwnd, wParam, lParam);
                    break;
            }
            return nint.Zero;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is CurrentMonthViewModel curMon)
            {
                int delta = e.Delta;
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
            double vl = SystemParameters.VirtualScreenLeft;
            double vt = SystemParameters.VirtualScreenTop;
            double vw = SystemParameters.VirtualScreenWidth;
            double vh = SystemParameters.VirtualScreenHeight;
            Rect wa = SystemParameters.WorkArea;
            double pw = SystemParameters.PrimaryScreenWidth;
            double ph = SystemParameters.PrimaryScreenHeight;
            double leftMin = vl < 0 ? vl : wa.Left;
            double rightMax = vl + vw > pw ? vl + vw : wa.Right;
            double topMin = vt < 0 ? vt : wa.Top;
            double bottomMax = vt + vh > ph ? vt + vh : wa.Bottom;
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

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            if (DataContext is CurrentMonthViewModel curMon)
            {
                curMon.UpdateTodayCommand.Execute(null);
            }
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if (_help == null || !_help.IsLoaded)
            {
                _help = new Help();
            }
            _help.Show();
        }
    }
}
