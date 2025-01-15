using System.Drawing;
using System.IO;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Dtos;

public class RaidPlayer
{
	public int lp { get; set; }

	public int character_id { get; set; }

	public string nickname { get; set; } = "";


	public string family { get; set; } = "";


	public int lvl { get; set; }

	public int clvl { get; set; }

	public long damage { get; set; }

	public long damage_miniboss { get; set; }

	public long damage_onyx { get; set; }

	public int max_hit { get; set; }

	public int max_hit_skill_id { get; set; }

	public int sp_id { get; set; } = -1;


	public int class_id { get; set; } = -1;


	public int sex { get; set; } = -1;


	public int pets { get; set; }

	public long gold { get; set; }

	public long average { get; set; }

	public int hit { get; set; }

	public int miss { get; set; }

	public int crit { get; set; }

	public int bon { get; set; }

	public int boncrit { get; set; }

	public int debuffs { get; set; }

	public int dead { get; set; }

	public int player_hits { get; set; }

	public int player_kills { get; set; }

	public long all_damage { get; set; }

	public long mob_damage { get; set; }

	public int all_hits { get; set; }

	public int all_miss { get; set; }

	public RaidPlayer()
	{
	}

	public RaidPlayer(RaidPlayer other)
	{
		lp = other.lp;
		character_id = other.character_id;
		nickname = other.nickname;
		family = other.family;
		lvl = other.lvl;
		clvl = other.clvl;
		damage = other.damage;
		damage_miniboss = other.damage_miniboss;
		damage_onyx = other.damage_onyx;
		max_hit = other.max_hit;
		max_hit_skill_id = other.max_hit_skill_id;
		sp_id = other.sp_id;
		class_id = other.class_id;
		pets = other.pets;
		gold = other.gold;
		average = other.average;
		hit = other.hit;
		miss = other.miss;
		crit = other.crit;
		bon = other.bon;
		boncrit = other.boncrit;
		debuffs = other.debuffs;
		dead = other.dead;
		player_hits = other.player_hits;
		player_kills = other.player_kills;
		all_damage = other.all_damage;
		mob_damage = other.mob_damage;
		all_hits = other.all_hits;
		all_miss = other.all_miss;
	}

	public Image? getPortrait()
	{
		SPCard sPCard = new SPCard();
		sPCard.UpdateSPCard(sp_id);
		string value = ((sex == 0) ? "male" : "female");
		string text = $"images\\portraits\\{sPCard.ID}_{value}{((sPCard != null && !sPCard.Shared) ? ("_" + class_id) : "")}.png";
		if (!File.Exists(text))
		{
			if (!File.Exists("images/npcs/empty.png"))
			{
				return null;
			}
			return Image.FromFile("images/npcs/empty.png");
		}
		return Image.FromFile(text);
	}
}
