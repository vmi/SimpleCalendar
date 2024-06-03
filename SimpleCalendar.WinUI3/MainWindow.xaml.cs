using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using SimpleCalendar.WinUI3.Services;
using SimpleCalendar.WinUI3.Utilities;
using SimpleCalendar.WinUI3.ViewModels;
using Windows.Graphics;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
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

        private const string ICON_NAME = "icon256.ico";
        //private const uint WMAPP_NOTIFYCALLBACK = NotifyIconManager.WM_APP + 1;

        //private NotifyIconManager _notifyIconManager;
        //private HwndSource? _source;
        //private HwndSourceHook? _sourceHook;

        //private bool _isNotificationIconAdded = false;

        private readonly LocalConfigService _localConfigService;
        private readonly WndProcRegistrar _wndProcRegistrar;
        private bool _isInitialized = false;
        private bool _isWindowSizeAdjusted = false;
        private bool _isDragging = false;
        private PointInt32 _startWinPos;
        private System.Drawing.Point _startCsrPos;
        private readonly DisplayAreas _displayAreas = ServiceRegistry.GetService<DisplayAreas>();

        private System.Drawing.Icon _icon;

        private WindowEx _help;
        private WindowEx _settingsView;

        public MainWindow()
        {
            _localConfigService = ServiceRegistry.GetService<LocalConfigService>()!;

            _wndProcRegistrar = new(this, WndProc, 0, 0);

            InitializeComponent();

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

            // 外見等の設定
            DisableTitleBar(this);
            _icon = AssemblyHelper.Instance.LoadIcon(ICON_NAME);
            if (_icon != null)
            {
                // exeファイルに設定されているアイコンがタスクバーに反映されないので、明示的に設定
                IconId iconId = Win32Interop.GetIconIdFromIcon(_icon.Handle);
                this.SetIcon(iconId);
            }

            // Windowに紐付く各種イベントハンドラの登録
            Activated += MainWindow_Activated; // メインウィンドウ初期化時に通知領域にアイコンを登録(未実装) & アクティブ化時に「今日」を最新化する
            Closed += MainWindow_Closed; // メインウィンドウクローズ時に通知領域からアイコンを削除(未実装)
            PositionChanged += MainWindow_PositionChanged;
            WindowStateChanged += MainWindow_WindowStateChanged; // 最小化時にタスクバーから非表示(未実装)

            if (WindowContent is FrameworkElement content)
            {
                // コンテンツに紐付く各種イベントハンドラの登録

                // 動作検証用 (ここから)
                content.SizeChanged += (sender, e) => { Debug.WriteLine($"[WindowContent:SizeChanged] PreviousSize={Str(e.PreviousSize)}, NewSize={Str(e.NewSize)}"); };
                content.PointerEntered += (sender, e) => { Debug.WriteLine($"[WindowContent:PointerEntered]"); };
                content.PointerExited += (sender, e) => { Debug.WriteLine($"[WindowContent:PointerExited]"); };
                // 動作検証用 (ここまで)

                // SizeToContentエミュレーション。一瞬リサイズ前のウィンドウが表示されるが、根本的な対処方法不明。
                // ワークアラウンドとして、ウィンドウサイズ確定後の値を設定ファイルに保存し、Windowのコンストラクタで
                // this.AppWindow.Size()等を呼び出す。
                content.LayoutUpdated += Content_LayoutUpdated;

                // 画面に表示されている「月」の行数および列数を登録
                content.Loaded += Content_Loaded;

                // ウィンドウ自体を掴んで移動可能にする。
                // マウス操作系はWndProcで拾えない(おそらく子のUIElementに食われている)ので、
                // WindowContentのマウス操作イベントにハンドラを登録。参考:
                // https://github.com/castorix/WinUI3_Transparent/blob/6c678cfdcf77340f17912cf9163eff94520cced7/MainWindow.xaml.cs#L232
                // ただし、背景色等が未設定のボーダーやパディング部分で操作を行うとイベントがロストする模様。(イベントを拾える箇所が不明)
                // コンテンツ最背面のバックグラウンドカラーを明示的に設定すると正常動作する。(本アプリの場合、BorderにBackgroundを設定)
                content.PointerPressed += Content_PointerPressed;
                content.PointerMoved += Content_PointerMoved;
                content.PointerReleased += Content_PointerReleased;

                // マウスホイールで上下移動
                content.PointerWheelChanged += Content_PointerWheelChanged;
            }

            // 前回起動時に保存した位置とサイズを復帰
            _localConfigService.Load(entry =>
            {
                PointInt32 pos = new((int)entry.Left, (int)entry.Top);
                SizeInt32 size = new((int)entry.Width, (int)entry.Height);
                AdjustWindowPosition(ref pos, size);
                if (entry.Width > 0 && entry.Height > 0)
                {
                    AppWindow.MoveAndResize(new(pos.X, pos.Y, size.Width, size.Height));
                }
                else
                {
                    AppWindow.Move(new(pos.X, pos.Y));
                }
                Debug.WriteLine($"Loaded: ({entry.Left}, {entry.Top}, {entry.Width}, {entry.Height})");
            });
        }

        private LRESULT WndProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            // 動作検証用 (ここから)
            if (MsgToString.TryGetValue(uMsg, out string name))
            {
                Debug.WriteLine($"Message=[{name}]");
            }
            else
            {
                Debug.WriteLine($"Message=[{uMsg}]");
            }
            // 動作検証用 (ここまで)

            // 通知アイコン対応時に復旧
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

            switch (uMsg)
            {
                default:
                    return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
            }
        }
        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            MainWindowViewModel vm = CalendarRoot.DataContext as MainWindowViewModel;
            if (!_isInitialized)
            {
                vm?.SettingsViewModel.UpdateHolidaysCommand.Execute(this);
                RegisterNotifyIcon();
                _isInitialized = true;
            }
            vm?.UpdateTodayCommand.Execute(null);
        }

        private void RegisterNotifyIcon()
        {
            // 通知アイコン対応時に復旧

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
        }

        // 通知アイコン対応時に復旧
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

            // 通知アイコン対応時に復旧
            //if (_notifyIconManager != null)
            //{
            //    _notifyIconManager.Delete();
            //    _source!.RemoveHook(_sourceHook);
            //    _notifyIconManager = null;
            //    _source = null;
            //    _sourceHook = null;
            //}

            Application.Current.Exit();
        }

        private void MainWindow_PositionChanged(object sender, PointInt32 e)
        {
            switch (_localConfigService.LoadStatus)
            {
                case LoadStatus.NotLoaded:
                    // no operation.
                    break;
                case LoadStatus.Loading:
                    // no operation.
                    break;
                case LoadStatus.Loaded:
                    PointInt32 pos = AppWindow.Position;
                    SizeInt32 size = AppWindow.Size;
                    _localConfigService.Save(pos.X, pos.Y, size.Width, size.Height);
                    break;
            }
        }

        private void MainWindow_WindowStateChanged(object sender, WindowState e)
        {
            // 通知アイコン実装後に有効化
            //
            // if (WindowState == WindowState.Minimized && _isNotificationIconAdded)
            // {
            //     Hide();
            // }
        }

        private void Content_LayoutUpdated(object sender, object e)
        {
            if (_isWindowSizeAdjusted || AppWindow == null) return;

            SizeInt32 size = AppWindow.Size;
            SizeInt32 cSize = AppWindow.ClientSize;
            FrameworkElement elem = (FrameworkElement)WindowContent;
            Windows.Foundation.Size dSize = elem.DesiredSize;
            Debug.WriteLine($"[WindowContent:LayoutUpdated] AppWindow[Size={Str(size)}, ClientSize={Str(cSize)}], WindowContent.Desired={Str(dSize)}");
            if (dSize.Width > 0)
            {
                if (dSize.Width == cSize.Width && dSize.Height == cSize.Height)
                {
                    _isWindowSizeAdjusted = true;
                }
                else
                {
                    PointInt32 pos = AppWindow.Position;
                    int wOffset = size.Width - cSize.Width;
                    int hOffset = size.Height - cSize.Height;
                    RectInt32 rect = new(pos.X, pos.Y, (int)dSize.Width + wOffset, (int)dSize.Height + hOffset);
                    AppWindow.MoveAndResize(rect);
                    Debug.WriteLine($"[WindowContent:LayoutUpdated] Resize=({rect.Width}, {rect.Height})");
                }
            }
        }

        private void Content_Loaded(object sender, RoutedEventArgs e)
        {
            if (CalendarRoot.DataContext is MainWindowViewModel vm)
            {
                vm.RowCount = CalendarRoot.ColumnDefinitions.Count;
                vm.ColumnCount = CalendarRoot.ColumnDefinitions.Count;
                RegisterKeyboardAccelerators((UIElement)sender);
            }
        }

        private void RegisterKeyboardAccelerators(UIElement content)
        {
            MainWindowViewModel vm = CalendarRoot.DataContext as MainWindowViewModel;
            IList<KeyValuePair<VirtualKey, IRelayCommand>> kaEntries = [
                KeyValuePair.Create(VirtualKey.Home, vm.ResetPageCommand),
                KeyValuePair.Create(VirtualKey.Left, vm.PrevMonthCommand),
                KeyValuePair.Create(VirtualKey.Right, vm.NextMonthCommand),
                KeyValuePair.Create(VirtualKey.Up, vm.PrevLineCommand),
                KeyValuePair.Create(VirtualKey.Down, vm.NextLineCommand),
                KeyValuePair.Create(VirtualKey.PageUp, vm.PrevPageCommand),
                KeyValuePair.Create(VirtualKey.PageDown, vm.NextPageCommand),
            ];
            var kas = content.KeyboardAccelerators;
            foreach (var kaEntry in kaEntries)
            {
                var ka = new KeyboardAccelerator() { Key = kaEntry.Key };
                ka.Invoked += (_, _) => kaEntry.Value.Execute(this);
                kas.Add(ka);
            }
        }

        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Microsoft.UI.Input.PointerPointProperties props = e.GetCurrentPoint((UIElement)sender).Properties;
            if (props.IsLeftButtonPressed)
            {
                ((UIElement)sender).CapturePointer(e.Pointer);
                _isDragging = true;
                _startWinPos = AppWindow.Position;
                PInvoke.GetCursorPos(out _startCsrPos);
            }
        }

        private void Content_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Microsoft.UI.Input.PointerPointProperties props = e.GetCurrentPoint((UIElement)sender).Properties;
            if (props.IsLeftButtonPressed && _isDragging)
            {
                PInvoke.GetCursorPos(out System.Drawing.Point curCsrPos);
                int dX = curCsrPos.X - _startCsrPos.X;
                int dY = curCsrPos.Y - _startCsrPos.Y;
                PointInt32 pos = new(_startWinPos.X + dX, _startWinPos.Y + dY);
                SizeInt32 size = AppWindow.Size;
                AdjustWindowPosition(ref pos, size);
                AppWindow.Move(pos);
            }
        }

        private void Content_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Microsoft.UI.Input.PointerPointProperties props = e.GetCurrentPoint((UIElement)sender).Properties;
            if (!props.IsLeftButtonPressed)
            {
                ((UIElement)sender).ReleasePointerCapture(e.Pointer);
                _isDragging = false;
            }
        }
        private void Content_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (CalendarRoot.DataContext is MainWindowViewModel vm)
            {
                Microsoft.UI.Input.PointerPoint pp = e.GetCurrentPoint((UIElement)sender);
                int delta = pp.Properties.MouseWheelDelta;
                if (delta < 0)
                {
                    vm.NextLineCommand.Execute(null);
                }
                else if (delta > 0)
                {
                    vm.PrevLineCommand.Execute(null);
                }
            }
        }

        private bool AdjustWindowPosition(ref PointInt32 pos, SizeInt32 size)
        {
            if (!_displayAreas.IsActive) return false;

            bool adjusted = false;
            RectInt32 wa = _displayAreas.VirtualWorkAreaRect;
            if (pos.X < wa.X)
            {
                pos.X = wa.X;
                adjusted = true;
            }
            else if (size.Width > 0 && wa.X + wa.Width < pos.X + size.Width)
            {
                pos.X = wa.X + wa.Width - size.Width;
                adjusted = true;
            }
            if (pos.Y < wa.Y)
            {
                pos.Y = wa.Y;
                adjusted = true;
            }
            else if (size.Height > 0 && wa.Y + wa.Height < pos.Y + size.Height)
            {
                pos.Y = wa.Y + wa.Height - size.Height;
                adjusted = true;
            }
            return adjusted;
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsView == null)
            {
                //_settingsView = new SettingsView();
            }
            //_settingsView.Show();

        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if (_help == null)
            {
                //_help = new Help();
            }
            //_help.Show();

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow_Closed(sender, default);
        }

        private void Home_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {

        }

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
