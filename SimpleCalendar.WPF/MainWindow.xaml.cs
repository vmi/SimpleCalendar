using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using NotifyIcon;
using SimpleCalendar.WPF.Services;
using SimpleCalendar.WPF.Utilities;
using SimpleCalendar.WPF.ViewModels;
using SimpleCalendar.WPF.Views;

namespace SimpleCalendar.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const uint NotificationIconId = 1;

        private const string ICON_NAME = "calendar128.ico";
        private const uint WMAPP_NOTIFYCALLBACK = NotifyIconManager.WM_APP + 1;

        private readonly LocalConfigService _localConfigService;
        private NotifyIconManager? _notifyIconManager;
        private HwndSource? _source;
        private HwndSourceHook? _sourceHook;

        private Window? _help;
        private Window? _settingsView;

        private bool _isNotificationIconAdded = false;

        public MainWindow()
        {
            _localConfigService = ServiceRegistry.GetService<LocalConfigService>()!;
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
            if (DataContext is MainWindowViewModel vm)
            {
                vm.SettingsViewModel.UpdateHolidaysCommand.Execute(this);
            }
            RegisterNotifyIcon();
        }

        private void RegisterNotifyIcon()
        {
            Icon? icon = AssemblyHelper.Instance.LoadIcon(ICON_NAME);
            if (icon == null)
            {
                Debug.WriteLine($"Missing icon resouce: {ICON_NAME}");
                return;
            }
            nint hwnd = new WindowInteropHelper(this).Handle;
            _notifyIconManager = new(hwnd, id: NotificationIconId);
            _source = HwndSource.FromHwnd(hwnd);
            _sourceHook = new HwndSourceHook(WndProc);
            _source.AddHook(_sourceHook);
            _notifyIconManager.Select = NotifyIcon_Select;
            _isNotificationIconAdded = _notifyIconManager.Add(icon, tip: "Simple Calendar", callbackMessage: WMAPP_NOTIFYCALLBACK);
        }

        private void NotifyIcon_Select(nint hwnd)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Minimized;
                    break;
                case WindowState.Minimized:
                    if (DataContext is MainWindowViewModel curMon)
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
            if (WindowState == WindowState.Minimized && _isNotificationIconAdded)
            {
                Hide();
            }
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is MainWindowViewModel curMon)
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
            switch (_localConfigService.LoadStatus)
            {
                case LoadStatus.NotLoaded:
                    _localConfigService.Load(entry =>
                    {
                        Left = entry.Left;
                        Top = entry.Top;
                        Debug.WriteLine($"Loaded: ({Left}, {Top})");
                    });
                    break;
                case LoadStatus.Loading:
                    // no operation.
                    break;
                case LoadStatus.Loaded:
                    AdjustWindowPosition();
                    _localConfigService.Save(Left, Top);
                    break;
            }
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
            if (sender is Grid grid && DataContext is MainWindowViewModel curMon)
            {
                curMon.RowCount = grid.ColumnDefinitions.Count;
                curMon.ColumnCount = grid.ColumnDefinitions.Count;
            }
        }

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel curMon)
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

        private void Exit_Click(object sender, RoutedEventArgs e)
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsView == null || !_settingsView.IsLoaded)
            {
                _settingsView = new SettingsView();
            }
            _settingsView.Show();
        }
    }
}
