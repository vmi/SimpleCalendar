using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.Graphics;
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

        public static Size Size(SizeInt32 size) => new(size.Width, size.Height);
        public static string Str(Size size) => $"({size.Width}, {size.Height})";
        public static string Str(SizeInt32 size) => Str(Size(size));

    }
}
