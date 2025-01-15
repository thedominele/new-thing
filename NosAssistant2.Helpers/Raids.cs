using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NosAssistant2.Configs;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;
using NosAssistant2.GUIElements;

namespace NosAssistant2.Helpers;

public static class Raids
{
	public static ConcurrentQueue<(NostaleCharacterInfo, string)> performActionQueue = new ConcurrentQueue<(NostaleCharacterInfo, string)>();

	public static bool joining = false;

	public static bool allowJoining = false;

	public static bool performingAction = false;

	public static bool aborting_task_running = false;

	public static int joined_raid_id = -1;

	public static bool isPickingUpGold = false;

	public static Point random_point = default(Point);

	public static Point random_point2 = default(Point);

	public static List<KeyValuePair<int, NostaleCharacterInfo>> readyRaiders { get; set; } = new List<KeyValuePair<int, NostaleCharacterInfo>>();


	public static List<NostaleCharacterInfo> readyForOpenList { get; set; } = new List<NostaleCharacterInfo>();


	public static bool isMovingToField { get; set; } = false;


	public static bool isOpeningBoxes { get; set; } = false;


	public static bool isUsingArcaneWisdom { get; set; } = false;


	public static (Keys, bool) findInHotbar(NostaleCharacterInfo character, List<int> itemsIDs, string itemName, bool isItem, bool alert = true)
	{
		character.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)character.process_id);
		HotBarElement hotBarElement = character.hotBar.Find((HotBarElement x) => itemsIDs.Contains(x.SlotValue) && x.isItem == isItem);
		if (hotBarElement == null)
		{
			if (alert)
			{
				GUI.ShowPopUp(character.nickname + " has no " + itemName + " in the hotbar");
			}
			return (Keys.None, false);
		}
		if (Controller.HotBarSlotIDToKeyDict.TryGetValue(hotBarElement.HotBarSlotID, out (Keys, bool) value))
		{
			var (item, item2) = value;
			return (item, item2);
		}
		return (Keys.None, false);
	}

	public static async Task allJoinList(int startIndex)
	{
		allowJoining = false;
		readyRaiders = readyRaiders.OrderBy<KeyValuePair<int, NostaleCharacterInfo>, int>((KeyValuePair<int, NostaleCharacterInfo> x) => GUI._nostaleCharacterInfoList.FindIndex((NostaleCharacterInfo y) => y.hwnd == x.Value.hwnd)).ToList();
		List<KeyValuePair<int, NostaleCharacterInfo>> list = readyRaiders.Where<KeyValuePair<int, NostaleCharacterInfo>>((KeyValuePair<int, NostaleCharacterInfo> pair) => !pair.Value.config.sideZenas).ToList();
		List<KeyValuePair<int, NostaleCharacterInfo>> list2 = readyRaiders.Where<KeyValuePair<int, NostaleCharacterInfo>>((KeyValuePair<int, NostaleCharacterInfo> pair) => pair.Value.config.sideZenas).ToList();
		bool flag = startIndex % 2 == 1;
		List<KeyValuePair<int, NostaleCharacterInfo>> firstList = (flag ? list : list2);
		List<KeyValuePair<int, NostaleCharacterInfo>> secondList = (flag ? list2 : list);
		int maxLength = Math.Max(firstList.Count, secondList.Count);
		await PrepareJoinList();
		for (int i = 0; i < maxLength; i++)
		{
			if (i < firstList.Count)
			{
				Controller.SingleButtonPress(firstList.ElementAt(i).Value.hwnd, Keys.Return);
				await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Raid));
			}
			if (i < secondList.Count)
			{
				Controller.SingleButtonPress(secondList.ElementAt(i).Value.hwnd, Keys.Return);
				await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Raid));
			}
		}
		await Task.Delay(200);
		Controller.MultiButtonPress(GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => readyRaiders.Any<KeyValuePair<int, NostaleCharacterInfo>>((KeyValuePair<int, NostaleCharacterInfo> y) => y.Value.hwnd == x.hwnd)).ToList(), Keys.Escape);
		readyRaiders.Clear();
		await Task.Delay(2000);
		Controller.MultiButtonPress(GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => readyRaiders.Any<KeyValuePair<int, NostaleCharacterInfo>>((KeyValuePair<int, NostaleCharacterInfo> y) => y.Value.hwnd == x.hwnd)).ToList(), Keys.Escape);
		joining = false;
	}

	public static async Task MoveCharsFromResolution(List<NostaleCharacterInfo> movers, int X, int Y, int randomizeCoord = 0)
	{
		foreach (NostaleCharacterInfo mover in movers)
		{
			DllImports.RECT rect = default(DllImports.RECT);
			DllImports.GetClientRect(mover.hwnd, ref rect);
			int num = rect.right - Utils.randomizeCoord(X, randomizeCoord);
			int num2 = rect.top + Utils.randomizeCoord(Y, randomizeCoord);
			Controller.ClickInBackground((short)num, (short)num2, mover.hwnd);
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Move));
		}
	}

	public static async Task MoveOneCharFromResolution(nint HWND, int X, int Y)
	{
		DllImports.RECT rect = default(DllImports.RECT);
		DllImports.GetClientRect(HWND, ref rect);
		int num = rect.right - X;
		int num2 = rect.top + Y;
		await Controller.ClickInBackground((short)num, (short)num2, HWND);
	}

	public static async void OpenList()
	{
		joining = true;
		int raidListSkillID = 11;
		if (GUI.raidHost == "")
		{
			joining = false;
			allowJoining = false;
			GUI.ShowPopUp("Raid host was not selected");
			return;
		}
		foreach (NostaleCharacterInfo raider in readyForOpenList)
		{
			if (raider.nickname == GUI.raidHost)
			{
				continue;
			}
			if (raider.inRaid)
			{
				GUI.ShowPopUp(raider.nickname + " is already in raid");
				continue;
			}
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Raid));
			var (keys, flag) = findInHotbar(raider, new List<int> { raidListSkillID }, "raid list", isItem: false);
			if (keys == Keys.None)
			{
				joining = false;
				allowJoining = false;
				await Task.Delay(300);
				Controller.MultiButtonPress(readyForOpenList, Keys.Escape);
				readyForOpenList.Clear();
				return;
			}
			if (flag)
			{
				Controller.buttonWithControl(raider.hwnd, keys);
			}
			else
			{
				Controller.SingleButtonPress(raider.hwnd, keys);
			}
		}
		readyForOpenList.Clear();
	}

	public static void JoinList()
	{
		if (joining)
		{
			return;
		}
		readyForOpenList = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.inRaid && !x.config.isDisabled && x.nickname != GUI.raidHost).ToList();
		if (readyForOpenList.Count == 0)
		{
			return;
		}
		allowJoining = true;
		if (!aborting_task_running)
		{
			Task.Run(async delegate
			{
				aborting_task_running = true;
				await Task.Delay(60000);
				allowJoining = false;
				joining = false;
				readyRaiders.Clear();
				aborting_task_running = false;
			});
		}
		OpenList();
	}

	public static async Task SingleJoinList(nint hwnd, int RaidListNumber)
	{
		NostaleCharacterInfo raider = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == hwnd);
		if (raider != null)
		{
			await Task.Delay(400);
			DllImports.RECT window = default(DllImports.RECT);
			DllImports.GetClientRect(raider.hwnd, ref window);
			int num = (window.right - window.left) / 2;
			int num2 = (window.bottom - window.top) / 2 - 164 + (22 * RaidListNumber - 1);
			Controller.ClickInBackground((short)num, (short)num2, raider.hwnd);
			await Task.Delay(200);
			int num3 = (window.right - window.left) / 2 + 128;
			num2 = (window.bottom - window.top) / 2 + 200;
			Controller.ClickInBackground((short)num3, (short)num2, raider.hwnd);
			await Task.Delay(400);
			Controller.SingleButtonPress(raider.hwnd, Keys.Return);
			await Task.Delay(300);
			Controller.SingleButtonPress(raider.hwnd, Keys.Escape);
		}
	}

	public static async Task PrepareJoinList()
	{
		bool flag = false;
		if (joined_raid_id == 16 || joined_raid_id == 17)
		{
			foreach (KeyValuePair<int, NostaleCharacterInfo> readyRaider in readyRaiders)
			{
				Keys keys = Keys.None;
				bool flag2 = false;
				NostaleCharacterInfo value = readyRaider.Value;
				if (joined_raid_id == 16)
				{
					(keys, flag2) = findInHotbar(value, new List<int>(1) { 4503 }, "Draco Amulet", isItem: true, alert: false);
					if (keys == Keys.None)
					{
						continue;
					}
				}
				else if (joined_raid_id == 17)
				{
					(keys, flag2) = findInHotbar(value, new List<int>(1) { 4504 }, "Glacerus Amulet", isItem: true, alert: false);
					if (keys == Keys.None)
					{
						continue;
					}
				}
				if (flag2)
				{
					Controller.buttonWithControl(value.hwnd, keys);
				}
				else
				{
					Controller.SingleButtonPress(value.hwnd, keys);
				}
				await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Raid));
				flag = true;
			}
		}
		if (!flag)
		{
			await Task.Delay(400);
		}
		foreach (KeyValuePair<int, NostaleCharacterInfo> readyRaider2 in readyRaiders)
		{
			NostaleCharacterInfo value2 = readyRaider2.Value;
			int key = readyRaider2.Key;
			DllImports.RECT rect = default(DllImports.RECT);
			DllImports.GetClientRect(value2.hwnd, ref rect);
			int num = (rect.right - rect.left) / 2;
			int num2 = (rect.bottom - rect.top) / 2 - 197 + 22 * key;
			Controller.ClickInBackground((short)num, (short)num2, value2.hwnd);
		}
		await Task.Delay(200);
		foreach (KeyValuePair<int, NostaleCharacterInfo> readyRaider3 in readyRaiders)
		{
			NostaleCharacterInfo value3 = readyRaider3.Value;
			_ = readyRaider3.Key;
			DllImports.RECT rect2 = default(DllImports.RECT);
			DllImports.GetClientRect(value3.hwnd, ref rect2);
			int num3 = (rect2.right - rect2.left) / 2;
			int num4 = (rect2.bottom - rect2.top) / 2 + 200;
			Controller.ClickInBackground((short)num3, (short)num4, value3.hwnd);
		}
		await Task.Delay(400);
	}

	private static bool ZenasIsLeftSide(NostaleCharacterInfo character)
	{
		return character.x_pos < 120;
	}

	public static async Task ExitRaid()
	{
		List<NostaleCharacterInfo> raiders = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled && x.inRaid && x.nickname != GUI.raidHost).ToList();
		foreach (NostaleCharacterInfo item in raiders)
		{
			DllImports.RECT rect = default(DllImports.RECT);
			DllImports.GetClientRect(item.hwnd, ref rect);
			int num = rect.right - 10;
			int num2 = 216;
			Controller.ClickInBackground((short)num, (short)num2, item.hwnd);
		}
		await Task.Delay(200);
		await Controller.MultiButtonPress(raiders, Keys.Return, Settings.config.DelaySettings.Raid);
		await Task.Delay(200);
		await Controller.MultiButtonPress(raiders, Keys.Escape, Settings.config.DelaySettings.Raid);
	}

	public static async void MoveCharacter(NostaleCharacterInfo character, string curRaid, int option)
	{
		Controller.SingleButtonPress(character.hwnd, Keys.Escape);
		if (curRaid == "Kirolas" && option == 1)
		{
			if (!RaidManager.raidStarted || !character.inRaid || !character.config.isMover || character.config.isDisabled || character.real_map_id != 2649)
			{
				return;
			}
			RaidDamage raidDamage = RaidManager.currentList.Find((RaidDamage x) => x.CharacterID == character.character_id);
			if (raidDamage == null)
			{
				return;
			}
			int index = RaidManager.currentList.IndexOf(raidDamage);
			if (index == -1 || index == 0 || index >= 8)
			{
				return;
			}
			await Task.Delay(new Random().Next(2500, 4000));
			Point point = new List<Point>
			{
				new Point(134, 89),
				new Point(144, 91),
				new Point(151, 101),
				new Point(151, 112),
				new Point(140, 118),
				new Point(131, 121),
				new Point(151, 106)
			}[index - 1];
			await MoveOneCharFromResolution(character.hwnd, point.X, point.Y);
		}
		if (curRaid == "Erenia")
		{
			if (option == 1)
			{
				if (!Settings.config.WaypointsConfig.SpreadCharacters)
				{
					await MoveOneCharFromResolution(character.hwnd, random_point.X, random_point.Y);
				}
				else
				{
					await MoveOneCharFromResolution(character.hwnd, Utils.randomizeCoord(107, 1), Utils.randomizeCoord(103, 1));
				}
			}
			if (option == 2)
			{
				await MoveOneCharFromResolution(character.hwnd, 111, 103);
				await Task.Delay(3000);
				await MoveOneCharFromResolution(character.hwnd, 82, 158);
				await Task.Delay(20000);
				await MoveOneCharFromResolution(character.hwnd, 46, 111);
			}
			if (option == 3)
			{
				await MoveOneCharFromResolution(character.hwnd, 41, 80);
			}
		}
		if (curRaid == "Zenas")
		{
			bool leftSide = ZenasIsLeftSide(character);
			if (option == 1)
			{
				if (!Settings.config.WaypointsConfig.SpreadCharacters)
				{
					await MoveOneCharFromResolution(character.hwnd, random_point.X, random_point.Y);
					return;
				}
				if (leftSide)
				{
					if (!Settings.config.WaypointsConfig.SpreadCharacters)
					{
						await MoveOneCharFromResolution(character.hwnd, random_point.X, random_point.Y);
					}
					else
					{
						int x2 = Utils.randomizeCoord(108, 2);
						int y = Utils.randomizeCoord(101, 2);
						await MoveOneCharFromResolution(character.hwnd, x2, y);
					}
				}
				else if (!Settings.config.WaypointsConfig.SpreadCharacters)
				{
					await MoveOneCharFromResolution(character.hwnd, random_point2.X, random_point2.Y);
				}
				else
				{
					Point point2 = PickRandomZenasRightPoint();
					await MoveOneCharFromResolution(character.hwnd, point2.X, point2.Y);
				}
			}
			if (option == 2)
			{
				if (!leftSide)
				{
					await MoveOneCharFromResolution(character.hwnd, 47, 92);
				}
				else
				{
					await MoveOneCharFromResolution(character.hwnd, 111, 102);
				}
				await Task.Delay(2000);
				await MoveOneCharFromResolution(character.hwnd, 79, 143);
			}
			if (option == 3)
			{
				await Task.Delay(3000);
				await MoveOneCharFromResolution(character.hwnd, 79, 145);
			}
		}
		if (curRaid == "Valehir" && option == 1)
		{
			await MoveOneCharFromResolution(character.hwnd, Utils.randomizeCoord(95, 1), Utils.randomizeCoord(77, 1));
			await Task.Delay(14500);
			await MoveOneCharFromResolution(character.hwnd, Utils.randomizeCoord(72, 1), Utils.randomizeCoord(82, 1));
			await Task.Delay(2500);
			await MoveOneCharFromResolution(character.hwnd, 71, 174);
			await Task.Delay(13000);
			await MoveOneCharFromResolution(character.hwnd, 146, 172);
		}
	}

	public static async void MoveCharacters(string curRaid, int option)
	{
		List<NostaleCharacterInfo> list = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isMover && x.inRaid && !x.config.isDisabled).ToList();
		foreach (NostaleCharacterInfo item in list)
		{
			MoveCharacter(item, curRaid, option);
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Move));
		}
	}

	public static async Task ChangeAmulets()
	{
		List<NostaleCharacterInfo> list = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled).ToList();
		foreach (NostaleCharacterInfo item in list)
		{
			useItem(item.hwnd, new List<int> { 4503, 4504 }, "Amulet", isItem: true, restrict: false);
			await Task.Delay(Settings.config.DelaySettings.Items);
		}
	}

	public static async Task TransformSP()
	{
		List<NostaleCharacterInfo> raiders = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isAttacker && !x.config.isDisabled).ToList();
		await Controller.MultiButtonPress(raiders, Keys.G, Settings.config.DelaySettings.Raid);
		await Controller.MultiButtonPress(raiders, Keys.H, Settings.config.DelaySettings.Raid);
	}

	public static void closeAlts()
	{
		if (new NAMessageBox("Are you sure you want to close all your alts?", "Closing Alts").ShowDialog() != DialogResult.Yes)
		{
			return;
		}
		foreach (NostaleCharacterInfo item in GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => !x.config.isAttacker).ToList())
		{
			Controller.closeWindow(item.process_id);
		}
	}

	public static async void useItem(nint hwnd, List<int> itemsIDs, string itemsDescription, bool isItem, bool restrict, int delay = 0)
	{
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.hwnd == hwnd);
		if (nostaleCharacterInfo == null || nostaleCharacterInfo.current_hp == 0)
		{
			return;
		}
		int num = 1000;
		if (!(Math.Abs((DateTime.UtcNow - nostaleCharacterInfo.lastItemUsed).TotalMilliseconds) < (double)num && restrict))
		{
			var (itemKey, pressControl) = findInHotbar(nostaleCharacterInfo, itemsIDs, itemsDescription, isItem);
			if (restrict)
			{
				nostaleCharacterInfo.lastItemUsed = DateTime.UtcNow;
			}
			await Task.Delay(delay);
			if (!pressControl)
			{
				Controller.SingleButtonPress(hwnd, itemKey);
			}
			else
			{
				Controller.buttonWithControl(hwnd, itemKey);
			}
		}
	}

	public static void useFullHPPotion(NostaleCharacterInfo character)
	{
		int num = 300;
		if (!(Math.Abs((DateTime.UtcNow - character.lastFullUsed).TotalMilliseconds) < (double)num))
		{
			character.lastFullUsed = DateTime.UtcNow;
			useItem(character.hwnd, ItemID.FullPotions, "Full Pottion", isItem: true, restrict: false);
		}
	}

	public static async void QFCSplit()
	{
		short[] QFCArrayX = new short[10] { 954, 978, 983, 968, 944, 922, 914, 931, 954, 978 };
		short[] QFCArrayY = new short[10] { 112, 106, 86, 61, 54, 69, 91, 110, 112, 106 };
		List<NostaleCharacterInfo> raiders = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled).ToList();
		for (int i = 0; i < raiders.Count; i++)
		{
			nint hwnd = raiders.ElementAt(i).hwnd;
			short x2 = QFCArrayX[i];
			short y = QFCArrayY[i];
			await Controller.ClickInBackground(x2, y, hwnd);
		}
	}

	public static async void FeedPets()
	{
		List<NostaleCharacterInfo> raiders = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled).ToList();
		foreach (NostaleCharacterInfo item in raiders)
		{
			DllImports.RECT rect = default(DllImports.RECT);
			DllImports.GetClientRect(item.hwnd, ref rect);
			int num = rect.left + 150;
			int num2 = rect.top + 260;
			Controller.ClickInBackground((short)num, (short)num2, item.hwnd);
		}
		await Task.Delay(100);
		for (int j = 0; j < 5; j++)
		{
			Keys key = Keys.D9;
			await Controller.MultiButtonPress(raiders, key, Settings.config.DelaySettings.Raid);
			await Task.Delay(Math.Max(750 - raiders.Count * Settings.config.DelaySettings.Raid, 0));
		}
	}

	public static async Task UseArcaneWisdom()
	{
		if (isUsingArcaneWisdom)
		{
			return;
		}
		List<NostaleCharacterInfo> list = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isAttacker && !x.config.isDisabled).ToList();
		if (list.Count == 0)
		{
			GUI.ShowPopUp("No attackers were selected!");
			return;
		}
		isUsingArcaneWisdom = true;
		Dictionary<nint, List<(Keys Key, bool ControlPressed)>> KeysToPress = new Dictionary<nint, List<(Keys, bool)>>();
		foreach (NostaleCharacterInfo attacker in list)
		{
			Controller.SingleButtonPress(attacker.hwnd, Keys.Tab);
			await Task.Delay(100);
			List<int> tattoos_list = SPCard.tattoos_list;
			attacker.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)attacker.process_id);
			foreach (int tattooID in tattoos_list)
			{
				HotBarElement hotBarElement = attacker.hotBar.Find((HotBarElement x) => x.SlotValue == tattooID && !x.isItem);
				if (hotBarElement != null)
				{
					if (!KeysToPress.Keys.Contains(attacker.hwnd))
					{
						KeysToPress[attacker.hwnd] = new List<(Keys, bool)>();
					}
					KeysToPress[attacker.hwnd].Add(Controller.HotBarSlotIDToKeyDict[hotBarElement.HotBarSlotID]);
				}
			}
			await Task.Delay(100);
			changeEq(attacker.hwnd);
			Keys tattoo1Key;
			bool press_control;
			if (KeysToPress.Values.Count > 0 && KeysToPress.Values.ElementAt(0).Count > 0)
			{
				(tattoo1Key, press_control) = KeysToPress.Values.ElementAt(0)[0];
			}
			else
			{
				tattoo1Key = Keys.None;
				press_control = false;
			}
			if (tattoo1Key != 0)
			{
				await Task.Delay(300);
				if (!press_control)
				{
					Controller.SingleButtonPress(attacker.hwnd, tattoo1Key);
				}
				else
				{
					Controller.buttonWithControl(attacker.hwnd, tattoo1Key);
				}
				await Task.Delay(50);
				if (!press_control)
				{
					Controller.SingleButtonPress(attacker.hwnd, tattoo1Key);
				}
				else
				{
					Controller.buttonWithControl(attacker.hwnd, tattoo1Key);
				}
				await Task.Delay(50);
				if (!press_control)
				{
					Controller.SingleButtonPress(attacker.hwnd, tattoo1Key);
				}
				else
				{
					Controller.buttonWithControl(attacker.hwnd, tattoo1Key);
				}
			}
			Keys tattoo2Key;
			if (KeysToPress.Values.Count > 0 && KeysToPress.Values.ElementAt(0).Count > 1)
			{
				(tattoo2Key, press_control) = KeysToPress.Values.ElementAt(0)[1];
			}
			else
			{
				tattoo2Key = Keys.None;
				press_control = false;
			}
			if (tattoo2Key != 0)
			{
				await Task.Delay(1000);
				if (!press_control)
				{
					Controller.SingleButtonPress(attacker.hwnd, tattoo2Key);
				}
				else
				{
					Controller.buttonWithControl(attacker.hwnd, tattoo2Key);
				}
				await Task.Delay(50);
				if (!press_control)
				{
					Controller.SingleButtonPress(attacker.hwnd, tattoo2Key);
				}
				else
				{
					Controller.buttonWithControl(attacker.hwnd, tattoo2Key);
				}
				await Task.Delay(50);
				if (!press_control)
				{
					Controller.SingleButtonPress(attacker.hwnd, tattoo2Key);
				}
				else
				{
					Controller.buttonWithControl(attacker.hwnd, tattoo2Key);
				}
			}
			await Task.Delay(300);
			changeEq(attacker.hwnd);
			await Task.Delay(100);
			Controller.SingleButtonPress(attacker.hwnd, Keys.Tab);
			isUsingArcaneWisdom = false;
		}
	}

	public static void changeEq(nint HWND)
	{
		Controller.SingleButtonPress(HWND, Keys.D1);
		Controller.SingleButtonPress(HWND, Keys.D2);
		Controller.SingleButtonPress(HWND, Keys.D3);
		Controller.SingleButtonPress(HWND, Keys.D4);
		Controller.SingleButtonPress(HWND, Keys.D5);
		Controller.SingleButtonPress(HWND, Keys.D6);
		Controller.SingleButtonPress(HWND, Keys.D7);
		Controller.SingleButtonPress(HWND, Keys.D8);
		Controller.SingleButtonPress(HWND, Keys.D9);
		Controller.SingleButtonPress(HWND, Keys.D0);
	}

	public static void handleTeleport(NostaleCharacterInfo character, int x, int y)
	{
		if (character == null)
		{
			return;
		}
		if (x == TeleportsLocations.ZenasBossroom.X && y == TeleportsLocations.ZenasBossroom.Y && character.real_map_id == 2602)
		{
			if (character.config.isDisabled || !character.config.isMover || !character.inRaid || character.config.isAttacker || !Settings.config.WaypointsConfig.ZenasBossroom)
			{
				return;
			}
			performActionQueue.Enqueue((character, "MoveZenasBossroom"));
			performAction();
		}
		if (x == TeleportsLocations.UltimateArmaBossRoom.X && y == TeleportsLocations.UltimateArmaBossRoom.Y && character.map_id == 2759)
		{
			RaidManager.IsInUltimateArmaBossroom = true;
		}
	}

	public static async void performAction()
	{
		if (performingAction)
		{
			return;
		}
		performingAction = true;
		(NostaleCharacterInfo character, string type) action;
		while (performActionQueue.TryDequeue(out action))
		{
			await Task.Delay(Utils.randomizeDelay(action.type.Contains("Move") ? Settings.config.DelaySettings.Move : Settings.config.DelaySettings.Items));
			switch (action.type)
			{
			case "ValehirDebuff":
				useItem(action.character.hwnd, new List<int> { 8555 }, "Anti-Venom Potion", isItem: true, restrict: true);
				break;
			case "AlzanorDebuff":
				useItem(action.character.hwnd, new List<int> { 8556 }, "Anti-Cold Potion", isItem: true, restrict: true);
				break;
			case "MoveZenasBossroom":
				MoveCharacter(action.character, "Zenas", 3);
				break;
			case "MoveZenasStart":
				MoveCharacter(action.character, "Zenas", 1);
				break;
			case "MoveEreniaStart":
				MoveCharacter(action.character, "Erenia", 1);
				break;
			case "MoveKirolas":
				MoveCharacter(action.character, "Kirolas", 1);
				break;
			}
		}
		performingAction = false;
	}

	public static async void MassHeal()
	{
		foreach (NostaleCharacterInfo item in GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => (x.config.isRaider || x.config.isBuffer) && !x.config.isDisabled && x.SPCard.ID == 7))
		{
			useItem(item.hwnd, new List<int> { 873 }, "Mass Heal", isItem: false, restrict: false);
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Buff));
		}
	}

	public static async void MimicMouse()
	{
		Point mouse_pos = Controller.getCursorPosOnWindow();
		await Task.Delay(30);
		Controller.MultiClickInBackground((short)mouse_pos.X, (short)mouse_pos.Y, GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled).ToList(), Settings.config.DelaySettings.Raid);
	}

	public static void MimicKeyboard(Keys key)
	{
		Controller.MultiButtonPress(GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled).ToList(), key, Settings.config.DelaySettings.Raid);
	}

	public static async Task UseDebuffs()
	{
		List<NostaleCharacterInfo> debbuffers = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isDebuffer && !x.config.isDisabled).ToList();
		if (debbuffers.Count == 0)
		{
			GUI.ShowPopUp("No Debuffers were selected!");
			return;
		}
		Dictionary<nint, List<(Keys Key, bool ControlPressed)>> KeysToPress = new Dictionary<nint, List<(Keys, bool)>>();
		foreach (NostaleCharacterInfo item in debbuffers)
		{
			KeysToPress[item.hwnd] = new List<(Keys, bool)>();
			if (item.SPCard == null)
			{
				continue;
			}
			item.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)item.process_id);
			foreach (int debuffID in SPCard.debuffs)
			{
				HotBarElement hotBarElement = item.hotBar.Find((HotBarElement x) => x.SlotValue == debuffID && !x.isItem);
				if (hotBarElement != null)
				{
					KeysToPress[item.hwnd].Add(Controller.HotBarSlotIDToKeyDict[hotBarElement.HotBarSlotID]);
				}
			}
		}
		int maxDebuffs = KeysToPress.Values.Max((List<(Keys Key, bool ControlPressed)> list) => list.Count);
		for (int i = 0; i < maxDebuffs; i++)
		{
			int used_debuffs = 0;
			foreach (NostaleCharacterInfo item2 in debbuffers)
			{
				if (KeysToPress[item2.hwnd].Count > i)
				{
					(Keys Key, bool ControlPressed) tuple = KeysToPress[item2.hwnd].ElementAt(i);
					var (keys, _) = tuple;
					if (tuple.ControlPressed)
					{
						Controller.buttonWithControl(item2.hwnd, keys);
					}
					else
					{
						Controller.SingleButtonPress(item2.hwnd, keys);
					}
					await Task.Delay(Math.Max(Utils.randomizeDelay(Settings.config.DelaySettings.Raid), 50));
					used_debuffs++;
				}
			}
			await Task.Delay(Math.Max(1800 - (Settings.config.DelaySettings.Raid - 70) * used_debuffs, 50));
		}
	}

	public static async Task OpenBoxes()
	{
		if (isOpeningBoxes)
		{
			return;
		}
		isOpeningBoxes = true;
		List<NostaleCharacterInfo> raiders = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isDisabled).ToList();
		while (isOpeningBoxes)
		{
			bool flag = false;
			bool isSealedTreasureChest = false;
			bool flag2 = false;
			List<NostaleCharacterInfo> has_box = new List<NostaleCharacterInfo>();
			foreach (NostaleCharacterInfo item in raiders)
			{
				var (keys, flag3) = findInHotbar(item, ItemID.Vessels, "Raid Box", isItem: true, alert: false);
				if (keys == Keys.None)
				{
					(keys, flag3) = findInHotbar(item, ItemID.Boxes, "Raid Box", isItem: true, alert: false);
					if (keys == Keys.None)
					{
						(keys, flag3) = findInHotbar(item, new List<int>(1) { 949 }, "Raid Box", isItem: true, alert: false);
						if (keys == Keys.None)
						{
							continue;
						}
						isSealedTreasureChest = true;
					}
				}
				else
				{
					flag = true;
				}
				has_box.Add(item);
				if (!flag3)
				{
					Controller.SingleButtonPress(item.hwnd, keys);
					flag2 = true;
				}
				else
				{
					Controller.buttonWithControl(item.hwnd, keys);
					flag2 = true;
				}
			}
			if (!flag2)
			{
				GUI.ShowPopUp("All boxes have been openend!", isNotification: true);
				isOpeningBoxes = false;
				GUI.form.SetOpenBoxesLabelText("Off");
			}
			if (!flag)
			{
				await Task.Delay(300 + Utils.randomizeDelay(100));
				if (!isSealedTreasureChest)
				{
					await Controller.MultiButtonPress(has_box, Keys.Return);
					await Task.Delay(300 + Utils.randomizeDelay(100));
				}
			}
			else
			{
				await Task.Delay(2000 + Utils.randomizeDelay(400));
			}
		}
		isOpeningBoxes = false;
	}

	public static void MoveCharacterToCoords(NostaleCharacterInfo character, int x, int y)
	{
		if (NAStyles.BitmapOriginal != null)
		{
			Bitmap bitmap = (Bitmap)NAStyles.BitmapOriginal.Clone();
			Size size = new Size(bitmap.Width / 2, bitmap.Height / 2);
			bitmap.Dispose();
			int num = 32;
			int num2 = 152;
			int num3 = 148;
			int num4 = (int)(double)(148 * x / size.Width);
			int num5 = (int)(double)(num3 * y / size.Height);
			MoveOneCharFromResolution(character.hwnd, num2 - num4, num + num5);
		}
	}

	public static async void MoveToGlacerusField()
	{
		await Task.Delay(Utils.randomizeDelay(800));
		List<NostaleCharacterInfo> list = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => !x.config.isDisabled && x.config.isRaider && x.inRaid && x.config.walkToFields).ToList();
		List<(NostaleCharacterInfo, GameField)> best_fields = new List<(NostaleCharacterInfo, GameField)>();
		foreach (NostaleCharacterInfo character in list)
		{
			GameField gameField = GUI.fields.OrderBy((GameField x) => Utils.CalculateDistance(new Point(character.x_pos, character.y_pos), new Point(x.x, x.y))).FirstOrDefault();
			if (gameField != null)
			{
				best_fields.Add((character, gameField));
			}
		}
		Task.Run(async delegate
		{
			bool first_run = true;
			while (GUI.fields.Count > 0)
			{
				if (first_run)
				{
					foreach (var (character2, gameField2) in best_fields)
					{
						MoveCharacterToCoords(character2, Utils.randomizeCoord(gameField2.x, 1), Utils.randomizeCoord(gameField2.y, 1));
						await Task.Delay(Settings.config.DelaySettings.Move);
					}
					first_run = false;
				}
				if (Math.Abs(best_fields.ElementAt(0).Item1.x_pos - best_fields.ElementAt(0).Item2.x) < 3 && Math.Abs(best_fields.ElementAt(0).Item1.y_pos - best_fields.ElementAt(0).Item2.y) < 3)
				{
					break;
				}
				MoveCharacterToCoords(best_fields.ElementAt(0).Item1, Utils.randomizeCoord(best_fields.ElementAt(0).Item2.x, 1), Utils.randomizeCoord(best_fields.ElementAt(0).Item2.y, 1));
				await Task.Delay(Utils.randomizeDelay(200));
			}
			isMovingToField = false;
		});
	}

	public static async void MoveToAsgobasField(int field_id, nint character_hwnd)
	{
		NostaleCharacterInfo current_character = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => !x.config.isDisabled && x.hwnd == character_hwnd && x.inRaid && x.config.isRaider && x.config.walkToFields).FirstOrDefault();
		if (current_character == null)
		{
			return;
		}
		await Task.Delay(Utils.randomizeDelay(800));
		GameField target_field = GUI.fields.Where((GameField x) => x.ID == field_id).FirstOrDefault();
		if (target_field == null)
		{
			return;
		}
		Task.Run(async delegate
		{
			while (GUI.fields.Count > 0)
			{
				if (Math.Abs(current_character.x_pos - target_field.x) > 8 || Math.Abs(current_character.y_pos - target_field.y) > 8)
				{
					MoveCharacterToCoords(current_character, Utils.randomizeCoord(target_field.x, 1), Utils.randomizeCoord(target_field.y, 1));
					await Task.Delay(Utils.randomizeDelay(400));
				}
				else
				{
					if (Math.Abs(current_character.x_pos - target_field.x) <= 3 && Math.Abs(current_character.y_pos - target_field.y) <= 3)
					{
						break;
					}
					MoveCharacterToCoords(current_character, Utils.randomizeCoord(target_field.x, 0), Utils.randomizeCoord(target_field.y, 0));
					await Task.Delay(Utils.randomizeDelay(400));
				}
			}
		});
	}

	public static void MoveToClosestPillar()
	{
		List<NostaleCharacterInfo> list = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => !x.config.isDisabled && x.inRaid && x.config.isRaider && x.config.walkToFields).ToList();
		List<(NostaleCharacterInfo, GameMonster)> best_pillars = new List<(NostaleCharacterInfo, GameMonster)>();
		foreach (NostaleCharacterInfo character in list)
		{
			GameMonster gameMonster = (from m in GUI.monsters
				where m.id == 3224
				select m into x
				orderby Utils.CalculateDistance(new Point(character.x_pos, character.y_pos), new Point(x.x, x.y))
				select x).FirstOrDefault();
			if (gameMonster != null)
			{
				best_pillars.Add((character, gameMonster));
			}
		}
		Task.Run(async delegate
		{
			foreach (var (character2, gameMonster2) in best_pillars)
			{
				MoveCharacterToCoords(character2, Utils.randomizeCoord(gameMonster2.x, 0), Utils.randomizeCoord(gameMonster2.y, 0));
				await Task.Delay(Settings.config.DelaySettings.Move);
			}
		});
	}

	public static async void PickUpGold()
	{
		NostaleCharacterInfo gold_picker = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.config.isGoldPicker && x.inRaid && !x.config.isDisabled && x.map_id == GUI.Mapper?.map_id);
		if (gold_picker == null)
		{
			isPickingUpGold = false;
			return;
		}
		List<GameEntity> gold_stacks = GUI.entities.Where((GameEntity x) => x.id == 1046).ToList();
		if (gold_stacks.Count == 0)
		{
			isPickingUpGold = false;
			return;
		}
		bool is_picker_picking_up = false;
		while (gold_stacks.Count != 0)
		{
			if (is_picker_picking_up)
			{
				await Task.Delay(100);
				continue;
			}
			List<GameEntity> source = gold_stacks.OrderBy((GameEntity x) => Utils.CalculateDistance(new Point(gold_picker.x_pos, gold_picker.y_pos), new Point(x.x, x.y))).ToList();
			GameEntity closest_gold_stack = source.First();
			if (closest_gold_stack == null)
			{
				break;
			}
			while (GUI.entities.Contains(closest_gold_stack))
			{
				double num = Utils.CalculateDistance(new Point(gold_picker.x_pos, gold_picker.y_pos), new Point(closest_gold_stack.x, closest_gold_stack.y));
				MoveCharacterToCoords(gold_picker, closest_gold_stack.x, closest_gold_stack.y);
				if (num < 3.0)
				{
					Controller.SingleButtonPress(gold_picker.hwnd, Keys.Oemtilde);
				}
				await Task.Delay(Utils.randomizeDelay(225));
			}
			is_picker_picking_up = false;
			await Task.Delay(100);
			gold_stacks = GUI.entities.Where((GameEntity x) => x.id == 1046).ToList();
		}
		isPickingUpGold = false;
	}

	public static Point PickRandomZenasRightPoint()
	{
		List<Point> list = new List<Point>
		{
			new Point(37, 88),
			new Point(37, 89),
			new Point(36, 88),
			new Point(36, 89)
		};
		int index = new Random().Next(0, list.Count);
		return list[index];
	}

	public static async void MoveByMapClick(int x, int y, int map_width, int map_height, string map_name)
	{
		if ((GUI.Mapper != null && (!TimeSpaceManager.ts_started || NAStyles.force_live_map_draw) && (!map_name.Contains("Quest") || (!NAvigator.show_time_space_map && !QuestManager.ShowQuestSearchInstanceMap))) || !(map_name != "Map"))
		{
			int num = 145;
			int num2 = 35;
			int num3 = 152;
			if (!RaidManager.IsInUltimateArmaBossroom)
			{
				x = num * x / map_width;
				y = num * y / map_height;
			}
			else
			{
				x = num * x / map_width;
				y = num * y / map_height;
				x = (int)((double)x * 150.0 / 316.0);
				y = (int)((double)y * 150.0 / 536.0);
				x += (int)((double)num3 * 83.0 / 316.0);
				y += (int)((double)(num3 - 8) * 386.0 / 536.0);
			}
			await MoveCharsFromResolution(GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isMover && !x.config.isDisabled && x.map_id == GUI.Mapper.map_id).ToList(), num3 - x, y + num2, Settings.config.randomizeCordsRange);
		}
	}
}
