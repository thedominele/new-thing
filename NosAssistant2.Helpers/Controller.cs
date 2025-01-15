using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NosAssistant2.Configs;
using NosAssistant2.GameData;

namespace NosAssistant2.Helpers;

public static class Controller
{
	private static Thread soundThread;

	public static bool isPlaying = false;

	public static bool renamedClients = true;

	public static readonly Dictionary<int, (Keys Key, bool ControlPressed)> HotBarSlotIDToKeyDict = new Dictionary<int, (Keys, bool)>
	{
		{
			0,
			(Keys.D1, false)
		},
		{
			1,
			(Keys.D2, false)
		},
		{
			2,
			(Keys.D3, false)
		},
		{
			3,
			(Keys.D4, false)
		},
		{
			4,
			(Keys.D5, false)
		},
		{
			5,
			(Keys.D6, false)
		},
		{
			6,
			(Keys.D7, false)
		},
		{
			7,
			(Keys.D8, false)
		},
		{
			8,
			(Keys.D9, false)
		},
		{
			9,
			(Keys.D0, false)
		},
		{
			10,
			(Keys.Q, false)
		},
		{
			11,
			(Keys.W, false)
		},
		{
			12,
			(Keys.E, false)
		},
		{
			13,
			(Keys.R, false)
		},
		{
			14,
			(Keys.T, false)
		},
		{
			15,
			(Keys.Q, true)
		},
		{
			16,
			(Keys.W, true)
		},
		{
			17,
			(Keys.E, true)
		},
		{
			18,
			(Keys.R, true)
		},
		{
			19,
			(Keys.T, true)
		},
		{
			20,
			(Keys.D1, true)
		},
		{
			21,
			(Keys.D2, true)
		},
		{
			22,
			(Keys.D3, true)
		},
		{
			23,
			(Keys.D4, true)
		},
		{
			24,
			(Keys.D5, true)
		},
		{
			25,
			(Keys.D6, true)
		},
		{
			26,
			(Keys.D7, true)
		},
		{
			27,
			(Keys.D8, true)
		},
		{
			28,
			(Keys.D9, true)
		},
		{
			29,
			(Keys.D0, true)
		}
	};

	public static int DetectMonitorsScalingFactors()
	{
		Screen primaryScreen = Screen.PrimaryScreen;
		return 100 - (100 - (int)((double)primaryScreen.Bounds.Width / 2560.0 * 100.0)) / 2;
	}

	public static List<NostaleCharacterInfo> getWindowsList()
	{
		List<NostaleCharacterInfo> list = new List<NostaleCharacterInfo>();
		nint zero = IntPtr.Zero;
		nint zero2 = IntPtr.Zero;
		for (nint num = DllImports.FindWindowEx(zero, zero2, "TNosTaleMainF", null); num != IntPtr.Zero; num = DllImports.FindWindowEx(zero, zero2, "TNosTaleMainF", null))
		{
			DllImports.GetWindowThreadProcessId(num, out var lpdwProcessId);
			Process processById = Process.GetProcessById((int)lpdwProcessId);
			list.Add(new NostaleCharacterInfo
			{
				hwnd = num,
				process_id = lpdwProcessId,
				start_time = processById.StartTime
			});
			zero2 = num;
		}
		list.Sort((NostaleCharacterInfo x, NostaleCharacterInfo y) => x.start_time.CompareTo(y.start_time));
		return list;
	}

	public static void SingleButtonPress(nint HWND, Keys key)
	{
		DllImports.PostMessage(HWND, DllImports.WM_KEYDOWN, (int)key, 0);
		DllImports.PostMessage(HWND, DllImports.WM_KEYUP, (int)key, 0);
	}

	public static async Task MultiButtonPress(List<NostaleCharacterInfo> characters, Keys key, int delay = 0)
	{
		foreach (NostaleCharacterInfo character in characters)
		{
			SingleButtonPress(character.hwnd, key);
			await Task.Delay(Utils.randomizeDelay(delay));
		}
	}

