using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace SimpleCalendar.WinUI3.Utilities
{
    public static class WindowHelper
    {
        public static void DisableTitleBar(Window window)
        {
            AppWindow appWindow = window.AppWindow;
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(true, false);
                HWND hwnd = new(window.GetWindowHandle());
                WINDOW_LONG_PTR_INDEX index = WINDOW_LONG_PTR_INDEX.GWL_STYLE;
                nint style = PInvoke.GetWindowLongPtr(hwnd, index);
                style &= ~(nint)WindowStyle.ThickFrame;
                style |= (nint)WindowStyle.Border;
                PInvoke.SetWindowLongPtr(hwnd, index, style);
            }
        }

        public static void F(nint hwndi)
        {
            HWND hwnd = new(hwndi);
            var dpi = PInvoke.GetDpiForWindow(hwnd);
            var cxFrame = PInvoke.GetSystemMetricsForDpi(SYSTEM_METRICS_INDEX.SM_CXFRAME, dpi);
            var cyFrame = PInvoke.GetSystemMetricsForDpi(SYSTEM_METRICS_INDEX.SM_CYFRAME, dpi);
            var cxBorder = PInvoke.GetSystemMetricsForDpi(SYSTEM_METRICS_INDEX.SM_CXBORDER, dpi);
            var cyBorder = PInvoke.GetSystemMetricsForDpi(SYSTEM_METRICS_INDEX.SM_CYBORDER, dpi);

        }
    }
}
