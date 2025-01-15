using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NosAssistant2.Configs;
using NosAssistant2.Dtos;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;
using NosAssistant2.GUIElements;

namespace NosAssistant2.Helpers;

public static class RaidManager
{
	public static int lastFinishedRaid = -1;

	public static int lastKilledBossId = -1;

	public static long lastKilledServerBossId = -1L;

	public static int currentRaid = -1;

	public static int raidsFinished = 0;

	public static List<TimeSpan> raidsDuration = new List<TimeSpan>();

	public static Stopwatch stopwatch = new Stopwatch();

	public static bool show_total = false;

	private static Color impostorColor = NAStyles.NotActiveCharColor;

	private static Color correctRaiderColor = NAStyles.ActiveCharColor;

	public static Color defaultColor = Color.FromArgb(20, 9, 67);

	public static int last_hit_character_id = -1;

	public static int last_pet_hit_server_id = -1;

	public static List<GameSummon> new_pet_summons = new List<GameSummon>();

	public static int current_boss_max_hp = -1;

	public static int current_boss_current_hp = -1;

	public static bool updating_ranks = false;

	private static DateTime boss_kill_date;

	public static bool IsInUltimateArmaBossroom = false;

	public static bool UltArmaBoxesSpawn = false;

	public static bool raidStarted { get; set; } = false;


	public static bool glacernonRaidStarted { get; set; } = false;


	public static bool firstRaid { get; set; } = true;


	public static bool bossDead { get; set; } = false;


	public static int mobsKilled { get; set; } = 0;


	public static List<RaidDamage> singleRaid { get; set; } = new List<RaidDamage>();


	public static List<RaidDamage> totalRaid { get; set; } = new List<RaidDamage>();


	public static List<RaidDamage> currentList { get; set; } = new List<RaidDamage>();


	public static int lastThresholdCount { get; set; } = 0;


	public static List<int> currentMobsThreshold { get; set; } = new List<int>();


	public static ConcurrentQueue<RaidDamage> rank_update_queue { get; set; } = new ConcurrentQueue<RaidDamage>();


	public static void AddDamage(int character_id, int mob_id, int damage, int type, int skill_id, bool pet)
	{
		if (!raidStarted && ICManager.ICStarted == 0)
		{
			return;
		}
		RaidDamage current_record_single = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (pet)
		{
			int pet_owner_id = GUI.entities.Find((GameEntity x) => x.server_id == character_id)?.pet_owner_id ?? (-1);
			if (pet_owner_id == -1)
			{
				return;
			}
			current_record_single = singleRaid.Find((RaidDamage x) => x.CharacterID == pet_owner_id);
			raidDamage = totalRaid.Find((RaidDamage x) => x.CharacterID == pet_owner_id);
		}
		if (current_record_single == null || raidDamage == null)
		{
			return;
		}
		GameMonster gameMonster = GUI.monsters.Find((GameMonster x) => x.server_id == mob_id);
		if (gameMonster == null)
		{
			return;
		}
		if (!pet && gameMonster.is_boss)
		{
			handlePlayerBossHit(current_record_single, raidDamage, type, damage, skill_id);
		}
		else if (!pet && !gameMonster.is_boss)
		{
			handlePlayerMobHit(current_record_single, raidDamage, type, damage, gameMonster);
		}
		else if (pet && gameMonster.is_boss)
		{
			handlePetBossHit(current_record_single, raidDamage, damage);
		}
		if (current_record_single.Hit > 0)
		{
			current_record_single.Average = (int)((double)current_record_single.Total / (double)current_record_single.Hit);
		}
		if (raidDamage.Hit > 0)
		{
			raidDamage.Average = (int)((double)raidDamage.Total / (double)raidDamage.Hit);
		}
		if (show_total)
		{
			RaidForm.updateCharacterInList(raidDamage);
		}
		else
		{
			RaidForm.updateCharacterInList(current_record_single);
		}
		if (ICManager.ICStarted != 0 && current_record_single.All >= ICManager.GetRequiredDMG(current_record_single.Lvl))
		{
			Color color = (GUI._nostaleCharacterInfoList.Exists((NostaleCharacterInfo x) => x.character_id == current_record_single.CharacterID) ? impostorColor : correctRaiderColor);
			RaidForm.PaintCounterRow(current_record_single, color);
		}
	}

	public static void AddSummon(int id, int mob_server_id)
	{
		if (!raidStarted)
		{
			return;
		}
		GameSummon gameSummon = new GameSummon(id, mob_server_id, 0);
		if (gameSummon == null)
		{
			return;
		}
		RaidDamage raidDamage = null;
		if (gameSummon.is_pet_summon)
		{
			new_pet_summons.Add(gameSummon);
			return;
		}
		gameSummon.owner_id = last_hit_character_id;
		singleRaid.Find((RaidDamage x) => x.CharacterID == last_hit_character_id)?.summonsList.Add(gameSummon);
	}

