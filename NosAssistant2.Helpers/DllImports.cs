using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NosAssistant2.GameData;

namespace NosAssistant2.Helpers;

public static class DllImports
{
	public struct POINT
	{
		public int X;

		public int Y;

		public POINT(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public enum Monitor_DPI_Type
	{
		MDT_Effective_DPI,
		MDT_Angular_DPI,
		MDT_Raw_DPI
	}

	public struct INPUT
	{
		public uint type;

		public InputUnion u;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct InputUnion
	{
		[FieldOffset(0)]
		public MOUSEINPUT mi;

		[FieldOffset(0)]
		public KEYBDINPUT ki;

		[FieldOffset(0)]
		public HARDWAREINPUT hi;
	}

	public struct MOUSEINPUT
	{
		public int dx;

		public int dy;

		public uint mouseData;

		public uint dwFlags;

		public uint time;

		public nint dwExtraInfo;
	}

	public struct KEYBDINPUT
	{
		public ushort wVk;

		public ushort wScan;

		public uint dwFlags;

		public uint time;

		public nint dwExtraInfo;
	}

	public struct HARDWAREINPUT
	{
		public uint uMsg;

		public ushort wParamL;

		public ushort wParamH;
	}

	public struct RECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;
	}

	public struct DEVMODE
	{
		private const int CCHDEVICENAME = 32;

		private const int CCHFORMNAME = 32;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmDeviceName;

		public short dmSpecVersion;

		public short dmDriverVersion;

		public short dmSize;

		public short dmDriverExtra;

		public int dmFields;

		public int dmPositionX;

		public int dmPositionY;

		public ScreenOrientation dmDisplayOrientation;

		public int dmDisplayFixedOutput;

		public short dmColor;

		public short dmDuplex;

		public short dmYResolution;

		public short dmTTOption;

		public short dmCollate;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmFormName;

		public short dmLogPixels;

		public int dmBitsPerPel;

		public int dmPelsWidth;

		public int dmPelsHeight;

		public int dmDisplayFlags;

		public int dmDisplayFrequency;
	}

	public static uint WM_KEYDOWN = 256u;

	public static uint WM_KEYUP = 257u;

	public static uint WM_MOUSEMOVE = 512u;

	public static uint WM_LBUTTONDOWN = 513u;

	public static uint WM_LBUTTONUP = 514u;

	public static int MK_LBUTTON = 1;

	public static int WM_MOUSEWHEEL = 522;

	public static uint WM_RBUTTONDOWN = 516u;

	public static uint WM_RBUTTONUP = 517u;

	public static int WH_MOUSE_LL = 14;

	public static uint WM_CLOSE = 16u;

	public static int SW_NORMAL = 9;

	public static int SW_HIDE = 0;

	public static uint VK_CONTROL = 162u;

	public static uint VK_SHIFT = 16u;

	public const uint VK_MENU = 18u;

	public static uint VK_ALT = 56u;

	public static int VK_F4 = 62;

	public static uint VK_E = 105u;

	public const uint KEYBDEVENTF_SHIFTSCANCODE = 42u;

	public const uint KEYBDEVENTF_CONTROLSCANCODE = 29u;

	public const int INPUT_KEYBOARD = 1;

	public const int KEYEVENTF_KEYDOWN = 0;

	public const int KEYEVENTF_KEYUP = 2;

	public const int SRCCOPY = 13369376;

	public const int ENUM_CURRENT_SETTINGS = -1;

	public const int DISP_CHANGE_SUCCESSFUL = 0;

	public const int WS_MINIMIZEBOX = 131072;

	public const int CS_DBLCLKS = 8;

	[DllImport("gdi32.dll")]
	public static extern bool BitBlt(nint hObject, int nXDest, int nYDest, int nWidth, int nHeight, nint hObjectSource, int nXSrc, int nYSrc, int dwRop);

	[DllImport("gdi32.dll")]
	public static extern nint CreateCompatibleBitmap(nint hDC, int nWidth, int nHeight);

	[DllImport("gdi32.dll")]
	public static extern nint CreateCompatibleDC(nint hDC);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteDC(nint hDC);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteObject(nint hObject);

	[DllImport("gdi32.dll")]
	public static extern nint SelectObject(nint hDC, nint hObject);

	[DllImport("user32.dll")]
	public static extern nint GetDC(nint hWnd);

	[DllImport("user32.dll")]
	public static extern nint ReleaseDC(nint hWnd, nint hDC);

	[DllImport("user32.dll")]
	public static extern nint GetClientRect(nint hWnd, ref RECT rect);

	[DllImport("Gdi32.dll")]
	public static extern nint CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRec, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

	[DllImport("user32.dll")]
	public static extern bool ReleaseCapture();

	[DllImport("user32.dll")]
	public static extern nint SendMessage(nint hWnd, int Msg, int wParam, int lParam);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsWindow(nint hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll")]
	public static extern nint FindWindowEx(nint hWndParent, nint hWndChildAfter, string lpClassName, string? lpWindowName);

	[DllImport("user32.dll")]
	public static extern nint PostMessage(nint hWnd, uint wMsg, int wParam, int lParam);

	[DllImport("user32.dll")]
	public static extern nint PostMessage(nint hWnd, uint wMsg, int wParam, uint lParam);

	[DllImport("user32.dll")]
	public static extern int SendMessage(nint hWnd, uint wMsg, uint wParam, int lParam);

	[DllImport("user32.dll")]
	public static extern nint SetForegroundWindow(nint hWnd);

	[DllImport("user32.dll")]
	public static extern nint GetForegroundWindow();

	[DllImport("user32.dll")]
	public static extern nint SetWindowTextA(nint hWnd, string text);

	[DllImport("user32.dll")]
	public static extern int GetWindowTextA(nint hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern void keybd_event(uint vk, uint scan, int flags, int extrainfo);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsWindowVisible(nint hwnd);

	[DllImport("user32.dll")]
	public static extern uint VkKeyScan(char ch);

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(nint hwnd, int acton);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
	public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

	[DllImport("User32.Dll")]
	public static extern long SetCursorPos(int x, int y);

	[DllImport("user32.dll")]
	public static extern nint SetWindowPos(nint hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	public static extern nint GetWindowRect(nint hWnd, out RECT rect);

	[DllImport("user32.dll")]
	public static extern nint GetDesktopWindow();

	[DllImport("user32.dll")]
	public static extern nint GetShellWindow();

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

	[DllImport("kernel32.dll")]
	public static extern nint OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

	[DllImport("user32.dll")]
	public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll")]
	public static extern bool UnregisterHotKey(nint hWnd, int id);

	[DllImport("iphlpapi.dll", SetLastError = true)]
	public static extern int GetExtendedTcpTable(nint pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TcpTableClass tableClass, int reserved);

	[DllImport("Shcore.dll")]
	public static extern int GetDpiForMonitor(nint hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

	[DllImport("User32.dll", SetLastError = true)]
	public static extern nint MonitorFromPoint(POINT pt, int dwFlags);

	[DllImport("user32.dll")]
	public static extern bool UnhookWindowsHookEx(nint hInstance);

	[DllImport("kernel32.dll")]
	public static extern nint LoadLibrary(string lpFileName);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

	[DllImport("user32.dll")]
	public static extern nint SendMessageW(nint hWnd, int Msg, nint wParam, nint lParam);

	[DllImport("user32.dll")]
	public static extern bool GetCursorPos(out Point lpPoint);

	[DllImport("user32.dll")]
	public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

	[DllImport("user32.dll")]
	public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);
}
