using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NosAssistant2.GUIElements;
using NosAssistant2.Helpers;

namespace NosAssistant2.GameObjects;

public class RaidDamage
{
	public int ID { get; set; } = -1;


	public int Lp { get; set; } = 1;


	public int Lvl { get; set; }

	public int CLvl { get; set; }

	public int CharacterID { get; set; }

	public string Nickname { get; set; } = "";


	public string Family { get; set; } = "-";


	public long Total { get; set; }

	public long TotalSpecial { get; set; }

	public long OnyxDmg { get; set; }

	public int MaxHit { get; set; }

	public int MaxHitSkillId { get; set; }

	public int SPId { get; set; } = -1;


	public int ClassID { get; set; } = -1;


	public int Sex { get; set; } = -1;


	public int Pets { get; set; }

	public long Gold { get; set; }

	public long Average { get; set; }

	public int Hit { get; set; }

	public bool Att { get; set; }

	public bool Booster { get; set; }

	public int Tarot { get; set; }

	public int Miss { get; set; }

	public int Crit { get; set; }

	public int Bon { get; set; }

	public int Boncrit { get; set; }

	public int Dbf { get; set; }

	public int Dead { get; set; }

	public int pHits { get; set; }

	public int pKills { get; set; }

	public int MBHit { get; set; }

	public long All { get; set; }

	public long MobDmg { get; set; }

	public int AllHits { get; set; }

	public int AllMiss { get; set; }

	public int raidsCount { get; set; }

	public List<GameSummon> summonsList { get; set; } = new List<GameSummon>();


	public bool WasInRaid { get; set; }

	public Color statusColor { get; set; } = RaidManager.defaultColor;


	public DateTime? lastTarotUpdate { get; set; }

	public bool isHitBon { get; set; }

	public bool valehirDebuff { get; set; }

	public bool bellialDebuff { get; set; }

	public Image? icon { get; set; }

	public int rank { get; set; } = -1;


	public Image? rankIcon { get; set; }

	public RaidDamage()
	{
	}

	public RaidDamage(RaidDamage other)
	{
		ID = other.ID;
		Lp = other.Lp;
		Lvl = other.Lvl;
		CLvl = other.CLvl;
		CharacterID = other.CharacterID;
		Nickname = other.Nickname;
		Family = other.Family;
		Total = other.Total;
		TotalSpecial = other.TotalSpecial;
		OnyxDmg = other.OnyxDmg;
		MaxHit = other.MaxHit;
		MaxHitSkillId = other.MaxHitSkillId;
		SPId = other.SPId;
		ClassID = other.ClassID;
		Sex = other.Sex;
		Booster = other.Booster;
		Tarot = other.Tarot;
		Pets = other.Pets;
		Gold = other.Gold;
		Att = other.Att;
		Hit = other.Hit;
		Miss = other.Miss;
		Crit = other.Crit;
		Bon = other.Bon;
		Boncrit = other.Boncrit;
		Dbf = other.Dbf;
		Dead = other.Dead;
		pHits = other.pHits;
		pKills = other.pKills;
		MBHit = other.MBHit;
		All = other.All;
		MobDmg = other.MobDmg;
		AllHits = other.AllHits;
		AllMiss = other.AllMiss;
		raidsCount = other.raidsCount;
		WasInRaid = other.WasInRaid;
		icon = other.icon;
		rankIcon = other.rankIcon;
		rank = other.rank;
	}

	public void Clear()
	{
		Total = 0L;
		TotalSpecial = 0L;
		OnyxDmg = 0L;
		MaxHit = 0;
		MaxHitSkillId = 0;
		Pets = 0;
		Gold = 0L;
		Hit = 0;
		Miss = 0;
		Crit = 0;
		Bon = 0;
		Boncrit = 0;
		Dbf = 0;
		Dead = 0;
		pHits = 0;
		pKills = 0;
		MBHit = 0;
		All = 0L;
		MobDmg = 0L;
		AllHits = 0;
		AllMiss = 0;
		raidsCount = 0;
	}

	public static List<RaidDamage> copyList(List<RaidDamage> other)
	{
		List<RaidDamage> list = new List<RaidDamage>();
		foreach (RaidDamage item in other)
		{
			list.Add(new RaidDamage(item));
		}
		return list;
	}

	public void UpdateIcon()
	{
		SPCard sPCard = new SPCard();
		sPCard.UpdateSPCard(SPId);
		string value = ((Sex == 0) ? "male" : "female");
		string text = $"images\\portraits\\{SPId}_{value}{((sPCard != null && !sPCard.Shared) ? ("_" + ClassID) : "")}.png";
		icon = (File.Exists(text) ? Image.FromFile(text) : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
	}

	public async void UpdateRank(int new_rank)
	{
		if (new_rank != rank)
		{
			rank = new_rank;
			rankIcon = Analytics.GetRankIcon(rank);
			RaidDamage raidDamage = RaidManager.singleRaid.Find((RaidDamage x) => x.CharacterID == CharacterID);
			RaidDamage raidDamage2 = RaidManager.totalRaid.Find((RaidDamage x) => x.CharacterID == CharacterID);
			if (raidDamage != null)
			{
				raidDamage.rank = rank;
				raidDamage.rankIcon = rankIcon;
			}
			if (raidDamage2 != null)
			{
				raidDamage2.rank = rank;
				raidDamage2.rankIcon = rankIcon;
			}
			RaidForm.updateRankInList(this);
		}
	}
}
