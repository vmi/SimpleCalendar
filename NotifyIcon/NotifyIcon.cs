using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.Controls;
using System.Runtime.CompilerServices;

namespace NotifyIcon
{
    public class NotifyIcon(string appName)
    {
        private const uint NOTIFYICON_VERSION_4 = 4;

        private DestroyIconSafeHandle? hIcon;
        private SafeHandle? hInstance;

        public void Initialize()
        {
            hInstance = PInvoke.GetModuleHandle(Unsafe.As<string>(null));
            PInvoke.LoadIconMetric(hInstance, "IDI_ICON", _LI_METRIC.LIM_LARGE, out hIcon);
            NOTIFY_ICON_MESSAGE msg = NOTIFY_ICON_MESSAGE.NIM_ADD;
            var data = default(NOTIFYICONDATAW);
            data.cbSize = (uint) Marshal.SizeOf<NOTIFYICONDATAW>();
            data.uID = 100; // ???
            data.Anonymous.uVersion = NOTIFYICON_VERSION_4;
            data.szTip = appName;
            data.hIcon = (HICON) hIcon.DangerousGetHandle();
            data.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
            if (PInvoke.Shell_NotifyIcon(msg, data))
            {

            }
        }
    }
}
