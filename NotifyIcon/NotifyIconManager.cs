using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace NotifyIcon
{
    public class NotifyIconManager(nint hwnd, uint id = 0, Guid? guid = null) : IDisposable
    {
        public const uint WM_APP = PInvoke.WM_APP;

        public bool IsAdded { get; private set; } = false;

        private static readonly Action<nint> s_empty = (h) => { };
        private static readonly Action<nint, uint, uint> s_empty3 = (h, x, y) => { };

        public Action<nint> Select = s_empty;
        public Action<nint> KeySelect = s_empty;
        public Action<nint> BalloonShow = s_empty;
        public Action<nint> BalloonHide = s_empty;
        public Action<nint> BalloonTimeout = s_empty;
        public Action<nint> BalloonUserClick = s_empty;
        public Action<nint> PopupOpen = s_empty;
        public Action<nint> PopupClose = s_empty;
        public Action<nint, uint, uint> ContextMenu = s_empty3;

        public bool Add(Icon icon, string? tip = null, uint? callbackMessage = null)
        {
            if (IsAdded) { return true; }
            var data = default(NOTIFYICONDATAW);
            data.cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>();
            data.hWnd = (HWND)hwnd;
            data.uID = id;
            data.hIcon = (HICON)icon.Handle;
            NOTIFY_ICON_DATA_FLAGS flags = NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
            if (guid != null)
            {
                data.guidItem = (Guid)guid;
                flags |= NOTIFY_ICON_DATA_FLAGS.NIF_GUID;
            }
            if (tip != null)
            {
                data.szTip = tip;
                flags |= NOTIFY_ICON_DATA_FLAGS.NIF_TIP | NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;
            }
            if (callbackMessage != null)
            {
                data.uCallbackMessage = (uint)callbackMessage;
                flags |= NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE;
            }
            data.uFlags = flags;
            bool isAdded = PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, data);
            if (!isAdded)
            {
                Debug.WriteLine("Failed to register notification icon.");
                return IsAdded = false;

            }
            data.Anonymous.uVersion = PInvoke.NOTIFYICON_VERSION_4;
            isAdded = PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, data);
            if (!isAdded)
            {
                Delete();
                Debug.WriteLine("Failed to set notification icon version to 4. Unregistered notification icon.");
                return IsAdded = false;
            }
            return IsAdded = true;
        }

        public bool Delete()
        {
            if (!IsAdded) { return false; }
            NOTIFY_ICON_MESSAGE msg = NOTIFY_ICON_MESSAGE.NIM_DELETE;
            var data = default(NOTIFYICONDATAW);
            data.cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>();
            data.hWnd = (HWND)hwnd;
            data.uID = id;
            if (guid != null)
            {
                data.guidItem = (Guid)guid;
                data.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_GUID;
            }
            return IsAdded = !PInvoke.Shell_NotifyIcon(msg, data);
        }

        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }

        private static uint LoWord(nint param) => (uint)(param & 0x0000FFFF);
        private static uint HiWord(nint param) => (uint)(param >> 16);

        public void NotifyCallback(nint hwnd, nint wParam, nint lParam)
        {
            switch (LoWord(lParam))
            {
                case PInvoke.NIN_SELECT: Select(hwnd); break;
                case PInvoke.NIN_SELECT | PInvoke.NINF_KEY: KeySelect(hwnd); break; // NIN_KEYSELECTがCsWin32で使用できないため、shellapiのヘッダファイルを参考に対応
                case PInvoke.NIN_BALLOONSHOW: BalloonShow(hwnd); break;
                case PInvoke.NIN_BALLOONHIDE: BalloonHide(hwnd); break;
                case PInvoke.NIN_BALLOONTIMEOUT: BalloonTimeout(hwnd); break;
                case PInvoke.NIN_BALLOONUSERCLICK: BalloonUserClick(hwnd); break;
                case PInvoke.NIN_POPUPOPEN: PopupOpen(hwnd); break;
                case PInvoke.NIN_POPUPCLOSE: PopupClose(hwnd); break;
                case PInvoke.WM_CONTEXTMENU: ContextMenu(hwnd, LoWord(wParam), HiWord(wParam)); break;
                default: break;
            }
        }
    }
}
