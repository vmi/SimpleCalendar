using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace SimpleCalendar.WinUI3.Utilities
{
    public static class WindowHelper
    {
        public static readonly Dictionary<uint, string> MsgToString = [];

        static WindowHelper()
        {
            Add("WM_NULL", 0x0000);
            Add("WM_CREATE", 0x0001);
            Add("WM_DESTROY", 0x0002);
            Add("WM_MOVE", 0x0003);
            Add("WM_SIZE", 0x0005);
            Add("WM_ACTIVATE", 0x0006);
            Add("WM_SETFOCUS", 0x0007);
            Add("WM_KILLFOCUS", 0x0008);
            Add("WM_ENABLE", 0x000A);
            Add("WM_SETREDRAW", 0x000B);
            Add("WM_SETTEXT", 0x000C);
            Add("WM_GETTEXT", 0x000D);
            Add("WM_GETTEXTLENGTH", 0x000E);
            Add("WM_PAINT", 0x000F);
            Add("WM_CLOSE", 0x0010);
            Add("WM_QUERYENDSESSION", 0x0011);
            Add("WM_QUERYOPEN", 0x0013);
            Add("WM_ENDSESSION", 0x0016);
            Add("WM_QUIT", 0x0012);
            Add("WM_ERASEBKGND", 0x0014);
            Add("WM_SYSCOLORCHANGE", 0x0015);
            Add("WM_SHOWWINDOW", 0x0018);
            Add("WM_WININICHANGE", 0x001A);
            // Add("WM_SETTINGCHANGE", WM_WININICHANGE);
            Add("WM_DEVMODECHANGE", 0x001B);
            Add("WM_ACTIVATEAPP", 0x001C);
            Add("WM_FONTCHANGE", 0x001D);
            Add("WM_TIMECHANGE", 0x001E);
            Add("WM_CANCELMODE", 0x001F);
            Add("WM_SETCURSOR", 0x0020);
            Add("WM_MOUSEACTIVATE", 0x0021);
            Add("WM_CHILDACTIVATE", 0x0022);
            Add("WM_QUEUESYNC", 0x0023);
            Add("WM_GETMINMAXINFO", 0x0024);
            Add("WM_PAINTICON", 0x0026);
            Add("WM_ICONERASEBKGND", 0x0027);
            Add("WM_NEXTDLGCTL", 0x0028);
            Add("WM_SPOOLERSTATUS", 0x002A);
            Add("WM_DRAWITEM", 0x002B);
            Add("WM_MEASUREITEM", 0x002C);
            Add("WM_DELETEITEM", 0x002D);
            Add("WM_VKEYTOITEM", 0x002E);
            Add("WM_CHARTOITEM", 0x002F);
            Add("WM_SETFONT", 0x0030);
            Add("WM_GETFONT", 0x0031);
            Add("WM_SETHOTKEY", 0x0032);
            Add("WM_GETHOTKEY", 0x0033);
            Add("WM_QUERYDRAGICON", 0x0037);
            Add("WM_COMPAREITEM", 0x0039);
            Add("WM_GETOBJECT", 0x003D);
            Add("WM_COMPACTING", 0x0041);
            Add("WM_COMMNOTIFY", 0x0044);
            Add("WM_WINDOWPOSCHANGING", 0x0046);
            Add("WM_WINDOWPOSCHANGED", 0x0047);
            Add("WM_POWER", 0x0048);
            Add("WM_COPYDATA", 0x004A);
            Add("WM_CANCELJOURNAL", 0x004B);
            Add("WM_NOTIFY", 0x004E);
            Add("WM_INPUTLANGCHANGEREQUEST", 0x0050);
            Add("WM_INPUTLANGCHANGE", 0x0051);
            Add("WM_TCARD", 0x0052);
            Add("WM_HELP", 0x0053);
            Add("WM_USERCHANGED", 0x0054);
            Add("WM_NOTIFYFORMAT", 0x0055);
            Add("WM_CONTEXTMENU", 0x007B);
            Add("WM_STYLECHANGING", 0x007C);
            Add("WM_STYLECHANGED", 0x007D);
            Add("WM_DISPLAYCHANGE", 0x007E);
            Add("WM_GETICON", 0x007F);
            Add("WM_SETICON", 0x0080);
            Add("WM_NCCREATE", 0x0081);
            Add("WM_NCDESTROY", 0x0082);
            Add("WM_NCCALCSIZE", 0x0083);
            Add("WM_NCHITTEST", 0x0084);
            Add("WM_NCPAINT", 0x0085);
            Add("WM_NCACTIVATE", 0x0086);
            Add("WM_GETDLGCODE", 0x0087);
            Add("WM_SYNCPAINT", 0x0088);
            Add("WM_NCMOUSEMOVE", 0x00A0);
            Add("WM_NCLBUTTONDOWN", 0x00A1);
            Add("WM_NCLBUTTONUP", 0x00A2);
            Add("WM_NCLBUTTONDBLCLK", 0x00A3);
            Add("WM_NCRBUTTONDOWN", 0x00A4);
            Add("WM_NCRBUTTONUP", 0x00A5);
            Add("WM_NCRBUTTONDBLCLK", 0x00A6);
            Add("WM_NCMBUTTONDOWN", 0x00A7);
            Add("WM_NCMBUTTONUP", 0x00A8);
            Add("WM_NCMBUTTONDBLCLK", 0x00A9);
            Add("WM_NCXBUTTONDOWN", 0x00AB);
            Add("WM_NCXBUTTONUP", 0x00AC);
            Add("WM_NCXBUTTONDBLCLK", 0x00AD);
            Add("WM_INPUT_DEVICE_CHANGE", 0x00fe);
            Add("WM_INPUT", 0x00FF);
            Add("WM_KEYFIRST", 0x0100);
            Add("WM_KEYDOWN", 0x0100);
            Add("WM_KEYUP", 0x0101);
            Add("WM_CHAR", 0x0102);
            Add("WM_DEADCHAR", 0x0103);
            Add("WM_SYSKEYDOWN", 0x0104);
            Add("WM_SYSKEYUP", 0x0105);
            Add("WM_SYSCHAR", 0x0106);
            Add("WM_SYSDEADCHAR", 0x0107);
            Add("WM_UNICHAR", 0x0109);
            Add("WM_KEYLAST", 0x0109);
            Add("WM_KEYLAST", 0x0108);
            Add("WM_IME_STARTCOMPOSITION", 0x010D);
            Add("WM_IME_ENDCOMPOSITION", 0x010E);
            Add("WM_IME_COMPOSITION", 0x010F);
            Add("WM_IME_KEYLAST", 0x010F);
            Add("WM_INITDIALOG", 0x0110);
            Add("WM_COMMAND", 0x0111);
            Add("WM_SYSCOMMAND", 0x0112);
            Add("WM_TIMER", 0x0113);
            Add("WM_HSCROLL", 0x0114);
            Add("WM_VSCROLL", 0x0115);
            Add("WM_INITMENU", 0x0116);
            Add("WM_INITMENUPOPUP", 0x0117);
            Add("WM_MENUSELECT", 0x011F);
            Add("WM_GESTURE", 0x0119);
            Add("WM_GESTURENOTIFY", 0x011A);
            Add("WM_MENUCHAR", 0x0120);
            Add("WM_ENTERIDLE", 0x0121);
            Add("WM_MENURBUTTONUP", 0x0122);
            Add("WM_MENUDRAG", 0x0123);
            Add("WM_MENUGETOBJECT", 0x0124);
            Add("WM_UNINITMENUPOPUP", 0x0125);
            Add("WM_MENUCOMMAND", 0x0126);
            Add("WM_CHANGEUISTATE", 0x0127);
            Add("WM_UPDATEUISTATE", 0x0128);
            Add("WM_QUERYUISTATE", 0x0129);
            Add("WM_CTLCOLORMSGBOX", 0x0132);
            Add("WM_CTLCOLOREDIT", 0x0133);
            Add("WM_CTLCOLORLISTBOX", 0x0134);
            Add("WM_CTLCOLORBTN", 0x0135);
            Add("WM_CTLCOLORDLG", 0x0136);
            Add("WM_CTLCOLORSCROLLBAR", 0x0137);
            Add("WM_CTLCOLORSTATIC", 0x0138);
            Add("WM_MOUSEFIRST", 0x0200);
            Add("WM_MOUSEMOVE", 0x0200);
            Add("WM_LBUTTONDOWN", 0x0201);
            Add("WM_LBUTTONUP", 0x0202);
            Add("WM_LBUTTONDBLCLK", 0x0203);
            Add("WM_RBUTTONDOWN", 0x0204);
            Add("WM_RBUTTONUP", 0x0205);
            Add("WM_RBUTTONDBLCLK", 0x0206);
            Add("WM_MBUTTONDOWN", 0x0207);
            Add("WM_MBUTTONUP", 0x0208);
            Add("WM_MBUTTONDBLCLK", 0x0209);
            Add("WM_MOUSEWHEEL", 0x020A);
            Add("WM_XBUTTONDOWN", 0x020B);
            Add("WM_XBUTTONUP", 0x020C);
            Add("WM_XBUTTONDBLCLK", 0x020D);
            Add("WM_MOUSEHWHEEL", 0x020e);
            Add("WM_MOUSELAST", 0x020e);
            Add("WM_MOUSELAST", 0x020d);
            Add("WM_MOUSELAST", 0x020a);
            Add("WM_MOUSELAST", 0x0209);
            Add("WM_PARENTNOTIFY", 0x0210);
            Add("WM_ENTERMENULOOP", 0x0211);
            Add("WM_EXITMENULOOP", 0x0212);
            Add("WM_NEXTMENU", 0x0213);
            Add("WM_SIZING", 0x0214);
            Add("WM_CAPTURECHANGED", 0x0215);
            Add("WM_MOVING", 0x0216);
            Add("WM_POWERBROADCAST", 0x0218);
            Add("WM_DEVICECHANGE", 0x0219);
            Add("WM_MDICREATE", 0x0220);
            Add("WM_MDIDESTROY", 0x0221);
            Add("WM_MDIACTIVATE", 0x0222);
            Add("WM_MDIRESTORE", 0x0223);
            Add("WM_MDINEXT", 0x0224);
            Add("WM_MDIMAXIMIZE", 0x0225);
            Add("WM_MDITILE", 0x0226);
            Add("WM_MDICASCADE", 0x0227);
            Add("WM_MDIICONARRANGE", 0x0228);
            Add("WM_MDIGETACTIVE", 0x0229);
            Add("WM_MDISETMENU", 0x0230);
            Add("WM_ENTERSIZEMOVE", 0x0231);
            Add("WM_EXITSIZEMOVE", 0x0232);
            Add("WM_DROPFILES", 0x0233);
            Add("WM_MDIREFRESHMENU", 0x0234);
            Add("WM_POINTERDEVICECHANGE", 0x238);
            Add("WM_POINTERDEVICEINRANGE", 0x239);
            Add("WM_POINTERDEVICEOUTOFRANGE", 0x23a);
            Add("WM_TOUCH", 0x0240);
            Add("WM_NCPOINTERUPDATE", 0x0241);
            Add("WM_NCPOINTERDOWN", 0x0242);
            Add("WM_NCPOINTERUP", 0x0243);
            Add("WM_POINTERUPDATE", 0x0245);
            Add("WM_POINTERDOWN", 0x0246);
            Add("WM_POINTERUP", 0x0247);
            Add("WM_POINTERENTER", 0x0249);
            Add("WM_POINTERLEAVE", 0x024a);
            Add("WM_POINTERACTIVATE", 0x024b);
            Add("WM_POINTERCAPTURECHANGED", 0x024c);
            Add("WM_TOUCHHITTESTING", 0x024d);
            Add("WM_POINTERWHEEL", 0x024e);
            Add("WM_POINTERHWHEEL", 0x024f);
            Add("WM_POINTERROUTEDTO", 0x0251);
            Add("WM_POINTERROUTEDAWAY", 0x0252);
            Add("WM_POINTERROUTEDRELEASED", 0x0253);
            Add("WM_IME_SETCONTEXT", 0x0281);
            Add("WM_IME_NOTIFY", 0x0282);
            Add("WM_IME_CONTROL", 0x0283);
            Add("WM_IME_COMPOSITIONFULL", 0x0284);
            Add("WM_IME_SELECT", 0x0285);
            Add("WM_IME_CHAR", 0x0286);
            Add("WM_IME_REQUEST", 0x0288);
            Add("WM_IME_KEYDOWN", 0x0290);
            Add("WM_IME_KEYUP", 0x0291);
            Add("WM_MOUSEHOVER", 0x02A1);
            Add("WM_MOUSELEAVE", 0x02A3);
            Add("WM_NCMOUSEHOVER", 0x02A0);
            Add("WM_NCMOUSELEAVE", 0x02A2);
            Add("WM_WTSSESSION_CHANGE", 0x02B1);
            Add("WM_TABLET_FIRST", 0x02c0);
            Add("WM_TABLET_LAST", 0x02df);
            Add("WM_DPICHANGED", 0x02e0);
            Add("WM_DPICHANGED_BEFOREPARENT", 0x02e2);
            Add("WM_DPICHANGED_AFTERPARENT", 0x02e3);
            Add("WM_GETDPISCALEDSIZE", 0x02e4);
            Add("WM_CUT", 0x0300);
            Add("WM_COPY", 0x0301);
            Add("WM_PASTE", 0x0302);
            Add("WM_CLEAR", 0x0303);
            Add("WM_UNDO", 0x0304);
            Add("WM_RENDERFORMAT", 0x0305);
            Add("WM_RENDERALLFORMATS", 0x0306);
            Add("WM_DESTROYCLIPBOARD", 0x0307);
            Add("WM_DRAWCLIPBOARD", 0x0308);
            Add("WM_PAINTCLIPBOARD", 0x0309);
            Add("WM_VSCROLLCLIPBOARD", 0x030A);
            Add("WM_SIZECLIPBOARD", 0x030B);
            Add("WM_ASKCBFORMATNAME", 0x030C);
            Add("WM_CHANGECBCHAIN", 0x030D);
            Add("WM_HSCROLLCLIPBOARD", 0x030E);
            Add("WM_QUERYNEWPALETTE", 0x030F);
            Add("WM_PALETTEISCHANGING", 0x0310);
            Add("WM_PALETTECHANGED", 0x0311);
            Add("WM_HOTKEY", 0x0312);
            Add("WM_PRINT", 0x0317);
            Add("WM_PRINTCLIENT", 0x0318);
            Add("WM_APPCOMMAND", 0x0319);
            Add("WM_THEMECHANGED", 0x031A);
            Add("WM_CLIPBOARDUPDATE", 0x031d);
            Add("WM_DWMCOMPOSITIONCHANGED", 0x031e);
            Add("WM_DWMNCRENDERINGCHANGED", 0x031f);
            Add("WM_DWMCOLORIZATIONCOLORCHANGED", 0x0320);
            Add("WM_DWMWINDOWMAXIMIZEDCHANGE", 0x0321);
            Add("WM_DWMSENDICONICTHUMBNAIL", 0x0323);
            Add("WM_DWMSENDICONICLIVEPREVIEWBITMAP", 0x0326);
            Add("WM_GETTITLEBARINFOEX", 0x033f);
            Add("WM_HANDHELDFIRST", 0x0358);
            Add("WM_HANDHELDLAST", 0x035F);
            Add("WM_AFXFIRST", 0x0360);
            Add("WM_AFXLAST", 0x037F);
            Add("WM_PENWINFIRST", 0x0380);
            Add("WM_PENWINLAST", 0x038F);
            Add("WM_APP", 0x8000);
            Add("WM_USER", 0x0400);
        }

        private static void Add(string name, uint msg)
        {
            if (!MsgToString.TryAdd(msg, name))
            {
                Debug.WriteLine($"Alread registered: {name} = {msg}");
            }
        }

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

        internal static (int, int) GET_XY_LPARAM(LPARAM lParam)
        {
            nint value = lParam.Value;
            int x = (short)(value & 0xffff);
            int y = (short)((value >> 16) & 0xffff);
            return (x, y);
        }

        // 検証用
        public static Size Size(SizeInt32 size) => new(size.Width, size.Height);
        public static string Str(Size size) => $"({size.Width}, {size.Height})";
        public static string Str(SizeInt32 size) => Str(Size(size));

    }
    internal class WndProcRegistrar : IDisposable
    {
        private HWND _hwnd;
        private readonly SUBCLASSPROC _pfnSubclass;
        private readonly nuint _uIdSubclass;

        public WndProcRegistrar(Window window, SUBCLASSPROC pfnSubclass, nuint uIdSubclass, nuint dwRefData)
        {
            _hwnd = new(window.GetWindowHandle());
            _pfnSubclass = pfnSubclass;
            _uIdSubclass = uIdSubclass;
            PInvoke.SetWindowSubclass(_hwnd, _pfnSubclass, _uIdSubclass, dwRefData);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_hwnd.IsNull)
            {
                PInvoke.RemoveWindowSubclass(_hwnd, _pfnSubclass, _uIdSubclass);
                _hwnd = HWND.Null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WndProcRegistrar() { Dispose(false); }

    }

}
