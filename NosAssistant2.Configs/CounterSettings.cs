namespace NosAssistant2.Configs;

public class CounterSettings
{
	public CounterColumnsSettings small { get; set; } = new CounterColumnsSettings
	{
		CLvl = true,
		Nickname = true,
		Family = false,
		Total = true,
		Special = true,
		MaxHit = false,
		MaxHitIcon = false,
		Pets = false,
		Average = false,
		Hit = true,
		Miss = false,
		Crit = false,
		Bon = false,
		BonCrit = false,
		OnyxDmg = false,
		Dead = true,
		Dbf = false,
		Gold = true,
		AttackPot = true,
		Booster = true,
		Tarot = true,
		RaidSpec = false,
		All = false,
		MobDmg = false,
		AllHits = false,
		AllMiss = false,
		Rank = false
	};


	public CounterColumnsSettings normal { get; set; } = new CounterColumnsSettings
	{
		CLvl = true,
		Nickname = true,
		Family = true,
		Total = true,
		Special = true,
		MaxHit = true,
		MaxHitIcon = true,
		Pets = false,
		Average = false,
		Hit = true,
		Miss = true,
		Crit = true,
		Bon = true,
		BonCrit = true,
		OnyxDmg = true,
		Dead = true,
		Dbf = false,
		Gold = true,
		AttackPot = true,
		Booster = true,
		Tarot = true,
		RaidSpec = true,
		All = false,
		MobDmg = false,
		AllHits = false,
		AllMiss = false,
		Rank = true
	};


	public CounterColumnsSettings large { get; set; } = new CounterColumnsSettings
	{
		CLvl = true,
		Nickname = true,
		Family = true,
		Total = true,
		Special = true,
		MaxHit = true,
		MaxHitIcon = true,
		Pets = true,
		Average = true,
		Hit = true,
		Miss = true,
		Crit = true,
		Bon = true,
		BonCrit = true,
		OnyxDmg = true,
		Dead = true,
		Dbf = true,
		Gold = true,
		AttackPot = true,
		Booster = true,
		Tarot = true,
		RaidSpec = true,
		All = true,
		MobDmg = true,
		AllHits = true,
		AllMiss = true,
		Rank = true
	};

}