	public static void buttonWithShift(nint HWND, Keys button)
	{
		uint wParam = DllImports.VkKeyScan((char)button);
		if (DllImports.GetForegroundWindow() == HWND)
		{
			ButtonPressForeground(button, "shift");
			return;
		}
		DllImports.keybd_event(DllImports.VK_SHIFT, 42u, 0, 0);
		DllImports.SendMessage(HWND, DllImports.WM_KEYDOWN, wParam, 0);
		DllImports.SendMessage(HWND, DllImports.WM_KEYUP, wParam, 0);
		DllImports.keybd_event(DllImports.VK_SHIFT, 42u, 2, 0);
	}

	public static void buttonWithControl(nint HWND, Keys button)
	{
		uint wParam = DllImports.VkKeyScan((char)button);
		if (DllImports.GetForegroundWindow() == HWND)
		{
			ButtonPressForeground(button, "ctrl");
			return;
		}
		DllImports.keybd_event(DllImports.VK_CONTROL, 29u, 0, 0);
		DllImports.SendMessage(HWND, DllImports.WM_KEYDOWN, wParam, 0);
		DllImports.SendMessage(HWND, DllImports.WM_KEYUP, wParam, 0);
		DllImports.keybd_event(DllImports.VK_CONTROL, 29u, 2, 0);
	}

	public static void ButtonPressForeground(Keys button, string modifier = "none")
	{
		uint num;
		switch (modifier.ToLower())
		{
		default:
			return;
		case "ctrl":
			num = DllImports.VK_CONTROL;
			break;
		case "alt":
			num = 18u;
			break;
		case "shift":
			num = DllImports.VK_SHIFT;
			break;
		case "none":
			num = 0u;
			break;
		}
		uint keyCode = DllImports.VkKeyScan((char)button);
		DllImports.INPUT[] array = ((num != 0) ? new DllImports.INPUT[4]
		{
			CreateInput(num, 0u),
			CreateInput(keyCode, 0u),
			CreateInput(keyCode, 2u),
			CreateInput(num, 2u)
		} : new DllImports.INPUT[2]
		{
			CreateInput(keyCode, 0u),
			CreateInput(keyCode, 2u)
		});
		DllImports.SendInput((uint)array.Length, array, Marshal.SizeOf(typeof(DllImports.INPUT)));
	}

	private static DllImports.INPUT CreateInput(uint keyCode, uint dwFlags)
	{
		DllImports.INPUT result = default(DllImports.INPUT);
		result.type = 1u;
		result.u = new DllImports.InputUnion
		{
			ki = new DllImports.KEYBDINPUT
			{
				wVk = (ushort)keyCode,
				dwFlags = dwFlags
			}
		};
		return result;
	}

	public static void buttonWithAlt(nint HWND, int key)
	{
		DllImports.PostMessage(HWND, DllImports.WM_KEYDOWN, key, 537001985);
		DllImports.PostMessage(HWND, DllImports.WM_KEYUP, key, 3758227457u);
	}

	public static int MakeLong(short a, short b)
	{
		return (ushort)a | ((ushort)b << 16);
	}

	public static async Task ClickInBackground(short X, short Y, nint HWND)
	{
		int Coordinates = MakeLong(X, Y);
		DllImports.PostMessage(HWND, DllImports.WM_MOUSEMOVE, 0, Coordinates);
		DllImports.PostMessage(HWND, DllImports.WM_LBUTTONDOWN, DllImports.MK_LBUTTON, Coordinates);
		await Task.Delay(5);
		DllImports.PostMessage(HWND, DllImports.WM_LBUTTONUP, 0, Coordinates);
	}

	public static void MouseClick()
	{
		uint x = (uint)Cursor.Position.X;
		uint y = (uint)Cursor.Position.Y;
		DllImports.mouse_event(6u, x, y, 0u, 0u);
	}

