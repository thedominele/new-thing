using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NosAssistant2.Configs;
using NosAssistant2.GameData;
using NosAssistant2.GUIElements;

namespace NosAssistant2.Helpers;

public static class KeyboardManager
{
	private delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

	private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

	private const int WH_KEYBOARD_LL = 13;

	private const int WM_KEYDOWN = 256;

	private const int WM_KEYUP = 257;

	private const int WH_MOUSE_LL = 14;

	private const int WM_LBUTTONDOWN = 513;

	private const int WM_RBUTTONDOWN = 516;

	private const int WM_MBUTTONDOWN = 519;

	private const int WM_XBUTTONDOWN = 523;

	private static LowLevelKeyboardProc _proc_k = onKeyPressed;

	private static LowLevelMouseProc _proc_m = OnMouseEvent;

	private static nint _hookID = IntPtr.Zero;

	public static bool new_binding = false;

	private static bool performingAction = false;

	public static bool mimic_keyboard = false;

	public static bool mimic_mouse = false;

	private static readonly Dictionary<Keys, string> KeyMap = new Dictionary<Keys, string>
	{
		{
			Keys.LControlKey,
			"LCtrl"
		},
		{
			Keys.RControlKey,
			"RCtrl"
		},
		{
			Keys.LShiftKey,
			"LShift"
		},
		{
			Keys.RShiftKey,
			"RShift"
		},
		{
			Keys.LMenu,
			"LAlt"
		},
		{
			Keys.RMenu,
			"RAlt"
		},
		{
			Keys.Capital,
			"CapsLock"
		},
		{
			Keys.Oemtilde,
			"`"
		},
		{
			Keys.OemMinus,
			"-"
		},
		{
			Keys.Oemplus,
			"="
		},
		{
			Keys.OemOpenBrackets,
			"["
		},
		{
			Keys.OemCloseBrackets,
			"]"
		},
		{
			Keys.OemPipe,
			"\\"
		},
		{
			Keys.OemSemicolon,
			";"
		},
		{
			Keys.OemQuotes,
			"'"
		},
		{
			Keys.Oemcomma,
			","
		},
		{
			Keys.OemPeriod,
			"."
		},
		{
			Keys.OemQuestion,
			"/"
		},
		{
			Keys.Snapshot,
			"PrtScr"
		},
		{
			Keys.Subtract,
			"-"
		},
		{
			Keys.Add,
			"+"
		},
		{
			Keys.Divide,
			"/"
		},
		{
			Keys.NumPad0,
			"Num0"
		},
		{
			Keys.NumPad1,
			"Num1"
		},
		{
			Keys.NumPad2,
			"Num2"
		},
		{
			Keys.NumPad3,
			"Num3"
		},
		{
			Keys.NumPad4,
			"Num4"
		},
		{
			Keys.NumPad5,
			"Num5"
		},
		{
			Keys.NumPad6,
			"Num6"
		},
		{
			Keys.NumPad7,
			"Num7"
		},
		{
			Keys.NumPad8,
			"Num8"
		},
		{
			Keys.NumPad9,
			"Num9"
		}
	};

	private static readonly Dictionary<int, string> MouseButtonsMap = new Dictionary<int, string>
	{
		{ 1, "MBLeft" },
		{ 2, "MBRight" },
		{ 3, "MBWheel" }
	};