	public static void AddPetSummon(int pet_id)
	{
		if (new_pet_summons.Count == 0)
		{
			return;
		}
		int pet_owner_id = GUI.entities.Find((GameEntity x) => x.server_id == pet_id)?.pet_owner_id ?? (-1);
		if (pet_owner_id == -1)
		{
			return;
		}
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == pet_owner_id);
		if (raidDamage == null)
		{
			return;
		}
		foreach (GameSummon new_pet_summon in new_pet_summons)
		{
			new_pet_summon.owner_id = raidDamage.CharacterID;
			raidDamage.summonsList.Add(new_pet_summon);
		}
		new_pet_summons.Clear();
	}

	public static void handleSummon(int summon_server_id, int dmg, int dest_id)
	{
		if (!raidStarted)
		{
			return;
		}
		RaidDamage current_record_single = singleRaid.Find((RaidDamage x) => x.summonsList.Any((GameSummon y) => y.server_id == summon_server_id));
		if (current_record_single == null)
		{
			return;
		}
		RaidDamage raidDamage = totalRaid.Find((RaidDamage x) => x.CharacterID == current_record_single.CharacterID);
		if (raidDamage == null)
		{
			return;
		}
		GameSummon gameSummon = current_record_single.summonsList.Find((GameSummon x) => x.server_id == summon_server_id);
		if (gameSummon != null)
		{
			if (gameSummon.name == "Onyx")
			{
				current_record_single.OnyxDmg += dmg;
				raidDamage.OnyxDmg += dmg;
			}
			if (gameSummon.single_use)
			{
				current_record_single.summonsList.Remove(gameSummon);
			}
			if (current_record_single != null)
			{
				AddDamage(current_record_single.CharacterID, dest_id, dmg, -1, -2, pet: false);
			}
			else if (raidDamage != null)
			{
				AddDamage(raidDamage.CharacterID, dest_id, dmg, -1, -2, pet: false);
			}
		}
	}

	public static void AddBon(int character_id)
	{
		if (raidStarted)
		{
			RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
			RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
			if (raidDamage != null && raidDamage2 != null)
			{
				raidDamage.isHitBon = true;
				raidDamage2.isHitBon = true;
			}
		}
	}

	private static void handlePetBossHit(RaidDamage current_record_single, RaidDamage current_record_total, int damage)
	{
		current_record_single.Pets += damage;
		current_record_total.Pets += damage;
		current_record_single.Total += damage;
		current_record_total.Total += damage;
		current_record_single.All += damage;
		current_record_total.All += damage;
	}

	public static void handlePlayerMobHit(RaidDamage current_record_single, RaidDamage current_record_total, int type, int damage, GameMonster mob)
	{
		current_record_single.All += damage;
		current_record_total.All += damage;
		current_record_single.MobDmg += damage;
		current_record_total.MobDmg += damage;
		current_record_single.AllHits++;
		current_record_total.AllHits++;
		if (type == 1 || type == 4 || type == 7)
		{
			current_record_single.AllMiss++;
			current_record_total.AllMiss++;
		}
		if (mob.is_special && ((type != 2 && type != 5 && type != 8) || damage != 0))
		{
			current_record_single.TotalSpecial += damage;
			current_record_total.TotalSpecial += damage;
		}
	}

	public static void handlePlayerBossHit(RaidDamage current_record_single, RaidDamage current_record_total, int type, int damage, int skill_id)
	{
		switch (type)
		{
		case -1:
		case 0:
		case 3:
		case 5:
		case 6:
		case 9:
			current_record_single.Total += damage;
			current_record_total.Total += damage;
			current_record_single.All += damage;
			current_record_total.All += damage;
			if (type != -1)
			{
				current_record_single.AllHits++;
				current_record_total.AllHits++;
			}
			if (current_record_single.MaxHit < damage)
			{
				current_record_single.MaxHit = damage;
				current_record_single.MaxHitSkillId = skill_id;
				if (!show_total)
				{
					RaidForm.updateMaxHitIconInList(current_record_single);
				}
			}
			if (current_record_total.MaxHit < damage)
			{
				current_record_total.MaxHit = damage;
				current_record_total.MaxHitSkillId = skill_id;
				if (show_total)
				{
					RaidForm.updateMaxHitIconInList(current_record_total);
				}
			}
			if (type == 3 || type == 6 || type == 9)
			{
				current_record_single.Crit++;
				current_record_total.Crit++;
				if (current_record_single.isHitBon)
				{
					current_record_single.Boncrit++;
				}
				if (current_record_total.isHitBon)
				{
					current_record_total.Boncrit++;
				}
			}
			break;
		case 1:
		case 4:
		case 7:
			current_record_single.Miss++;
			current_record_total.Miss++;
			break;
		}
		if ((type == 2 || type == 5 || type == 8) && damage == 0)
		{
			current_record_single.Dbf++;
			current_record_total.Dbf++;
		}
		else
		{
			if (current_record_single.isHitBon)
			{
				current_record_single.Bon++;
			}
			if (current_record_total.isHitBon)
			{
				current_record_total.Bon++;
			}
			if (type != -1)
			{
				current_record_single.Hit++;
				current_record_total.Hit++;
			}
		}
		current_record_single.isHitBon = false;
		current_record_total.isHitBon = false;
	}

	public static void MobKilled(GameMonster mob)
	{
		GUI.monsters.Remove(mob);
		if (raidStarted && mob.id != GameSummon.OnyxID)
		{
			mobsKilled++;
			GUI.RaidModeForm?.UpdateMobKilled();
		}
	}

	public static async Task<int> GetBestCandidateForRaid()
	{
		if (GUI.Mapper == null)
		{
			return 0;
		}
		return await NAHttpClient.GetBestCandidate(new BestCandidateDto
		{
			server_id = NostaleServers.GetServerIdFromName(GUI.Mapper.server),
			server_boss_id = lastKilledServerBossId,
			boss_id = lastKilledBossId
		});
	}

	public static async void BossKilled(GameMonster boss)
	{
		if (!raidStarted)
		{
			return;
		}
		bossDead = true;
		lastKilledBossId = boss.id;
		lastKilledServerBossId = boss.server_id;
		lastFinishedRaid = RaidID.GetRaidID(boss.id);
		if (ICManager.ICStarted != 0)
		{
			return;
		}
		stopwatch.Stop();
		boss_kill_date = DateTime.UtcNow;
		raidsFinished++;
		GUI.RaidModeForm?.setRaidsFinishedCount(raidsFinished);
		TimeSpan elapsed = stopwatch.Elapsed;
		raidsDuration.Add(elapsed);
		GUI.RaidModeForm?.setCurrentTime(elapsed);
		GUI.RaidModeForm?.setBestTime(raidsDuration.Min());
		GUI.RaidModeForm?.setAverageTime((raidsDuration.Count != 0) ? TimeSpan.FromTicks((long)raidsDuration.Average((TimeSpan d) => d.Ticks)) : TimeSpan.Zero);
		RaidForm.SortByTotal();
		if (GUI.Mapper != null)
		{
			if (glacernonRaidStarted)
			{
				await Task.Delay(1000);
				SaveRaid(save_locally: true);
				raidStarted = false;
				bossDead = false;
			}
			else
			{
				SaveRaid();
			}
		}
	}

	public static void SaveRaid(bool save_locally = false)
	{
		if (raidsFinished == 0 || GUI.Main == null || GUI.Mapper == null)
		{
			return;
		}
		List<RaidDamage> list = new List<RaidDamage>();
		list.AddRange(singleRaid.OrderByDescending((RaidDamage x) => x.Total));
		List<RaidPlayer> list2 = new List<RaidPlayer>();
		int num = 1;
		foreach (RaidDamage item in list)
		{
			if (item.SPId == 43)
			{
				item.SPId = 42;
			}
			if (item.SPId == 30)
			{
				item.SPId = 29;
			}
			list2.Add(new RaidPlayer
			{
				lp = num++,
				character_id = item.CharacterID,
				nickname = item.Nickname,
				family = item.Family,
				lvl = item.Lvl,
				clvl = item.CLvl,
				damage = item.Total,
				damage_miniboss = item.TotalSpecial,
				damage_onyx = item.OnyxDmg,
				max_hit = item.MaxHit,
				max_hit_skill_id = item.MaxHitSkillId,
				sp_id = item.SPId,
				class_id = item.ClassID,
				sex = item.Sex,
				pets = item.Pets,
				gold = item.Gold,
				average = item.Average,
				hit = item.Hit,
				miss = item.Miss,
				crit = item.Crit,
				bon = item.Bon,
				boncrit = item.Boncrit,
				debuffs = item.Dbf,
				dead = item.Dead,
				player_hits = item.pHits,
				player_kills = item.pKills,
				all_damage = item.All,
				mob_damage = item.MobDmg,
				all_hits = item.AllHits,
				all_miss = item.AllMiss
			});
		}
		RaidData raidData = new RaidData
		{
			version = GUI.version,
			server_id = NostaleServers.GetServerIdFromName(GUI.Mapper.server),
			channel_id = GUI.Mapper.channel.GetValueOrDefault(),
			sent_by_character_id = GUI.Mapper.character_id,
			sent_by_character_nickname = GUI.Mapper.nickname,
			server_boss_id = lastKilledServerBossId,
			boss_id = lastKilledBossId,
			finished_in = raidsDuration.Last().TotalSeconds,
			players = list2,
			boss_killed_at = boss_kill_date
		};
		RabbitEventHandler.SendRaidDataEvent(raidData);
		if (save_locally)
		{
			string bossName = GameBoss.getBossName(lastKilledBossId);
			if (!Directory.Exists("raid_history"))
			{
				Directory.CreateDirectory("raid_history");
			}
			if (!Directory.Exists("raid_history/" + bossName))
			{
				Directory.CreateDirectory("raid_history/" + bossName);
			}
			File.WriteAllText($"raid_history/{bossName}/{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json", JsonConvert.SerializeObject(raidData, Formatting.Indented));
		}
	}

	public static void AddDeath(int character_id)
	{
		if (!raidStarted)
		{
			return;
		}
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null && raidDamage2 != null)
		{
			raidDamage.Dead++;
			raidDamage2.Dead++;
			if (show_total)
			{
				RaidForm.updateCharacterInList(raidDamage2);
			}
			else
			{
				RaidForm.updateCharacterInList(raidDamage);
			}
		}
	}

	public static void AddGold(int character_id, int goldAmount)
	{
		if (!raidStarted)
		{
			return;
		}
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null && raidDamage2 != null)
		{
			raidDamage.Gold += goldAmount;
			raidDamage2.Gold += goldAmount;
			if (show_total)
			{
				RaidForm.updateCharacterInList(raidDamage2);
			}
			else
			{
				RaidForm.updateCharacterInList(raidDamage);
			}
		}
	}

	public static void UpdateFam(int character_id, string family_name)
	{
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null)
		{
			raidDamage.Family = family_name;
		}
		if (raidDamage2 != null)
		{
			raidDamage2.Family = family_name;
		}
		if (show_total && raidDamage2 != null)
		{
			RaidForm.updateCharacterInList(raidDamage2);
		}
		else if (raidDamage != null)
		{
			RaidForm.updateCharacterInList(raidDamage);
		}
	}

	public static void UpdateFairy(int character_id, int fairyID)
	{
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null)
		{
			raidDamage.Booster = GameFairy.IsFairyBoosted(fairyID);
		}
		if (raidDamage2 != null)
		{
			raidDamage2.Booster = GameFairy.IsFairyBoosted(fairyID);
		}
		if (show_total && raidDamage2 != null)
		{
			RaidForm.updateCharacterInList(raidDamage2);
		}
		else if (raidDamage != null)
		{
			RaidForm.updateCharacterInList(raidDamage);
		}
	}

	public static void UpdateTarot(int character_id, int tarot_id)
	{
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null)
		{
			raidDamage.Tarot = tarot_id;
		}
		if (raidDamage2 != null)
		{
			raidDamage2.Tarot = tarot_id;
		}
		if (raidDamage != null)
		{
			raidDamage.lastTarotUpdate = DateTime.UtcNow;
		}
		if (raidDamage2 != null)
		{
			raidDamage2.lastTarotUpdate = DateTime.UtcNow;
		}
		if (show_total && raidDamage2 != null)
		{
			RaidForm.updateTarotInList(raidDamage2);
		}
		else if (raidDamage != null)
		{
			RaidForm.updateTarotInList(raidDamage);
		}
	}

	public static void ValidateTarots()
	{
		int num = 10;
		List<RaidDamage> list = singleRaid;
		List<RaidDamage> list2 = new List<RaidDamage>();
		CollectionsMarshal.SetCount(list2, list.Count);
		Span<RaidDamage> span = CollectionsMarshal.AsSpan(list2);
		int num2 = 0;
		Span<RaidDamage> span2 = CollectionsMarshal.AsSpan(list);
		span2.CopyTo(span.Slice(num2, span2.Length));
		num2 += span2.Length;
		foreach (RaidDamage item in list2)
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime? lastTarotUpdate = item.lastTarotUpdate;
			TimeSpan? timeSpan = utcNow - lastTarotUpdate;
			if (timeSpan.HasValue && timeSpan.Value.TotalSeconds > (double)num)
			{
				UpdateTarot(item.CharacterID, 0);
			}
		}
	}

	public static void UpdateSP(int character_id, int spID)
	{
		if (spID > 54)
		{
			return;
		}
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null && raidDamage.SPId != spID && raidDamage2 != null && raidDamage2.SPId != spID)
		{
			raidDamage.SPId = spID;
			raidDamage2.SPId = spID;
			rank_update_queue.Enqueue(raidDamage);
			if (show_total)
			{
				RaidForm.updateCharacterInList(raidDamage2);
			}
			else
			{
				RaidForm.updateCharacterInList(raidDamage);
			}
		}
	}

	public static void AddCharacterToGlacernonRaidOrIC(GamePlayer player)
	{
		if (!singleRaid.Any((RaidDamage x) => x.CharacterID == player.character_id))
		{
			RaidDamage raidDamage = new RaidDamage
			{
				Nickname = player.nickname,
				Lvl = player.lvl,
				CLvl = player.clvl,
				CharacterID = player.character_id,
				SPId = player.spID,
				ClassID = player.classID,
				Sex = player.sex,
				Family = player.family,
				icon = player.icon
			};
			singleRaid.Add(raidDamage);
			totalRaid = singleRaid;
			RaidForm.addCharacterToList(raidDamage);
		}
	}

	public static void UpdateCharactersInRaidList(NosPacket packet)
	{
		if (glacernonRaidStarted || ICManager.ICStarted != 0)
		{
			return;
		}
		List<string> list = packet.packet_splitted.Skip((packet.packet_type == "rdlst") ? 4 : 5).ToList();
		currentList.Clear();
		foreach (string item in list)
		{
			string[] array = item.Split(".");
			List<string> list2 = new List<string>();
			CollectionsMarshal.SetCount(list2, array.Length);
			Span<string> span = CollectionsMarshal.AsSpan(list2);
			int num = 0;
			Span<string> span2 = new Span<string>(array);
			span2.CopyTo(span.Slice(num, span2.Length));
			num += span2.Length;
			List<string> list3 = list2;
			if (list3.Count < 2)
			{
				continue;
			}
			int num2 = Convert.ToInt32(list3.ElementAt(1));
			if (num2 == -1)
			{
				num2 = 0;
			}
			if (list3.Count < 7)
			{
				continue;
			}
			int ID = Convert.ToInt32(list3.ElementAt(6));
			RaidDamage raidDamage = totalRaid.Find((RaidDamage x) => x.CharacterID == ID);
			RaidDamage raidDamage2;
			if (raidDamage != null)
			{
				raidDamage.SPId = num2;
				raidDamage2 = new RaidDamage(raidDamage);
				raidDamage2.Clear();
				raidDamage2.UpdateIcon();
				rank_update_queue.Enqueue(raidDamage2);
				currentList.Add(raidDamage2);
				if (firstRaid && !singleRaid.Any((RaidDamage x) => x.CharacterID == ID))
				{
					singleRaid.Add(raidDamage2);
					RaidForm.addCharacterToList(raidDamage2);
				}
				else if (singleRaid.Find((RaidDamage x) => x.CharacterID == ID) != null)
				{
					RaidDamage raidDamage3 = singleRaid.Find((RaidDamage x) => x.CharacterID == ID);
					if (raidDamage3 != null && raidDamage3.SPId != num2)
					{
						raidDamage3.SPId = num2;
						raidDamage3.UpdateIcon();
					}
				}
			}
			else
			{
				raidDamage2 = createNewRaider(item);
				rank_update_queue.Enqueue(raidDamage2);
				totalRaid.Add(raidDamage2);
				currentList.Add(new RaidDamage(raidDamage2));
				if (firstRaid)
				{
					singleRaid.Add(new RaidDamage(raidDamage2));
					RaidForm.addCharacterToList(raidDamage2);
				}
			}
			detectImpostor(raidDamage2);
		}
		if (firstRaid)
		{
			foreach (RaidDamage item2 in singleRaid.Where((RaidDamage x) => !currentList.Any((RaidDamage y) => y.CharacterID == x.CharacterID)).ToList())
			{
				singleRaid.Remove(item2);
				RaidForm.removeCharacterFromList(item2);
			}
			{
				foreach (RaidDamage item3 in totalRaid.Where((RaidDamage x) => !currentList.Any((RaidDamage y) => y.CharacterID == x.CharacterID)).ToList())
				{
					totalRaid.Remove(item3);
				}
				return;
			}
		}
		foreach (RaidDamage item4 in singleRaid.Where((RaidDamage x) => !currentList.Any((RaidDamage y) => y.CharacterID == x.CharacterID)).ToList())
		{
			if (!item4.WasInRaid)
			{
				singleRaid.Remove(item4);
				RaidForm.removeCharacterFromList(item4);
			}
			else
			{
				RaidForm.PaintCounterRow(item4, defaultColor);
				updateRaiderStatusColor(item4, defaultColor);
			}
		}
		foreach (RaidDamage item5 in totalRaid.Where((RaidDamage x) => !currentList.Any((RaidDamage y) => y.CharacterID == x.CharacterID)).ToList())
		{
			if (item5.raidsCount == 0)
			{
				totalRaid.Remove(item5);
			}
		}
	}

	public static void UpdateCharactersInfo(NosPacket packet)
	{
		if (glacernonRaidStarted || ICManager.ICStarted != 0)
		{
			return;
		}
		foreach (string item in packet.packet_splitted.Skip((packet.packet_type == "rdlst") ? 4 : 5).ToList())
		{
			string[] array = item.Split(".");
			List<string> list = new List<string>();
			CollectionsMarshal.SetCount(list, array.Length);
			Span<string> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			Span<string> span2 = new Span<string>(array);
			span2.CopyTo(span.Slice(num, span2.Length));
			num += span2.Length;
			List<string> source = list;
			int num2 = Convert.ToInt32(source.ElementAt(1));
			if (num2 == -1)
			{
				num2 = 0;
			}
			int ID = Convert.ToInt32(source.ElementAt(6));
			RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == ID);
			RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == ID);
			if (raidDamage != null)
			{
				raidDamage.SPId = num2;
				raidDamage.UpdateIcon();
			}
			if (raidDamage2 != null)
			{
				raidDamage2.UpdateIcon();
				raidDamage2.SPId = num2;
			}
		}
	}

	private static void detectImpostor(RaidDamage newRaider)
	{
		if (firstRaid)
		{
			return;
		}
		RaidDamage raider = totalRaid.Find((RaidDamage x) => x.CharacterID == newRaider.CharacterID);
		if (raider == null)
		{
			return;
		}
		if (!raider.WasInRaid)
		{
			if (singleRaid.Find((RaidDamage x) => x.CharacterID == raider.CharacterID) == null)
			{
				singleRaid.Add(raider);
				RaidForm.addCharacterToList(raider);
				if (currentRaid == lastFinishedRaid)
				{
					RaidForm.PaintCounterRow(raider, impostorColor);
					updateRaiderStatusColor(raider, impostorColor);
					Controller.PlaySound("Impostor");
				}
			}
		}
		else
		{
			RaidForm.PaintCounterRow(raider, correctRaiderColor);
			updateRaiderStatusColor(raider, correctRaiderColor);
		}
	}

	private static void updateRaiderStatusColor(RaidDamage character, Color color)
	{
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character.CharacterID);
		if (raidDamage != null)
		{
			raidDamage.statusColor = color;
		}
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character.CharacterID);
		if (raidDamage2 != null)
		{
			raidDamage2.statusColor = color;
		}
	}

	private static RaidDamage createNewRaider(string raidMemberString)
	{
		List<string> source = raidMemberString.Split(".").ToList();
		int lvl = Convert.ToInt32(source.ElementAt(0));
		int num = Convert.ToInt32(source.ElementAt(1));
		int classID = Convert.ToInt32(source.ElementAt(2));
		string nickname = source.ElementAt(4);
		int sex = Convert.ToInt32(source.ElementAt(5));
		int ID = Convert.ToInt32(source.ElementAt(6));
		int cLvl = Convert.ToInt32(source.ElementAt(7));
		if (num == -1)
		{
			num = 0;
		}
		RaidDamage raidDamage = new RaidDamage
		{
			Nickname = nickname,
			Lvl = lvl,
			CLvl = cLvl,
			CharacterID = ID,
			SPId = num,
			ClassID = classID,
			Sex = sex
		};
		raidDamage.UpdateIcon();
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.character_id == ID);
		if (nostaleCharacterInfo != null && nostaleCharacterInfo.family_name != "")
		{
			raidDamage.Family = nostaleCharacterInfo.family_name;
		}
		else
		{
			NostaleCharacterInfo nostaleCharacterInfo2 = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.character_id == ID);
			if (nostaleCharacterInfo2 != null && nostaleCharacterInfo2.family_name != "-")
			{
				raidDamage.Family = nostaleCharacterInfo2.family_name;
			}
		}
		return raidDamage;
	}

	public static void ResetCounter()
	{
		ClearAll();
		firstRaid = true;
		bossDead = false;
		raidStarted = false;
		totalRaid.Clear();
		singleRaid.Clear();
		raidsFinished = 0;
		mobsKilled = 0;
		lastFinishedRaid = -1;
		raidsDuration.Clear();
		GUI.RaidModeForm?.setCurrentTime(TimeSpan.Zero);
		GUI.RaidModeForm?.setBestTime(TimeSpan.Zero);
		GUI.RaidModeForm?.setAverageTime(TimeSpan.Zero);
		GUI.RaidModeForm?.setRaidsFinishedCount(0);
		GUI.RaidModeForm?.UpdateMobKilled();
		Utils.InvokeIfRequired(RaidForm._statisticsPanel, delegate
		{
			foreach (Control control in RaidForm._statisticsPanel.Controls)
			{
				control.Visible = false;
			}
		});
		RaidForm.hideRaidSpecificColumns();
	}

	public static void ClearAll()
	{
		bossDead = false;
		singleRaid.Clear();
		RaidForm.ClearTable();
	}

	public static void RestoreCounter(bool paint)
	{
		RaidForm.ClearTable();
		if (!paint)
		{
			ClearTotal();
		}
		foreach (RaidDamage character in (!show_total || firstRaid) ? singleRaid : totalRaid)
		{
			RaidDamage raidDamage = totalRaid.Find((RaidDamage x) => x.CharacterID == character.CharacterID);
			if (raidDamage == null)
			{
				continue;
			}
			int raidsCount = raidDamage.raidsCount;
			bool flag = singleRaid.Find((RaidDamage x) => x.CharacterID == character.CharacterID) != null;
			if (raidsCount != 0 || flag || show_total || firstRaid)
			{
				RaidForm.addCharacterToList(character);
				if (paint)
				{
					RaidForm.PaintCounterRow(character, character.statusColor);
				}
			}
		}
		if (paint)
		{
			return;
		}
		List<RaidDamage> list = new List<RaidDamage>();
		foreach (RaidDamage character in totalRaid)
		{
			character.statusColor = defaultColor;
			RaidDamage raidDamage2 = singleRaid.Find((RaidDamage x) => x.CharacterID == character.CharacterID);
			if (raidDamage2 != null)
			{
				raidDamage2.statusColor = defaultColor;
			}
			if (raidDamage2 != null && !raidDamage2.WasInRaid)
			{
				singleRaid.Remove(raidDamage2);
			}
			if (raidDamage2 == null && character.raidsCount == 0)
			{
				list.Add(character);
			}
		}
		foreach (RaidDamage item in list)
		{
			totalRaid.Remove(item);
			if (show_total)
			{
				RaidForm.removeCharacterFromList(item);
			}
		}
	}

	public static void ClearTotal()
	{
		totalRaid = totalRaid.Where((RaidDamage x) => x.raidsCount != 0).ToList();
	}

	public static void GlacernonRaidStarted()
	{
		ResetCounter();
		lastFinishedRaid = -2;
		raidStarted = true;
		bossDead = false;
		stopwatch = Stopwatch.StartNew();
	}

	public static void savePayloadsData()
	{
		if (!Directory.Exists("test"))
		{
			Directory.CreateDirectory("test");
		}
		File.WriteAllText("test/payloads_" + DateTime.Now.ToString("HH_mm_ss") + ".json", JsonConvert.SerializeObject(PacketsManager.payloadsDict));
		foreach (nint key in PacketsManager.payloadsDict.Keys)
		{
			List<string> list = new List<string>();
			List<byte[]> list2 = PacketsManager.SplitPackets(PacketsManager.payloadsDict[key]);
			List<string> list3 = new List<string>();
			foreach (byte[] item in list2)
			{
				list.Add(Crypto.NormalizeRecvPacket(Crypto.DecryptRecvPacket(item)));
				list3.Add(BitConverter.ToString(item).Replace("-", " "));
			}
			File.WriteAllText($"test/payloads_decrypted_{key}_{DateTime.Now.ToString("HH_mm_ss")}.json", JsonConvert.SerializeObject(list, Formatting.Indented));
			File.WriteAllText($"test/splitted_packets_{key}_{DateTime.Now.ToString("HH_mm_ss")}.json", JsonConvert.SerializeObject(list3, Formatting.Indented));
		}
		File.WriteAllText("test/processed_packets_" + DateTime.Now.ToString("HH_mm_ss") + ".json", JsonConvert.SerializeObject(PacketsManager.processed_packets, Formatting.Indented));
		PacketsManager.processed_packets.Clear();
		PacketsManager.payloadsDict = new Dictionary<nint, byte[]>();
		PacketsManager.start_collecting = false;
	}

	public static void RaidFinished()
	{
		if (bossDead && ICManager.ICStarted == 0)
		{
			SaveRaid(save_locally: true);
		}
		raidStarted = false;
		bossDead = false;
	}

	public static void RaidStarted()
	{
		if (GUI.Mapper == null)
		{
			return;
		}
		if (Settings.config.RaidModeSettings.AutoOpenCounter)
		{
			Utils.InvokeIfRequired(GUI.RaidModeForm, delegate
			{
				RaidForm? raidModeForm = GUI.RaidModeForm;
				if (raidModeForm != null && !raidModeForm.Visible)
				{
					GUI.RaidModeForm?.Show();
				}
			});
		}
		if (lastFinishedRaid != currentRaid && !firstRaid)
		{
			ResetCounter();
		}
		PacketsManager.packets_corrupted_count = 0;
		raidStarted = true;
		firstRaid = false;
		bossDead = false;
		stopwatch = Stopwatch.StartNew();
		ClearAll();
		Dictionary<int, int> indexDict = singleRaid.Select((RaidDamage item, int index) => new
		{
			CharacterID = item.CharacterID,
			Index = index
		}).ToDictionary(x => x.CharacterID, x => x.Index);
		currentList.Sort(delegate(RaidDamage x, RaidDamage y)
		{
			if (!indexDict.TryGetValue(x.CharacterID, out var value))
			{
				value = int.MaxValue;
			}
			if (!indexDict.TryGetValue(y.CharacterID, out var value2))
			{
				value2 = int.MaxValue;
			}
			return value.CompareTo(value2);
		});
		List<RaidDamage> list = currentList;
		List<RaidDamage> list2 = new List<RaidDamage>();
		CollectionsMarshal.SetCount(list2, list.Count);
		Span<RaidDamage> span = CollectionsMarshal.AsSpan(list2);
		int num = 0;
		Span<RaidDamage> span2 = CollectionsMarshal.AsSpan(list);
		span2.CopyTo(span.Slice(num, span2.Length));
		num += span2.Length;
		singleRaid = list2;
		if (totalRaid.Count == 0)
		{
			totalRaid = RaidDamage.copyList(singleRaid);
		}
		foreach (RaidDamage item in show_total ? totalRaid : singleRaid)
		{
			if (GUI.Mapper != null && item.CharacterID == GUI.Mapper.character_id && SPID.IsCombatSP(GUI.Mapper.SPCard.ID))
			{
				item.SPId = GUI.Mapper.SPCard.ID;
			}
			RaidForm.addCharacterToList(item);
		}
		foreach (RaidDamage raider_total in totalRaid)
		{
			RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == raider_total.CharacterID);
			if (raidDamage != null)
			{
				raider_total.WasInRaid = true;
				raidDamage.WasInRaid = true;
				raider_total.raidsCount++;
				raidDamage.raidsCount++;
				raidDamage.summonsList.Clear();
				updateRaiderStatusColor(raider_total, defaultColor);
				updateRaiderStatusColor(raidDamage, defaultColor);
			}
			else
			{
				raider_total.WasInRaid = false;
				updateRaiderStatusColor(raider_total, defaultColor);
			}
		}
		if ((currentRaid != 23 || !Settings.config.WaypointsConfig.ZenasStart) && (currentRaid != 24 || !Settings.config.WaypointsConfig.EreniaStart))
		{
			return;
		}
		Task.Run(async delegate
		{
			int num2 = new Random().Next(7500, 10001);
			if (currentRaid == 24)
			{
				num2 += 3000;
			}
			await Task.Delay(num2);
			if (currentRaid == 24)
			{
				Raids.random_point = new Point(Utils.randomizeCoord(107, 1), Utils.randomizeCoord(103, 1));
			}
			else if (currentRaid == 23)
			{
				if (!Settings.config.WaypointsConfig.SpreadCharacters)
				{
					Raids.random_point = new Point(Utils.randomizeCoord(108, 2), Utils.randomizeCoord(101, 2));
					Raids.random_point2 = Raids.PickRandomZenasRightPoint();
				}
				else
				{
					Raids.random_point = new Point(Utils.randomizeCoord(95, 1), Utils.randomizeCoord(61, 1));
				}
			}
			foreach (NostaleCharacterInfo item2 in GUI._nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isMover && x.inRaid && !x.config.isDisabled))
			{
				if (currentRaid == 23)
				{
					Raids.performActionQueue.Enqueue((item2, "MoveZenasStart"));
				}
				else if (currentRaid == 24)
				{
					Raids.performActionQueue.Enqueue((item2, "MoveEreniaStart"));
				}
				Raids.performAction();
			}
		});
	}

	public static void GraphsStart()
	{
		for (int i = 0; i < 15; i++)
		{
			Utils.InvokeIfRequired(RaidForm._statisticsPanel, delegate
			{
				GraphsItem graphsItem = new GraphsItem();
				GUI.ScaleControl(graphsItem);
				graphsItem.setNickname("");
				graphsItem.setPortait(null);
				graphsItem.setValue(0.0);
				graphsItem.Visible = false;
				RaidForm._statisticsPanel.Controls.Add(graphsItem);
			});
		}
	}

	public static void GraphsUpdate()
	{
		if (GUI.Mapper == null || (!GUI.Mapper.inRaid && ICManager.ICStarted == 0) || (!MapID.isRaidMap(GUI.Mapper.map_id) && !firstRaid && GUI.Mapper.map_id != 2717 && GUI.Mapper.map_id != 2004))
		{
			return;
		}
		bool graphs_special_mode = !MapID.isBossRoom(GUI.Mapper?.map_id ?? 0);
		List<RaidDamage> list;
		if (ICManager.ICStarted == 0)
		{
			if (!graphs_special_mode)
			{
				list = new List<RaidDamage>();
				list.AddRange(singleRaid.OrderByDescending((RaidDamage item) => item.Total));
			}
			else
			{
				list = new List<RaidDamage>();
				list.AddRange(singleRaid.OrderByDescending((RaidDamage item) => item.TotalSpecial));
			}
		}
		else
		{
			list = new List<RaidDamage>();
			list.AddRange(singleRaid.OrderByDescending((RaidDamage item) => item.All));
		}
		List<RaidDamage> sortedList = list;
		long totalDMG = sortedList.Take(2).Sum((RaidDamage item) => (ICManager.ICStarted == 0) ? ((!graphs_special_mode) ? item.Total : item.TotalSpecial) : item.All);
		int i = 0;
		List<GraphsItem> list2 = RaidForm._statisticsPanel.Controls.OfType<GraphsItem>().ToList();
		int num = Math.Min(list2.Count, sortedList.Count);
		foreach (GraphsItem item in list2)
		{
			bool isVisible = i < num;
			Utils.InvokeIfRequired(item, delegate
			{
				item.Visible = isVisible;
				if (isVisible)
				{
					RaidDamage raidDamage = sortedList[i];
					item.setNickname(raidDamage.Nickname);
					item.setPortait(raidDamage.icon);
					item.setValue((totalDMG != 0L) ? Math.Max((double)((ICManager.ICStarted != 0) ? raidDamage.All : (graphs_special_mode ? raidDamage.TotalSpecial : raidDamage.Total)) / (double)totalDMG, 0.01) : 0.01);
				}
			});
			int num2 = i;
			i = num2 + 1;
		}
	}

	public static void UpdateEffects(int character_id, bool atk_pot, bool valehirDebuff, bool bellialDebuff)
	{
		RaidDamage raidDamage = (show_total ? totalRaid.Find((RaidDamage x) => x.CharacterID == character_id) : singleRaid.Find((RaidDamage x) => x.CharacterID == character_id));
		if (raidDamage == null)
		{
			return;
		}
		raidDamage.Att = atk_pot;
		if (raidDamage.valehirDebuff != valehirDebuff)
		{
			raidDamage.valehirDebuff = valehirDebuff;
			Color color = (valehirDebuff ? impostorColor : defaultColor);
			RaidForm.PaintCounterRow(raidDamage, color);
		}
		if (raidDamage.bellialDebuff != bellialDebuff)
		{
			raidDamage.bellialDebuff = bellialDebuff;
			if (!bellialDebuff)
			{
				RaidForm.PaintCounterRow(raidDamage, defaultColor);
			}
		}
		RaidForm.updateCharacterInList(raidDamage);
	}

	public static void MarkPlayerHittingPlayers(int character_id, int is_alive)
	{
		if (!RaidForm.isInBellial)
		{
			return;
		}
		RaidDamage raidDamage = singleRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		RaidDamage raidDamage2 = totalRaid.Find((RaidDamage x) => x.CharacterID == character_id);
		if (raidDamage != null)
		{
			raidDamage.pHits++;
			if (is_alive == 0)
			{
				raidDamage.pKills++;
			}
			RaidForm.updateCharacterInList(raidDamage);
		}
		if (raidDamage2 != null)
		{
			raidDamage2.pHits++;
			if (is_alive == 0)
			{
				raidDamage2.pKills++;
			}
			RaidForm.updateCharacterInList(raidDamage2);
		}
		RaidDamage raidDamage3 = (show_total ? raidDamage2 : raidDamage);
		if (raidDamage3 != null)
		{
			RaidForm.PaintCounterRow(raidDamage3, impostorColor);
		}
	}

	public static void handleArmaButtons(GameEntity entity)
	{
		NostaleCharacterInfo? mapper = GUI.Mapper;
		if (mapper != null && mapper.real_map_id == 2754)
		{
			if (entity.id == 1135)
			{
				List<GameEntity> list = GUI.entities.Where((GameEntity x) => x.type_name == "Lever").ToList();
				if (list.Count == 0 || list.Count == 3 || list.Count == 4 || list.Count == 6)
				{
					entity.is_required_lever = true;
				}
				else
				{
					entity.is_required_lever = false;
				}
			}
			else if (entity.id == 1136)
			{
				List<GameEntity> list2 = GUI.entities.Where((GameEntity x) => x.type_name == "Lever" && x.is_required_lever).ToList();
				entity.is_required_lever = list2.Count != 4;
			}
		}
		NostaleCharacterInfo? mapper2 = GUI.Mapper;
		if (mapper2 == null || mapper2.real_map_id != 2755)
		{
			return;
		}
		if (entity.id == 1135)
		{
			List<GameEntity> list3 = GUI.entities.Where((GameEntity x) => x.type_name == "Lever").ToList();
			if (list3.Count == 1 || list3.Count == 4 || list3.Count == 7)
			{
				entity.is_required_lever = true;
			}
			else
			{
				entity.is_required_lever = false;
			}
		}
		else if (entity.id == 1136)
		{
			List<GameEntity> list4 = GUI.entities.Where((GameEntity x) => x.type_name == "Lever" && x.is_required_lever).ToList();
			entity.is_required_lever = list4.Count != 3;
		}
	}

	public static async void UpdateRanks()
	{
		_ = 1;
		try
		{
			if (updating_ranks || GUI.Mapper == null || rank_update_queue.IsEmpty)
			{
				return;
			}
			updating_ranks = true;
			int serverIdFromName = NostaleServers.GetServerIdFromName(GUI.Mapper?.server ?? "");
			if (serverIdFromName == 0 || currentRaid == -1)
			{
				updating_ranks = false;
				return;
			}
			int bossID = RaidID.GetBossID(currentRaid);
			if (bossID == -1)
			{
				return;
			}
			List<RaidDamage> raiders = new List<RaidDamage>();
			List<GetPlayerRaidRankingInfoDto> list = new List<GetPlayerRaidRankingInfoDto>();
			List<GetRaidRankingInfoDto> unique_sps_data = new List<GetRaidRankingInfoDto>();
			while (true)
			{
				if (!rank_update_queue.TryDequeue(out RaidDamage record) || record == null)
				{
					break;
				}
				if (record.SPId <= 54 && record.SPId != 43 && record.SPId != 30)
				{
					raiders.Add(record);
					list.Add(new GetPlayerRaidRankingInfoDto
					{
						server_id = NostaleServers.GetServerIdFromName(GUI.Mapper.server),
						boss_id = bossID,
						character_id = record.CharacterID,
						sp_id = record.SPId
					});
					if (unique_sps_data.Count == 0 || !unique_sps_data.Any((GetRaidRankingInfoDto x) => x.sp_id == record.SPId))
					{
						unique_sps_data.Add(new GetRaidRankingInfoDto
						{
							server_id = serverIdFromName,
							boss_id = bossID,
							sp_id = record.SPId
						});
					}
				}
			}
			if (raiders.Count == 0)
			{
				updating_ranks = false;
				return;
			}
			List<PlayerRaidRankingInfo> avg_dmgs = await NAHttpClient.GetPlayersRaidRankingInfo(list);
			if (avg_dmgs == null)
			{
				return;
			}
			List<RaidRankingInfo> list2 = await NAHttpClient.GetRaidRankingsInfo(unique_sps_data);
			if (list2 == null)
			{
				return;
			}
			if (avg_dmgs.Count == raiders.Count)
			{
				int i;
				for (i = 0; i < raiders.Count; i++)
				{
					RaidRankingInfo raidRankingInfo = list2.Find((RaidRankingInfo x) => x.sp_id == raiders[i].SPId);
					if (raidRankingInfo != null)
					{
						int new_rank = ((avg_dmgs[i].player_avg_damage != 0) ? Analytics.AssignPlayersRank(avg_dmgs[i].player_avg_damage, raidRankingInfo.mean_damage, raidRankingInfo.stddev_damage) : 0);
						raiders[i].UpdateRank(new_rank);
					}
					else
					{
						raiders[i].UpdateRank(0);
					}
				}
			}
			updating_ranks = false;
		}
		catch (Exception ex)
		{
			updating_ranks = false;
			string errorlog = $"Error in UpdateRanks. Error message: {ex}";
			if (GUI.Mapper == null)
			{
				return;
			}
			NALogger.LogExceptionToFile(ex);
			try
			{
				using HttpClient client = new HttpClient();
				client.PostAsJsonAsync("https://nosassistant.pl/api/errorlog", new ErrorLog
				{
					errorlog = errorlog,
					mapper = GUI.Mapper?.nickname,
					map_id = GUI.Mapper.real_map_id.ToString()
				}).Wait();
			}
			catch (Exception exception)
			{
				NALogger.LogExceptionToFile(exception);
			}
		}
	}

	public static void MarkUltArmaNotInFieldRaiders()
	{
		if (!UltArmaBoxesSpawn)
		{
			return;
		}
		List<GameMonster> source = GUI.monsters.Where((GameMonster x) => x.id == 1563 || x.id == 1564).ToList();
		foreach (RaidDamage raider in singleRaid)
		{
			int raider_x = -1;
			int raider_y = -1;
			GamePlayer gamePlayer = GUI.players.Find((GamePlayer x) => x.character_id == raider.CharacterID);
			if (gamePlayer == null)
			{
				NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => x.character_id == raider.CharacterID);
				if (nostaleCharacterInfo != null)
				{
					raider_x = nostaleCharacterInfo.x_pos;
					raider_y = nostaleCharacterInfo.y_pos;
				}
			}
			else
			{
				raider_x = gamePlayer.x;
				raider_y = gamePlayer.y;
			}
			if (raider_x != -1 && raider_y != -1)
			{
				float tolerance_distance = 3f;
				raider.statusColor = (source.Any((GameMonster x) => Utils.CalculateDistance(new Point(x.x, x.y), new Point(raider_x, raider_y)) <= (double)tolerance_distance) ? defaultColor : impostorColor);
				if (!show_total)
				{
					RaidForm.PaintCounterRow(raider, raider.statusColor);
				}
			}
		}
	}
}
