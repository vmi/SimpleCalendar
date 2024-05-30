using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Versioning;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SimpleCalendar.WinUI3.Utilities;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;
using static SimpleCalendar.WinUI3.Utilities.WindowHelper;

namespace SimpleCalendar.WinUI3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class MainWindow : WindowEx
    {
        //private const uint NotificationIconId = 1;

        //private const string ICON_NAME = "icon256.ico";
        //private const uint WMAPP_NOTIFYCALLBACK = NotifyIconManager.WM_APP + 1;

        //private readonly LocalConfigService _localConfigService;
        //private NotifyIconManager _notifyIconManager;
        //private HwndSource? _source;
        //private HwndSourceHook? _sourceHook;

        //private Window _help;
        //private Window _settingsView;

        //private bool _isNotificationIconAdded = false;

        private readonly WndProcRegistrar _wndProcRegistrar;
        private bool _isDragging = false;
        private PointInt32 _startWinPos;
        private System.Drawing.Point _startCsrPos;

        public MainWindow()
        {
            _wndProcRegistrar = new(this, WndProc, 0, 0);
            //_localConfigService = ServiceRegistry.GetService<LocalConfigService>()!;
            InitializeComponent();
            DisableTitleBar(this);

            // 動作検証用 (ここから)
            Activated += (_, e) => Debug.WriteLine($"[MainWindow:Activated] State={e.WindowActivationState}");
            Closed += (_, e) => Debug.WriteLine($"[MainWindow:Closed]");
            PositionChanged += (_, e) => Debug.WriteLine($"[MainWindow:PositionChanged] Position=({e.X}, {e.Y})");
            SizeChanged += (_, e) => Debug.WriteLine($"[MainWindow:SizeChanged] Size={Str(e.Size)}");
            PresenterChanged += (_, e) => Debug.WriteLine($"[MainWindow:PresenterChanged] Kind={e.Kind}");
            VisibilityChanged += (_, e) => Debug.WriteLine($"[MainWindow:VisibilityChanged] Visible={e.Visible}");
            WindowStateChanged += (_, e) => Debug.WriteLine($"[MainWindow:WindowStateChanged] State={e}");
            ZOrderChanged += (_, e) => Debug.WriteLine($"[MainWindow:ZOrderChanged] BelowWindowId={e.ZOrderBelowWindowId.Value}, Top={e.IsZOrderAtTop}, Bottom={e.IsZOrderAtBottom}");
            // 動作検証用 (ここまで)

            if (WindowContent is FrameworkElement elem)
            {
                // 動作検証用 (ここから)
                elem.SizeChanged += (sender, e) => { Debug.WriteLine($"[WindowContent:SizeChanged] PreviousSize={Str(e.PreviousSize)}, NewSize={Str(e.NewSize)}"); };
                // 動作検証用 (ここまで)

                elem.LayoutUpdated += Content_LayoutUpdated;
            }
            if (Content is UIElement c)
            {
                c.PointerPressed += (sender, e) =>
                {
                    Microsoft.UI.Input.PointerPointProperties props = e.GetCurrentPoint((UIElement)sender).Properties;
                    if (props.IsLeftButtonPressed)
                    {
                        ((UIElement)sender).CapturePointer(e.Pointer);
                        _isDragging = true;
                        _startWinPos = AppWindow.Position;
                        PInvoke.GetCursorPos(out _startCsrPos);
                    }
                };
                c.PointerMoved += (sender, e) =>
                {
                    Microsoft.UI.Input.PointerPointProperties props = e.GetCurrentPoint((UIElement)sender).Properties;
                    if (props.IsLeftButtonPressed && _isDragging)
                    {
                        PInvoke.GetCursorPos(out System.Drawing.Point curCsrPos);
                        int dX = curCsrPos.X - _startCsrPos.X;
                        int dY = curCsrPos.Y - _startCsrPos.Y;
                        PointInt32 pos = new PointInt32(_startWinPos.X + dX, _startWinPos.Y + dY);
                        AppWindow.Move(pos);
                    }
                };
                c.PointerReleased += (sender, e) =>
                {
                    Microsoft.UI.Input.PointerPointProperties props = e.GetCurrentPoint((UIElement)sender).Properties;
                    if (!props.IsLeftButtonPressed)
                    {
                        ((UIElement)sender).ReleasePointerCapture(e.Pointer);
                        _isDragging = false;
                    }
                };
            }
            Closed += MainWindow_Closed;

            //            this.VisibilityChanged += MainWindow_VisibilityChanged;
            //Loaded += MainWindow_Loaded; // メインウィンドウ初期化時に通知領域にアイコンを登録
            //Closed += MainWindow_Closed; // メインウィンドウクローズ時に通知領域からアイコンを削除
            //Activated += MainWindow_Activated; // アクティブ化時に「今日」を最新化する
            //CalendarRoot.Loaded += CalendarRoot_Loaded; // 画面に表示されている「月」の行数および列数を登録
            //MouseLeftButtonDown += MainWindow_MouseLeftButtonDown; // ウィンドウ自体を掴んで移動可能にする
            //LocationChanged += MainWindow_LocationChanged; // 画面外に移動できないようにする(暫定実装)
            ///MouseWheel += MainWindow_MouseWheel; // マウスホイールの回転で画面を前後に移動させる (XAMLのInputBindingsがマウスホイールの回転に対応していない)
            //StateChanged += MainWindow_StateChanged; // 最小化時にHide()を実行してタスクバーから見えなくする
        }

        private System.Drawing.Point _ptStart = new();

        private LRESULT WndProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            if (MsgToString.TryGetValue(uMsg, out string name))
            {
                Debug.WriteLine($"Message=[{name}]");
            }
            else
            {
                Debug.WriteLine($"Message=[{uMsg}]");
            }
            switch (uMsg)
            {
                case PInvoke.WM_LBUTTONDOWN:
                    // 左ボタンが押された場合
                    PInvoke.SetCapture(hWnd);
                    (_ptStart.X, _ptStart.Y) = GET_XY_LPARAM(lParam);
                    Debug.WriteLine($"[LBUTTONDOWN] ({_ptStart.X}, {_ptStart.Y})");
                    return default;

                case PInvoke.WM_MOUSEMOVE:
                    // マウスが移動した場合
                    if (PInvoke.GetCapture() == hWnd)
                    {
                        // ウィンドウをドラッグする
                        PInvoke.GetCursorPos(out System.Drawing.Point ptCurrent);
                        int x = ptCurrent.X - _ptStart.X;
                        int y = ptCurrent.Y - _ptStart.Y;
                        PInvoke.SetWindowPos(hWnd, HWND.Null,
                            x, y, 0, 0,
                            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
                        Debug.WriteLine($"[MOUSEMOVE] Current=({ptCurrent.X}, {ptCurrent.Y}) => SetWindowPos({x}, {y})");
                    }
                    return default;

                case PInvoke.WM_LBUTTONUP:
                    // 左ボタンが離された場合
                    BOOL result = PInvoke.ReleaseCapture();
                    Debug.WriteLine($"[LBUTTONUP] ReleaseCapture={result.Value}");
                    return default;

                default:
                    return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
            }
        }

        private void Content_LayoutUpdated(object sender, object e)
        {
            if (AppWindow == null) return;

            SizeInt32 size = AppWindow.Size;
            SizeInt32 cSize = AppWindow.ClientSize;
            FrameworkElement elem = (FrameworkElement)WindowContent;
            double w = elem.Width;
            double h = elem.Height;
            Vector2 aSize = elem.ActualSize;
            Windows.Foundation.Size dSize = elem.DesiredSize;
            Windows.Foundation.Size rSize = elem.RenderSize;
            Debug.WriteLine($"[WindowContent:LayoutUpdated] AppWindow[Size={Str(size)}, ClientSize={Str(cSize)}], WindowContent[Size=({w}, {h}), Actual=({aSize.X}, {aSize.Y}), Desired={Str(dSize)}, Render={Str(rSize)}]");
            if (dSize.Width > 0 && (dSize.Width != cSize.Width || dSize.Height != cSize.Height))
            {
                int wOffset = size.Width - cSize.Width;
                int hOffset = size.Height - cSize.Height;
                PointInt32 pos = AppWindow.Position;
                RectInt32 rect = new()
                {
                    X = pos.X,
                    Y = pos.Y,
                    Width = (int)(dSize.Width + wOffset),
                    Height = (int)(dSize.Height + hOffset)
                };
                AppWindow.MoveAndResize(rect);
                Debug.WriteLine($"[WindowContent:LayoutUpdated] Resize=({rect.Width}, {rect.Height})");
            }
        }

        //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //if (DesignerProperties.GetIsInDesignMode(this)) { return; }
        //if (CalendarRoot.DataContext is MainWindowViewModel vm)
        //{
        //    vm.SettingsViewModel.UpdateHolidaysCommand.Execute(this);
        //}
        //  RegisterNotifyIcon();
        //}

        //private void RegisterNotifyIcon()
        //{
        //    Icon icon = AssemblyHelper.Instance.LoadIcon(ICON_NAME);
        //    if (icon == null)
        //    {
        //        Debug.WriteLine($"Missing icon resouce: {ICON_NAME}");
        //        return;
        //    }
        //    //nint hwnd = new WindowInteropHelper(this).Handle;
        //    //_notifyIconManager = new(hwnd, id: NotificationIconId);
        //    //_source = HwndSource.FromHwnd(hwnd);
        //    //_sourceHook = new HwndSourceHook(WndProc);
        //    //_source.AddHook(_sourceHook);
        //    //_notifyIconManager.Select = NotifyIcon_Select;
        //    //_isNotificationIconAdded = _notifyIconManager.Add(icon, tip: "Simple Calendar", callbackMessage: WMAPP_NOTIFYCALLBACK);
        //}

        //private void NotifyIcon_Select(nint hwnd)
        //{
        //    switch (WindowState)
        //    {
        //        case WindowState.Normal:
        //            WindowState = WindowState.Minimized;
        //            break;
        //        case WindowState.Minimized:
        //            if (DataContext is MainWindowViewModel curMon)
        //            {
        //                curMon.UpdateTodayCommand.Execute(null);
        //            }
        //            Show();
        //            WindowState = WindowState.Normal;
        //            break;
        //    }
        //}

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            _wndProcRegistrar.Dispose();
            //if (_notifyIconManager != null)
            //{
            //    _notifyIconManager.Delete();
            //    _source!.RemoveHook(_sourceHook);
            //    _notifyIconManager = null;
            //    _source = null;
            //    _sourceHook = null;
            //}
        }

        //private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        //{
        //    switch ((uint)msg)
        //    {
        //        case WMAPP_NOTIFYCALLBACK:
        //            //_notifyIconManager?.NotifyCallback(hwnd, wParam, lParam);
        //            break;
        //    }
        //    return nint.Zero;
        //}

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            //if (WindowState == WindowState.Minimized && _isNotificationIconAdded)
            //{
            //    Hide();
            //}
        }

        /*private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
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
        }*/

        //private void MainWindow_LocationChanged(object? sender, EventArgs e)
        //{
        //    switch (_localConfigService.LoadStatus)
        //    {
        //        case LoadStatus.NotLoaded:
        //            _localConfigService.Load(entry =>
        //            {
        //                Left = entry.Left;
        //                Top = entry.Top;
        //                Debug.WriteLine($"Loaded: ({Left}, {Top})");
        //            });
        //            break;
        //        case LoadStatus.Loading:
        //            // no operation.
        //            break;
        //        case LoadStatus.Loaded:
        //            AdjustWindowPosition();
        //            _localConfigService.Save(Left, Top);
        //            break;
        //    }
        //}

        //private void AdjustWindowPosition()
        //{
        //    //double vl = SystemParameters.VirtualScreenLeft;
        //    //double vt = SystemParameters.VirtualScreenTop;
        //    //double vw = SystemParameters.VirtualScreenWidth;
        //    //double vh = SystemParameters.VirtualScreenHeight;
        //    //Rect wa = SystemParameters.WorkArea;
        //    //double pw = SystemParameters.PrimaryScreenWidth;
        //    //double ph = SystemParameters.PrimaryScreenHeight;
        //    //double leftMin = vl < 0 ? vl : wa.Left;
        //    //double rightMax = vl + vw > pw ? vl + vw : wa.Right;
        //    //double topMin = vt < 0 ? vt : wa.Top;
        //    //double bottomMax = vt + vh > ph ? vt + vh : wa.Bottom;
        //    //if (Left < leftMin)
        //    //{
        //    //    Left = leftMin;
        //    //}
        //    //else if (Left + ActualWidth > rightMax)
        //    //{
        //    //    Left = rightMax - ActualWidth;
        //    //}
        //    //if (Top < topMin)
        //    //{
        //    //    Top = topMin;
        //    //}
        //    //else if (Top + ActualHeight > bottomMax)
        //    //{
        //    //    Top = bottomMax - ActualHeight;
        //    //}
        //}

        //private void CalendarRoot_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //if (sender is Grid grid && DataContext is MainWindowViewModel curMon)
        //    //{
        //    //    curMon.RowCount = grid.ColumnDefinitions.Count;
        //    //    curMon.ColumnCount = grid.ColumnDefinitions.Count;
        //    //}
        //}

        //private void MainWindow_Activated(object sender, EventArgs e)
        //{
        //    //if (CalendarRoot.DataContext is MainWindowViewModel curMon)
        //    //{
        //    //    curMon.UpdateTodayCommand.Execute(null);
        //    //}
        //}

        //private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    //if (e.ChangedButton == MouseButton.Left)
        //    //{
        //    //    DragMove();
        //    //}
        //}

        //private void Exit_Click(object sender, RoutedEventArgs e)
        //{
        //    Application.Current.Exit();
        //}

        //private void Help_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (_help == null || !_help.IsLoaded)
        //    //{
        //    //    _help = new Help();
        //    //}
        //    //_help.Show();
        //}

        //private void Settings_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (_settingsView == null || !_settingsView.IsLoaded)
        //    //{
        //    //    _settingsView = new SettingsView();
        //    //}
        //    //_settingsView.Show();
        //}
    }
}