	[DllImport("user32.dll")]
	private static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, nint hInstance, uint threadId);

	[DllImport("user32.dll")]
	private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc callback, nint hInstance, uint threadId);

	public static void StartListeningKeyboard()
	{
		using Process process = Process.GetCurrentProcess();
		using ProcessModule processModule = process.MainModule;
		_hookID = SetWindowsHookEx(13, _proc_k, DllImports.LoadLibrary(processModule.ModuleName), 0u);
	}

	private static nint onKeyPressed(int nCode, nint wParam, nint lParam)
	{
		if (nCode >= 0 && wParam == 257)
		{
			handleKeyPressed(Marshal.ReadInt32(lParam), isKeyboard: true);
		}
		return DllImports.CallNextHookEx(_hookID, nCode, wParam, lParam);
	}

	public static void StartListeningMouse()
	{
		using Process process = Process.GetCurrentProcess();
		using ProcessModule processModule = process.MainModule;
		_hookID = SetWindowsHookEx(14, _proc_m, DllImports.LoadLibrary(processModule.ModuleName), 0u);
	}

	private static nint OnMouseEvent(int nCode, nint wParam, nint lParam)
	{
		if (nCode >= 0 && (wParam == 513 || wParam == 516 || wParam == 519 || wParam == 523))
		{
			HandleMouseClick(wParam, lParam);
		}
		return DllImports.CallNextHookEx(_hookID, nCode, wParam, lParam);
	}

	private static void HandleMouseClick(nint wParam, nint lParam)
	{
		if (GUI.Main != null)
		{
			int num = Marshal.ReadInt32(lParam, 8) >> 16;
			num += 3;
			if (wParam == 513)
			{
				num = 1;
			}
			if (wParam == 516)
			{
				num = 2;
			}
			handleKeyPressed(num, isKeyboard: false);
		}
	}

	private static async void handleKeyPressed(int key, bool isKeyboard)
	{
		if (GUI.Main == null)
		{
			return;
		}
		if (new_binding)
		{
			if (isKeyboard || (key != 1 && key != 2))
			{
				handleControlsModification(key, isKeyboard);
			}
			return;
		}
		if (mimic_keyboard && isKeyboard)
		{
			Raids.MimicKeyboard((Keys)key);
			return;
		}
		if (mimic_mouse && !isKeyboard && key == 2)
		{
			Raids.MimicMouse();
		}
		if (!Settings.config.enableHotkeys)
		{
			performingAction = false;
		}
		else if (Settings.config.ControlsSettings.useBuffs.Item2 == isKeyboard && key == Settings.config.ControlsSettings.useBuffs.Item1)
		{
			await Miniland.UseBuffs(0);
		}
		else if (Settings.config.ControlsSettings.invite.Item2 == isKeyboard && key == Settings.config.ControlsSettings.invite.Item1)
		{
			if (!performingAction)
			{
				performingAction = true;
				await Miniland.InvitePlayersToML();
				performingAction = false;
			}
		}
		else if (Settings.config.ControlsSettings.joinList.Item2 == isKeyboard && key == Settings.config.ControlsSettings.joinList.Item1)
		{
			if (!performingAction)
			{
				performingAction = true;
				Raids.JoinList();
				performingAction = false;
			}
		}
		else if (Settings.config.ControlsSettings.exitRaid.Item2 == isKeyboard && key == Settings.config.ControlsSettings.exitRaid.Item1)
		{
			if (!performingAction)
			{
				performingAction = true;
				await Raids.ExitRaid();
				performingAction = false;
			}
		}
		else if (Settings.config.ControlsSettings.massHeal.Item2 == isKeyboard && key == Settings.config.ControlsSettings.massHeal.Item1)
		{
			Raids.MassHeal();
		}
		else if (Settings.config.ControlsSettings.wearSP.Item2 == isKeyboard && key == Settings.config.ControlsSettings.wearSP.Item1)
		{
			await Raids.TransformSP();
		}
		else if (Settings.config.ControlsSettings.useSelfBuffs.Item2 == isKeyboard && key == Settings.config.ControlsSettings.useSelfBuffs.Item1)
		{
			await Miniland.UseSelfBuffs();
		}
		else if (Settings.config.ControlsSettings.useBuffset1.Item2 == isKeyboard && key == Settings.config.ControlsSettings.useBuffset1.Item1)
		{
			await Miniland.UseBuffs(1);
		}
		else if (Settings.config.ControlsSettings.useBuffset2.Item2 == isKeyboard && key == Settings.config.ControlsSettings.useBuffset2.Item1)
		{
			await Miniland.UseBuffs(2);
		}
		else if (Settings.config.ControlsSettings.useBuffset3.Item2 == isKeyboard && key == Settings.config.ControlsSettings.useBuffset3.Item1)
		{
			await Miniland.UseBuffs(3);
		}
		else if (Settings.config.ControlsSettings.useDebuffs.Item2 == isKeyboard && key == Settings.config.ControlsSettings.useDebuffs.Item1)
		{
			await Raids.UseDebuffs();
		}
		else if (Settings.config.ControlsSettings.arcaneWisdom.Item2 == isKeyboard && key == Settings.config.ControlsSettings.arcaneWisdom.Item1)
		{
			await Raids.UseArcaneWisdom();
		}
	}

	private static void handleControlsModification(int key, bool isKeyboard)
	{
		if (GUI.currentlyModifiedKeyLabel == null || GUI.currentlyModifiedKey == "")
		{
			new_binding = false;
			return;
		}
		PropertyInfo[] properties = Settings.config.ControlsSettings.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.GetValue(Settings.config.ControlsSettings, null) is (int num, bool flag))
			{
                if (num != key || propertyInfo.Name != GUI.currentlyModifiedKey || isKeyboard != flag)
                {
                    continue;
                }
            }
			PropertyInfo[] properties2 = Settings.config.ControlsSettings.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo2 in properties2)
			{
				if (propertyInfo2.Name == GUI.currentlyModifiedKey)
				{
					propertyInfo2.SetValue(Settings.config.ControlsSettings, (0, true));
					GUI.ShowPopUp("This key is already binded");
					GUI.currentlyModifiedKeyLabel.Text = "None";
					new_binding = false;
					return;
				}
			}
		}
		switch (GUI.currentlyModifiedKey)
		{
		case "useBuffs":
			Settings.config.ControlsSettings.useBuffs = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel5 = GUI.currentlyModifiedKeyLabel;
				string text5;
				if (!KeyMap.TryGetValue((Keys)key, out string value5))
				{
					Keys keys = (Keys)key;
					text5 = keys.ToString();
				}
				else
				{
					text5 = value5;
				}
				currentlyModifiedKeyLabel5.Text = text5;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffs.Item1);
			}
			break;
		case "invite":
			Settings.config.ControlsSettings.invite = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel3 = GUI.currentlyModifiedKeyLabel;
				string text3;
				if (!KeyMap.TryGetValue((Keys)key, out string value3))
				{
					Keys keys = (Keys)key;
					text3 = keys.ToString();
				}
				else
				{
					text3 = value3;
				}
				currentlyModifiedKeyLabel3.Text = text3;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.invite.Item1);
			}
			break;
		case "joinList":
			Settings.config.ControlsSettings.joinList = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel8 = GUI.currentlyModifiedKeyLabel;
				string text8;
				if (!KeyMap.TryGetValue((Keys)key, out string value8))
				{
					Keys keys = (Keys)key;
					text8 = keys.ToString();
				}
				else
				{
					text8 = value8;
				}
				currentlyModifiedKeyLabel8.Text = text8;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.joinList.Item1);
			}
			break;
		case "exitRaid":
			Settings.config.ControlsSettings.exitRaid = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel6 = GUI.currentlyModifiedKeyLabel;
				string text6;
				if (!KeyMap.TryGetValue((Keys)key, out string value6))
				{
					Keys keys = (Keys)key;
					text6 = keys.ToString();
				}
				else
				{
					text6 = value6;
				}
				currentlyModifiedKeyLabel6.Text = text6;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.exitRaid.Item1);
			}
			break;
		case "massHeal":
			Settings.config.ControlsSettings.massHeal = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel10 = GUI.currentlyModifiedKeyLabel;
				string text10;
				if (!KeyMap.TryGetValue((Keys)key, out string value10))
				{
					Keys keys = (Keys)key;
					text10 = keys.ToString();
				}
				else
				{
					text10 = value10;
				}
				currentlyModifiedKeyLabel10.Text = text10;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.massHeal.Item1);
			}
			break;
		case "wearSP":
			Settings.config.ControlsSettings.wearSP = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel11 = GUI.currentlyModifiedKeyLabel;
				string text11;
				if (!KeyMap.TryGetValue((Keys)key, out string value11))
				{
					Keys keys = (Keys)key;
					text11 = keys.ToString();
				}
				else
				{
					text11 = value11;
				}
				currentlyModifiedKeyLabel11.Text = text11;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.wearSP.Item1);
			}
			break;
		case "useSelfBuffs":
			Settings.config.ControlsSettings.useSelfBuffs = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel9 = GUI.currentlyModifiedKeyLabel;
				string text9;
				if (!KeyMap.TryGetValue((Keys)key, out string value9))
				{
					Keys keys = (Keys)key;
					text9 = keys.ToString();
				}
				else
				{
					text9 = value9;
				}
				currentlyModifiedKeyLabel9.Text = text9;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useSelfBuffs.Item1);
			}
			break;
		case "useBuffset1":
			Settings.config.ControlsSettings.useBuffset1 = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel4 = GUI.currentlyModifiedKeyLabel;
				string text4;
				if (!KeyMap.TryGetValue((Keys)key, out string value4))
				{
					Keys keys = (Keys)key;
					text4 = keys.ToString();
				}
				else
				{
					text4 = value4;
				}
				currentlyModifiedKeyLabel4.Text = text4;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset1.Item1);
			}
			break;
		case "useBuffset2":
			Settings.config.ControlsSettings.useBuffset2 = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel12 = GUI.currentlyModifiedKeyLabel;
				string text12;
				if (!KeyMap.TryGetValue((Keys)key, out string value12))
				{
					Keys keys = (Keys)key;
					text12 = keys.ToString();
				}
				else
				{
					text12 = value12;
				}
				currentlyModifiedKeyLabel12.Text = text12;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset2.Item1);
			}
			break;
		case "useBuffset3":
			Settings.config.ControlsSettings.useBuffset3 = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel2 = GUI.currentlyModifiedKeyLabel;
				string text2;
				if (!KeyMap.TryGetValue((Keys)key, out string value2))
				{
					Keys keys = (Keys)key;
					text2 = keys.ToString();
				}
				else
				{
					text2 = value2;
				}
				currentlyModifiedKeyLabel2.Text = text2;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset3.Item1);
			}
			break;
		case "useDebuffs":
			Settings.config.ControlsSettings.useDebuffs = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel7 = GUI.currentlyModifiedKeyLabel;
				string text7;
				if (!KeyMap.TryGetValue((Keys)key, out string value7))
				{
					Keys keys = (Keys)key;
					text7 = keys.ToString();
				}
				else
				{
					text7 = value7;
				}
				currentlyModifiedKeyLabel7.Text = text7;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useDebuffs.Item1);
			}
			break;
		case "arcaneWisdom":
			Settings.config.ControlsSettings.arcaneWisdom = (key, isKeyboard);
			if (isKeyboard)
			{
				NALabel? currentlyModifiedKeyLabel = GUI.currentlyModifiedKeyLabel;
				string text;
				if (!KeyMap.TryGetValue((Keys)key, out string value))
				{
					Keys keys = (Keys)key;
					text = keys.ToString();
				}
				else
				{
					text = value;
				}
				currentlyModifiedKeyLabel.Text = text;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.arcaneWisdom.Item1);
			}
			break;
		}
		Settings.SaveSettings();
		new_binding = false;
		GUI.currentlyModifiedKey = "";
		GUI.currentlyModifiedKeyLabel = null;
	}

	public static void RestoreEditedHotkeyLabel()
	{
		if (GUI.currentlyModifiedKeyLabel == null || GUI.currentlyModifiedKey == "")
		{
			return;
		}
		int num = 0;
		string currentlyModifiedKey = GUI.currentlyModifiedKey;
		if (currentlyModifiedKey == null)
		{
			return;
		}
		switch (currentlyModifiedKey.Length)
		{
		case 8:
			switch (currentlyModifiedKey[0])
			{
			case 'u':
			{
				if (!(currentlyModifiedKey == "useBuffs"))
				{
					break;
				}
				(int, bool) useBuffs = Settings.config.ControlsSettings.useBuffs;
				(num, _) = useBuffs;
				if (useBuffs.Item2)
				{
					NALabel? currentlyModifiedKeyLabel10 = GUI.currentlyModifiedKeyLabel;
					string text10;
					if (!KeyMap.TryGetValue((Keys)num, out string value10))
					{
						Keys keys = (Keys)num;
						text10 = keys.ToString();
					}
					else
					{
						text10 = value10;
					}
					currentlyModifiedKeyLabel10.Text = text10;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffs.Item1);
				}
				break;
			}
			case 'j':
			{
				if (!(currentlyModifiedKey == "joinList"))
				{
					break;
				}
				(int, bool) joinList = Settings.config.ControlsSettings.joinList;
				(num, _) = joinList;
				if (joinList.Item2)
				{
					NALabel? currentlyModifiedKeyLabel12 = GUI.currentlyModifiedKeyLabel;
					string text12;
					if (!KeyMap.TryGetValue((Keys)num, out string value12))
					{
						Keys keys = (Keys)num;
						text12 = keys.ToString();
					}
					else
					{
						text12 = value12;
					}
					currentlyModifiedKeyLabel12.Text = text12;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.joinList.Item1);
				}
				break;
			}
			case 'e':
			{
				if (!(currentlyModifiedKey == "exitRaid"))
				{
					break;
				}
				(int, bool) exitRaid = Settings.config.ControlsSettings.exitRaid;
				(num, _) = exitRaid;
				if (exitRaid.Item2)
				{
					NALabel? currentlyModifiedKeyLabel11 = GUI.currentlyModifiedKeyLabel;
					string text11;
					if (!KeyMap.TryGetValue((Keys)num, out string value11))
					{
						Keys keys = (Keys)num;
						text11 = keys.ToString();
					}
					else
					{
						text11 = value11;
					}
					currentlyModifiedKeyLabel11.Text = text11;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.exitRaid.Item1);
				}
				break;
			}
			case 'm':
			{
				if (!(currentlyModifiedKey == "massHeal"))
				{
					break;
				}
				(int, bool) massHeal = Settings.config.ControlsSettings.massHeal;
				(num, _) = massHeal;
				if (massHeal.Item2)
				{
					NALabel? currentlyModifiedKeyLabel9 = GUI.currentlyModifiedKeyLabel;
					string text9;
					if (!KeyMap.TryGetValue((Keys)num, out string value9))
					{
						Keys keys = (Keys)num;
						text9 = keys.ToString();
					}
					else
					{
						text9 = value9;
					}
					currentlyModifiedKeyLabel9.Text = text9;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.massHeal.Item1);
				}
				break;
			}
			}
			break;
		case 6:
			switch (currentlyModifiedKey[0])
			{
			case 'i':
			{
				if (!(currentlyModifiedKey == "invite"))
				{
					break;
				}
				(int, bool) invite = Settings.config.ControlsSettings.invite;
				(num, _) = invite;
				if (invite.Item2)
				{
					NALabel? currentlyModifiedKeyLabel8 = GUI.currentlyModifiedKeyLabel;
					string text8;
					if (!KeyMap.TryGetValue((Keys)num, out string value8))
					{
						Keys keys = (Keys)num;
						text8 = keys.ToString();
					}
					else
					{
						text8 = value8;
					}
					currentlyModifiedKeyLabel8.Text = text8;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.invite.Item1);
				}
				break;
			}
			case 'w':
			{
				if (!(currentlyModifiedKey == "wearSP"))
				{
					break;
				}
				(int, bool) wearSP = Settings.config.ControlsSettings.wearSP;
				(num, _) = wearSP;
				if (wearSP.Item2)
				{
					NALabel? currentlyModifiedKeyLabel7 = GUI.currentlyModifiedKeyLabel;
					string text7;
					if (!KeyMap.TryGetValue((Keys)num, out string value7))
					{
						Keys keys = (Keys)num;
						text7 = keys.ToString();
					}
					else
					{
						text7 = value7;
					}
					currentlyModifiedKeyLabel7.Text = text7;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.wearSP.Item1);
				}
				break;
			}
			}
			break;
		case 12:
			switch (currentlyModifiedKey[0])
			{
			case 'u':
			{
				if (!(currentlyModifiedKey == "useSelfBuffs"))
				{
					break;
				}
				(int, bool) useSelfBuffs = Settings.config.ControlsSettings.useSelfBuffs;
				(num, _) = useSelfBuffs;
				if (useSelfBuffs.Item2)
				{
					NALabel? currentlyModifiedKeyLabel3 = GUI.currentlyModifiedKeyLabel;
					string text3;
					if (!KeyMap.TryGetValue((Keys)num, out string value3))
					{
						Keys keys = (Keys)num;
						text3 = keys.ToString();
					}
					else
					{
						text3 = value3;
					}
					currentlyModifiedKeyLabel3.Text = text3;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useSelfBuffs.Item1);
				}
				break;
			}
			case 'a':
			{
				if (!(currentlyModifiedKey == "arcaneWisdom"))
				{
					break;
				}
				(int, bool) arcaneWisdom = Settings.config.ControlsSettings.arcaneWisdom;
				(num, _) = arcaneWisdom;
				if (arcaneWisdom.Item2)
				{
					NALabel? currentlyModifiedKeyLabel2 = GUI.currentlyModifiedKeyLabel;
					string text2;
					if (!KeyMap.TryGetValue((Keys)num, out string value2))
					{
						Keys keys = (Keys)num;
						text2 = keys.ToString();
					}
					else
					{
						text2 = value2;
					}
					currentlyModifiedKeyLabel2.Text = text2;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.arcaneWisdom.Item1);
				}
				break;
			}
			}
			break;
		case 11:
			switch (currentlyModifiedKey[10])
			{
			case '1':
			{
				if (!(currentlyModifiedKey == "useBuffset1"))
				{
					break;
				}
				(int, bool) useBuffset3 = Settings.config.ControlsSettings.useBuffset1;
				(num, _) = useBuffset3;
				if (useBuffset3.Item2)
				{
					NALabel? currentlyModifiedKeyLabel6 = GUI.currentlyModifiedKeyLabel;
					string text6;
					if (!KeyMap.TryGetValue((Keys)num, out string value6))
					{
						Keys keys = (Keys)num;
						text6 = keys.ToString();
					}
					else
					{
						text6 = value6;
					}
					currentlyModifiedKeyLabel6.Text = text6;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset1.Item1);
				}
				break;
			}
			case '2':
			{
				if (!(currentlyModifiedKey == "useBuffset2"))
				{
					break;
				}
				(int, bool) useBuffset2 = Settings.config.ControlsSettings.useBuffset2;
				(num, _) = useBuffset2;
				if (useBuffset2.Item2)
				{
					NALabel? currentlyModifiedKeyLabel5 = GUI.currentlyModifiedKeyLabel;
					string text5;
					if (!KeyMap.TryGetValue((Keys)num, out string value5))
					{
						Keys keys = (Keys)num;
						text5 = keys.ToString();
					}
					else
					{
						text5 = value5;
					}
					currentlyModifiedKeyLabel5.Text = text5;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset2.Item1);
				}
				break;
			}
			case '3':
			{
				if (!(currentlyModifiedKey == "useBuffset3"))
				{
					break;
				}
				(int, bool) useBuffset = Settings.config.ControlsSettings.useBuffset3;
				(num, _) = useBuffset;
				if (useBuffset.Item2)
				{
					NALabel? currentlyModifiedKeyLabel4 = GUI.currentlyModifiedKeyLabel;
					string text4;
					if (!KeyMap.TryGetValue((Keys)num, out string value4))
					{
						Keys keys = (Keys)num;
						text4 = keys.ToString();
					}
					else
					{
						text4 = value4;
					}
					currentlyModifiedKeyLabel4.Text = text4;
				}
				else
				{
					GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset3.Item1);
				}
				break;
			}
			}
			break;
		case 10:
		{
			if (!(currentlyModifiedKey == "useDebuffs"))
			{
				break;
			}
			(int, bool) useDebuffs = Settings.config.ControlsSettings.useDebuffs;
			(num, _) = useDebuffs;
			if (useDebuffs.Item2)
			{
				NALabel? currentlyModifiedKeyLabel = GUI.currentlyModifiedKeyLabel;
				string text;
				if (!KeyMap.TryGetValue((Keys)num, out string value))
				{
					Keys keys = (Keys)num;
					text = keys.ToString();
				}
				else
				{
					text = value;
				}
				currentlyModifiedKeyLabel.Text = text;
			}
			else
			{
				GUI.currentlyModifiedKeyLabel.Text = IntToMouseButtonName(Settings.config.ControlsSettings.useDebuffs.Item1);
			}
			break;
		}
		case 7:
		case 9:
			break;
		}
	}

	private static async void handleHotbarPageChanged()
	{
		nint foregroundHWND = DllImports.GetForegroundWindow();
		NostaleCharacterInfo character = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == foregroundHWND);
		if (character != null)
		{
			await Task.Delay(100);
			character.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)character.process_id);
		}
	}

	public static string KeyToString(Keys key)
	{
		if (!KeyMap.TryGetValue(key, out string value))
		{
			return key.ToString();
		}
		return value;
	}

	public static string IntToMouseButtonName(int id)
	{
		if (MouseButtonsMap.ContainsKey(id))
		{
			return MouseButtonsMap[id];
		}
		return $"MB{id}";
	}
}
