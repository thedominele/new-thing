using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NosAssistant2.Configs;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class Miniland
{
	public static bool isInOwnMiniland = false;

	public static List<PetTrainerMob> pet_trainer_mobs_list = new List<PetTrainerMob>();

	public static List<MinilandPet> pets_list = new List<MinilandPet>();

	public static int trained_pets_count = 0;

	public static DateTime? trained_pets_count_change_date = null;

	public static async void InviteBuffersToML()
	{
		if (GUI.Inviter == null)
		{
			GUI.ShowPopUp("No Inviter choosen");
			return;
		}
		List<NostaleCharacterInfo> buffers = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isBuffer && !x.config.isDisabled && x.hwnd != GUI.Inviter.hwnd).ToList();
		if (buffers.Count == 0)
		{
			GUI.ShowPopUp("No Buffers have been selected");
			return;
		}
		Controller.PopWindow(GUI.Inviter.hwnd);
		await Task.Delay(200);
		Controller.ButtonPressForeground(Keys.Return);
		foreach (NostaleCharacterInfo item in buffers)
		{
			string text = "$Invite " + item.nickname;
			await ClipboardAddition.Run(delegate
			{
				Clipboard.SetText(text);
			});
			Controller.ButtonPressForeground(Keys.V, "ctrl");
			Controller.ButtonPressForeground(Keys.Return);
			await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Invite));
		}
		Controller.ButtonPressForeground(Keys.Escape);
		await Task.Delay(200);
	}

	public static async Task InvitePlayersToML()
	{
		if (GUI.Inviter == null)
		{
			GUI.ShowPopUp("No Inviter choosen");
			return;
		}
		if (GUI.Inviter.map_id != 20001)
		{
			GUI.ShowPopUp("Inviter is not in miniland");
			return;
		}
		List<InviteItem> nicknames = Settings.config.inviteList.Where((InviteItem x) => x.active).ToList();
		nicknames = nicknames.Where((InviteItem x) => !GUI.miniland_state.Any((GamePlayer y) => y.nickname == x.nickname)).ToList();
		if (nicknames.Count == 0)
		{
			GUI.ShowPopUp("No Players to invite");
			return;
		}
		await Controller.PopWindow(GUI.Inviter.hwnd);
		await Task.Delay(200);
		Controller.ButtonPressForeground(Keys.Return);
		foreach (InviteItem item in nicknames)
		{
			string text = "$Invite " + item.nickname;
			await ClipboardAddition.Run(delegate
			{
				Clipboard.SetText(text);
			});
			Controller.ButtonPressForeground(Keys.V, "ctrl");
			await Task.Delay(50);
			Controller.ButtonPressForeground(Keys.Return);
			await Task.Delay(Math.Max(Utils.randomizeDelay(Settings.config.DelaySettings.Invite), 100));
		}
		Controller.ButtonPressForeground(Keys.Escape);
		await Task.Delay(100);
		foreach (NostaleCharacterInfo character in GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => !x.config.isDisabled).ToList())
		{
			if (character != GUI.Inviter && nicknames.Any((InviteItem x) => x.nickname == character.nickname && x.active) && character.config.isAttacker)
			{
				Controller.PopWindow(character.hwnd);
			}
		}
	}

	public static async Task UseSelfBuffs()
	{
		List<NostaleCharacterInfo> self_buffers = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isAttacker && !x.config.isDisabled).ToList();
		if (self_buffers.Count == 0)
		{
			GUI.ShowPopUp("No Attackers were selected!");
			return;
		}
		Dictionary<nint, List<((Keys Key, bool ControlPressed), bool isTatoo)>> KeysToPress = new Dictionary<nint, List<((Keys, bool), bool)>>();
		foreach (NostaleCharacterInfo item4 in self_buffers)
		{
			KeysToPress[item4.hwnd] = new List<((Keys, bool), bool)>();
			if (item4.SPCard == null)
			{
				continue;
			}
			item4.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)item4.process_id);
			List<int> selfBuffsIDs = item4.SPCard.selfBuffsIDs;
			List<int> list2 = new List<int>(selfBuffsIDs.Count);
			list2.AddRange(selfBuffsIDs);
			list2.AddRange(SPCard.self_partners_buffs_list);
			list2.AddRange(SPCard.tattoos_list);
			foreach (int buffID in list2)
			{
				HotBarElement hotBarElement = item4.hotBar.Find((HotBarElement x) => x.SlotValue == buffID && !x.isItem);
				if (hotBarElement != null)
				{
					KeysToPress[item4.hwnd].Add((Controller.HotBarSlotIDToKeyDict[hotBarElement.HotBarSlotID], SPCard.tattoos_list.Contains(buffID)));
				}
			}
		}
		int maxNormalBuffs = KeysToPress.Values.Max((List<((Keys Key, bool ControlPressed), bool isTatoo)> list) => list.Count);
		for (int i = 0; i < maxNormalBuffs; i++)
		{
			int used_buffs = 0;
			int delay = 1800;
			foreach (NostaleCharacterInfo item5 in self_buffers)
			{
				if (KeysToPress[item5.hwnd].Count > i)
				{
					((Keys, bool), bool) tuple = KeysToPress[item5.hwnd].ElementAt(i);
					(Keys, bool) item = tuple.Item1;
					Keys item2 = item.Item1;
					bool item3 = item.Item2;
					bool isTatoo = tuple.Item2;
					if (item3)
					{
						Controller.buttonWithControl(item5.hwnd, item2);
					}
					else
					{
						Controller.SingleButtonPress(item5.hwnd, item2);
					}
					await Task.Delay(Math.Max(Utils.randomizeDelay(Settings.config.DelaySettings.Buff), 50));
					used_buffs++;
					if (self_buffers.Count == 1 && isTatoo)
					{
						delay = 800;
					}
				}
			}
			await Task.Delay(Utils.randomizeDelay(Math.Max(delay - (Settings.config.DelaySettings.Buff - 70) * used_buffs, 50)));
		}
	}

	public static async Task UseBuffs(int BuffsetID)
	{
		int maxPartnerBuffs = 0;
		Buffset current_buffset = null;
		new List<int>();
		List<int> allowedBuffs;
		if (BuffsetID == 0)
		{
			List<int> buffs_list = SkillID.buffs_list;
			List<int> list2 = new List<int>(buffs_list.Count);
			list2.AddRange(buffs_list);
			allowedBuffs = list2;
			allowedBuffs.Insert(0, 1585);
		}
		else
		{
			current_buffset = Settings.config.buffsets.ElementAt(BuffsetID - 1);
			List<int> buffs_list = current_buffset.buffset;
			List<int> list3 = new List<int>(buffs_list.Count);
			list3.AddRange(buffs_list);
			allowedBuffs = list3;
		}
		List<NostaleCharacterInfo> buffers = GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isBuffer && !x.config.isDisabled).ToList();
		if (buffers.Count == 0)
		{
			GUI.ShowPopUp("No buffers were selected!");
			return;
		}
		foreach (NostaleCharacterInfo item in buffers)
		{
			if (item.config.partnerBuffs.Length >= maxPartnerBuffs && item.config.partnerBuffs != "")
			{
				maxPartnerBuffs = item.config.partnerBuffs.Length;
			}
		}
		for (int i = 0; i < maxPartnerBuffs; i++)
		{
			if (current_buffset != null && !current_buffset.partner)
			{
				break;
			}
			foreach (NostaleCharacterInfo item2 in buffers)
			{
				if (item2.config.partnerBuffs.Length >= i && !(item2.config.partnerBuffs == ""))
				{
					char c = item2.config.partnerBuffs[i];
					Keys button = (Keys)Enum.Parse(typeof(Keys), c.ToString(), ignoreCase: true);
					Controller.buttonWithShift(item2.hwnd, button);
					await Task.Delay(100 + Utils.randomizeDelay(Settings.config.DelaySettings.Buff));
				}
			}
			await Task.Delay(Math.Max(1500 - Settings.config.DelaySettings.Buff * maxPartnerBuffs, 0));
		}
		Dictionary<nint, List<(Keys Key, bool ControlPressed)>> KeysToPress = new Dictionary<nint, List<(Keys, bool)>>();
		foreach (NostaleCharacterInfo item3 in buffers)
		{
			KeysToPress[item3.hwnd] = new List<(Keys, bool)>();
			if (item3.SPCard == null)
			{
				continue;
			}
			item3.hotBar = Crypto.ReadCurrentHotbarFromMemory((int)item3.process_id);
			List<int> buffs_list = item3.SPCard.BuffsIDs;
			List<int> list4 = new List<int>(buffs_list.Count);
			list4.AddRange(buffs_list);
			List<int> list5 = list4;
			if (current_buffset == null || (current_buffset != null && current_buffset.partner))
			{
				list5.Insert(0, SPCard.group_partners_buffs_list.ElementAt(0));
				list5.Add(SPCard.group_partners_buffs_list.ElementAt(1));
			}
			foreach (int buffID in list5)
			{
				if (allowedBuffs.Contains(buffID) || SPCard.isGroupPartnerBuff(buffID))
				{
					HotBarElement hotBarElement = item3.hotBar.Find((HotBarElement x) => x.SlotValue == buffID && !x.isItem);
					if (hotBarElement != null)
					{
						KeysToPress[item3.hwnd].Add(Controller.HotBarSlotIDToKeyDict[hotBarElement.HotBarSlotID]);
					}
				}
			}
		}
		int maxNormalBuffs = KeysToPress.Values.Max((List<(Keys Key, bool ControlPressed)> list) => list.Count);
		for (int i = 0; i < maxNormalBuffs; i++)
		{
			int used_buffs = 0;
			foreach (NostaleCharacterInfo item4 in buffers)
			{
				if (KeysToPress[item4.hwnd].Count > i)
				{
					(Keys Key, bool ControlPressed) tuple = KeysToPress[item4.hwnd].ElementAt(i);
					var (keys, _) = tuple;
					if (tuple.ControlPressed)
					{
						Controller.buttonWithControl(item4.hwnd, keys);
					}
					else
					{
						Controller.SingleButtonPress(item4.hwnd, keys);
					}
					await Task.Delay(Math.Max(Utils.randomizeDelay(Settings.config.DelaySettings.Buff), 50));
					used_buffs++;
				}
			}
			await Task.Delay(Math.Max(1800 - (Settings.config.DelaySettings.Buff - 70) * used_buffs, 50));
		}
		foreach (NostaleCharacterInfo item5 in buffers)
		{
			if (current_buffset == null || current_buffset.pet)
			{
				if (item5.config.petBuff)
				{
					Keys button2 = (Keys)Enum.Parse(typeof(Keys), "R", ignoreCase: true);
					Controller.buttonWithShift(item5.hwnd, button2);
					await Task.Delay(Utils.randomizeDelay(Settings.config.DelaySettings.Buff));
				}
				continue;
			}
			break;
		}
	}

	public static async Task Respawn(NostaleCharacterInfo character)
	{
		await Task.Delay(new Random().Next(21000, 26001));
		Controller.SingleButtonPress(character.hwnd, Keys.Return);
	}

	public static void ClearTrainerData()
	{
		isInOwnMiniland = false;
		pet_trainer_mobs_list.Clear();
		trained_pets_count = 0;
		trained_pets_count_change_date = null;
	}
}