	public static async Task MultiClickInBackground(short X, short Y, List<NostaleCharacterInfo> characters, int delay)
	{
		int Coordinates = MakeLong(X, Y);
		foreach (NostaleCharacterInfo character in characters)
		{
			DllImports.PostMessage(character.hwnd, DllImports.WM_MOUSEMOVE, 0, Coordinates);
			DllImports.PostMessage(character.hwnd, DllImports.WM_LBUTTONDOWN, DllImports.MK_LBUTTON, Coordinates);
			await Task.Delay(5);
			DllImports.PostMessage(character.hwnd, DllImports.WM_LBUTTONUP, 0, Coordinates);
			await Task.Delay(Utils.randomizeDelay(delay));
		}
	}

	public static void RenameClients(bool? force = false)
	{
		if (force == true)
		{
			renamedClients = true;
		}
		int num = 1;
		foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
		{
			if (renamedClients)
			{
				DllImports.SetWindowTextA(nostaleCharacterInfo.hwnd, "NosTale " + num + " " + nostaleCharacterInfo.nickname);
			}
			else
			{
				DllImports.SetWindowTextA(nostaleCharacterInfo.hwnd, "NosTale");
			}
			num++;
		}
		if (force == false)
		{
			renamedClients = !renamedClients;
		}
	}

	private static bool isMinimized(nint HWND)
	{
		if (DllImports.IsWindowVisible(HWND))
		{
			return false;
		}
		return true;
	}

	private static bool isFullScreen(nint HWND)
	{
		nint desktopWindow = DllImports.GetDesktopWindow();
		nint shellWindow = DllImports.GetShellWindow();
		HWND = DllImports.GetForegroundWindow();
		if (HWND != IntPtr.Zero && !((IntPtr)HWND).Equals(IntPtr.Zero) && !((IntPtr)HWND).Equals(desktopWindow) && !((IntPtr)HWND).Equals(shellWindow))
		{
			DllImports.GetWindowRect(HWND, out var rect);
			Rectangle bounds = Screen.FromHandle(HWND).Bounds;
			if (rect.bottom - rect.top == bounds.Height && rect.right - rect.left == bounds.Width)
			{
				return true;
			}
		}
		return false;
	}

	private static void moveWindow(nint HWND, int x, int y)
	{
		DllImports.SetWindowPos(HWND, 0, x, y, 0, 0, 1);
	}

	public static void stackWindows()
	{
		Screen screen = null;
		foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
		{
			if (nostaleCharacterInfo.config.isAttacker || nostaleCharacterInfo.config.isDisabled || !nostaleCharacterInfo.config.isRaider)
			{
				screen = Screen.FromHandle(nostaleCharacterInfo.hwnd);
				break;
			}
		}
		if (screen == null)
		{
			return;
		}
		foreach (NostaleCharacterInfo item in GUI._nostaleCharacterInfoList.OrderByDescending((NostaleCharacterInfo x) => !x.config.isAttacker).ThenByDescending(GUI._nostaleCharacterInfoList.IndexOf))
		{
			if (isMinimized(item.hwnd))
			{
				DllImports.ShowWindow(item.hwnd, DllImports.SW_NORMAL);
			}
			DllImports.RECT rect = default(DllImports.RECT);
			DllImports.GetClientRect(item.hwnd, ref rect);
			moveWindow(item.hwnd, screen.WorkingArea.X + screen.WorkingArea.Width / 2 - rect.right / 2, screen.WorkingArea.Y + screen.WorkingArea.Height / 2 - rect.bottom / 2);
			DllImports.SetForegroundWindow(item.hwnd);
		}
	}

	public static void windowsToWaterfall()
	{
		Screen screen = null;
		DllImports.RECT rect = default(DllImports.RECT);
		int count = GUI._nostaleCharacterInfoList.Count;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (NostaleCharacterInfo item in (from item in GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => (x.config.isAttacker || x.config.isRaider) && !x.config.isDisabled).Select((NostaleCharacterInfo x, int i) => new
			{
				Character = x,
				OriginalIndex = i
			})
			orderby item.Character.config.isAttacker, item.OriginalIndex descending
			select item.Character).ToList())
		{
			if (num3 == 0)
			{
				DllImports.GetClientRect(item.hwnd, ref rect);
				screen = Screen.FromHandle(item.hwnd);
				rect.right += 3;
				if (screen == null)
				{
					continue;
				}
				if (count == 1)
				{
					num = 0;
					num2 = 0;
				}
				else
				{
					num = (screen.WorkingArea.Width - rect.right) / (count - 1);
					num2 = (screen.WorkingArea.Height - rect.bottom - 26) / (count - 1);
				}
				if (num2 > 110)
				{
					num2 = 80;
				}
				if (num > 250)
				{
					num = 150;
				}
			}
			if (screen != null)
			{
				if (isMinimized(item.hwnd))
				{
					DllImports.ShowWindow(item.hwnd, DllImports.SW_NORMAL);
				}
				moveWindow(item.hwnd, screen.WorkingArea.X + screen.WorkingArea.Width - rect.right - num3 * num, screen.WorkingArea.Y + num3 * num2);
				DllImports.SetForegroundWindow(item.hwnd);
				num3++;
			}
		}
	}

	private static bool isPointVisibleOnAScreen(Point p)
	{
		Screen[] allScreens = Screen.AllScreens;
		foreach (Screen screen in allScreens)
		{
			if (p.X < screen.Bounds.Right && p.X > screen.Bounds.Left && p.Y > screen.Bounds.Top && p.Y < screen.Bounds.Bottom)
			{
				return true;
			}
		}
		return false;
	}

	private static bool isWidnowFullyVisible(nint HWND)
	{
		DllImports.GetWindowRect(HWND, out var rect);
		if (isPointVisibleOnAScreen(new Point(rect.left, rect.top)) && isPointVisibleOnAScreen(new Point(rect.right, rect.top)) && isPointVisibleOnAScreen(new Point(rect.left, rect.bottom)))
		{
			return isPointVisibleOnAScreen(new Point(rect.right, rect.bottom));
		}
		return false;
	}

	public static async Task PopWindow(nint HWND)
	{
		while (DllImports.GetForegroundWindow() != HWND)
		{
			DllImports.SetForegroundWindow(HWND);
			await Task.Delay(10);
		}
	}

	public static void closeWindow(uint processId)
	{
		Process process = new Process();
		process.StartInfo.FileName = "taskkill";
		process.StartInfo.Arguments = $"/F /PID {processId}";
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
	}

	public static void PlaySound(string sound_name, bool allow_spam = false)
	{
		if ((isPlaying && !allow_spam) || !Settings.config.playSounds)
		{
			return;
		}
		SoundSettings sound_config = null;
		if (sound_name.Contains("Ultimate Arma Color"))
		{
			sound_config = Settings.config.sounds.Find((SoundSettings x) => x.sound_name == "Ultimate Arma Color");
			if (sound_config == null)
			{
				return;
			}
			if (sound_name.Contains("Red"))
			{
				sound_config = new SoundSettings
				{
					sound_name = sound_name,
					path = "sounds/ultimate_arma_red.wav",
					state = sound_config.state,
					volume = sound_config.volume
				};
			}
			else
			{
				if (!sound_name.Contains("Blue"))
				{
					return;
				}
				sound_config = new SoundSettings
				{
					sound_name = sound_name,
					path = "sounds/ultimate_arma_blue.wav",
					state = sound_config.state,
					volume = sound_config.volume
				};
			}
		}
		else
		{
			sound_config = Settings.config.sounds.Find((SoundSettings x) => x.sound_name == sound_name);
		}
		if (sound_config == null || !sound_config.state)
		{
			return;
		}
		isPlaying = true;
		if (!File.Exists(sound_config.path))
		{
			return;
		}
		float volume = (float)Math.Min(Math.Max(sound_config.volume, 0), 100) / 100f;
		soundThread = new Thread((ThreadStart)delegate
		{
			try
			{
				using AudioFileReader waveProvider = new AudioFileReader(sound_config.path);
				using WaveOutEvent waveOutEvent = new WaveOutEvent();
				waveOutEvent.Init(new VolumeSampleProvider(waveProvider.ToSampleProvider())
				{
					Volume = volume
				});
				waveOutEvent.Play();
				while (waveOutEvent.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred while playing audio: " + ex.Message);
			}
			isPlaying = false;
		});
		soundThread.Start();
	}

	public static Point getCursorPosOnWindow()
	{
		DllImports.GetCursorPos(out var lpPoint);
		nint foregroundWindow = DllImports.GetForegroundWindow();
		DllImports.GetWindowRect(foregroundWindow, out var rect);
		if (!isFullScreen(foregroundWindow))
		{
			return new Point(lpPoint.X - rect.left - 3, lpPoint.Y - rect.top - 26);
		}
		return new Point(lpPoint.X - rect.left, lpPoint.Y - rect.top);
	}

	public static void SaveWindowsPositions()
	{
		DllImports.RECT rect = default(DllImports.RECT);
		foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
		{
			DllImports.GetWindowRect(nostaleCharacterInfo.hwnd, out rect);
			nostaleCharacterInfo.config.window_position = new Point(rect.left, rect.top);
		}
		Settings.SaveSettings();
	}

	public static void LoadWindowsPositions()
	{
		foreach (NostaleCharacterInfo nostaleCharacterInfo in GUI._nostaleCharacterInfoList)
		{
			Point window_position = nostaleCharacterInfo.config.window_position;
			moveWindow(nostaleCharacterInfo.hwnd, window_position.X, window_position.Y);
		}
		Settings.SaveSettings();
	}

	public static void ResizeWindows(int width, int height)
	{
		nint num = new IntPtr(0);
		int num2 = 2;
		int num3 = 4;
		int num4 = 64;
		foreach (NostaleCharacterInfo item in GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isAttacker && !x.config.isDisabled))
		{
			DllImports.RECT rect = default(DllImports.RECT);
			DllImports.GetWindowRect(item.hwnd, out rect);
			DllImports.SetWindowPos(item.hwnd, (int)num, rect.left, rect.top, width, height, num2 | num3 | num4);
		}
		RefreshMonitors();
	}

	private static void RefreshMonitors()
	{
		DllImports.DEVMODE devMode = default(DllImports.DEVMODE);
		devMode.dmSize = (short)Marshal.SizeOf(devMode);
		DllImports.EnumDisplaySettings(null, -1, ref devMode);
		devMode.dmDisplayOrientation = ScreenOrientation.Angle180;
		DllImports.ChangeDisplaySettings(ref devMode, 0);
		devMode.dmDisplayOrientation = ScreenOrientation.Angle0;
		DllImports.ChangeDisplaySettings(ref devMode, 0);
	}

	public static void LimitFPS(int processId, float fps)
	{
		float num = 4E-45f;
		float num2 = fps / num;
		num2 = 30f / fps;
		float value = 4.5E-44f * num2;
		try
		{
			Process processById = Process.GetProcessById(processId);
			nint baseAddress = processById.MainModule.BaseAddress;
			int offset = 3537192;
			nint lpBaseAddress = IntPtr.Add(baseAddress, offset);
			byte[] bytes = BitConverter.GetBytes(value);
			if (DllImports.WriteProcessMemory(processById.Handle, lpBaseAddress, bytes, bytes.Length, out var _))
			{
				_ = bytes.Length;
			}
		}
		catch (Exception)
		{
		}
	}
}
