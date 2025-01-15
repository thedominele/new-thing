using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace NosAssistant2.GameObjects;

public class SPCard
{
	public static List<int> group_partners_buffs_list;

	public static List<int> self_partners_buffs_list;

	public static List<int> tattoos_list;

	public static List<int> debuffs;

	private static readonly Dictionary<int, (string Name, string Description, bool shared, List<int> buffsIDs, List<int> selfBuffsIDs)> Dict;

	public int ID { get; set; }

	public string Name { get; set; } = "";


	public string Description { get; set; } = "";


	public bool Shared { get; set; }

	public int SPUpgrade { get; set; } = -1;


	public int WingsID { get; set; } = -1;


	public List<int> BuffsIDs { get; set; } = new List<int>();


	public List<int> selfBuffsIDs { get; set; } = new List<int>();


	public static bool isGroupPartnerBuff(int skill_id)
	{
		return group_partners_buffs_list.Contains(skill_id);
	}

	public void UpdateSPCard(int sp_id, int sp_upgrade = 0, int sp_wings = 0)
	{
		ID = sp_id;
		SPUpgrade = sp_upgrade;
		WingsID = sp_wings;
		if (Dict.TryGetValue(sp_id, out (string, string, bool, List<int>, List<int>) value))
		{
			(Name, Description, Shared, BuffsIDs, selfBuffsIDs) = value;
		}
		else
		{
			Name = "Unknown";
			Description = "Unknown";
		}
	}

	public static Image? getSkillIcon(int skill_id)
	{
		if (Path.Exists($"images\\spells\\{skill_id}.png"))
		{
			return Image.FromFile($"images\\spells\\{skill_id}.png");
		}
		if (Path.Exists("images\\effects\\empty.png"))
		{
			return Image.FromFile("images\\effects\\empty.png");
		}
		return null;
	}

	public static Image? GetWingsIcon(string id)
	{
		if (!File.Exists("images/sp_wings/" + id + ".png"))
		{
			return null;
		}
		return Image.FromFile("images/sp_wings/" + id + ".png");
	}

	static SPCard()
	{
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 2);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 1482;
		num++;
		span[num] = 1489;
		num++;
		group_partners_buffs_list = list;
		List<int> list2 = new List<int>();
		CollectionsMarshal.SetCount(list2, 3);
		span = CollectionsMarshal.AsSpan(list2);
		num = 0;
		span[num] = 1782;
		num++;
		span[num] = 1783;
		num++;
		span[num] = 1784;
		num++;
		self_partners_buffs_list = list2;
		List<int> list3 = new List<int>();
		CollectionsMarshal.SetCount(list3, 33);
		span = CollectionsMarshal.AsSpan(list3);
		num = 0;
		span[num] = 671;
		num++;
		span[num] = 686;
		num++;
		span[num] = 687;
		num++;
		span[num] = 688;
		num++;
		span[num] = 689;
		num++;
		span[num] = 690;
		num++;
		span[num] = 691;
		num++;
		span[num] = 692;
		num++;
		span[num] = 693;
		num++;
		span[num] = 694;
		num++;
		span[num] = 695;
		num++;
		span[num] = 696;
		num++;
		span[num] = 697;
		num++;
		span[num] = 698;
		num++;
		span[num] = 699;
		num++;
		span[num] = 700;
		num++;
		span[num] = 701;
		num++;
		span[num] = 702;
		num++;
		span[num] = 703;
		num++;
		span[num] = 704;
		num++;
		span[num] = 705;
		num++;
		span[num] = 706;
		num++;
		span[num] = 707;
		num++;
		span[num] = 708;
		num++;
		span[num] = 709;
		num++;
		span[num] = 710;
		num++;
		span[num] = 711;
		num++;
		span[num] = 712;
		num++;
		span[num] = 713;
		num++;
		span[num] = 714;
		num++;
		span[num] = 715;
		num++;
		span[num] = 716;
		num++;
		span[num] = 755;
		num++;
		tattoos_list = list3;
		List<int> list4 = new List<int>();
		CollectionsMarshal.SetCount(list4, 13);
		span = CollectionsMarshal.AsSpan(list4);
		num = 0;
		span[num] = 907;
		num++;
		span[num] = 916;
		num++;
		span[num] = 892;
		num++;
		span[num] = 870;
		num++;
		span[num] = 920;
		num++;
		span[num] = 929;
		num++;
		span[num] = 953;
		num++;
		span[num] = 1329;
		num++;
		span[num] = 857;
		num++;
		span[num] = 942;
		num++;
		span[num] = 1073;
		num++;
		span[num] = 1082;
		num++;
		span[num] = 1087;
		num++;
		debuffs = list4;
		Dictionary<int, (string, string, bool, List<int>, List<int>)> dictionary = new Dictionary<int, (string, string, bool, List<int>, List<int>)>();
		List<int> list5 = new List<int>();
		CollectionsMarshal.SetCount(list5, 2);
		span = CollectionsMarshal.AsSpan(list5);
		num = 0;
		span[num] = 273;
		num++;
		span[num] = 272;
		num++;
		List<int> list6 = new List<int>();
		CollectionsMarshal.SetCount(list6, 1);
		span = CollectionsMarshal.AsSpan(list6);
		num = 0;
		span[num] = 271;
		num++;
		dictionary.Add(0, ("Norm", "Norm", false, list5, list6));
		dictionary.Add(1, ("Pyjama", "Pyjama", true, new List<int>(), new List<int>()));
		List<int> list7 = new List<int>();
		CollectionsMarshal.SetCount(list7, 1);
		span = CollectionsMarshal.AsSpan(list7);
		num = 0;
		span[num] = 819;
		num++;
		List<int> list8 = new List<int>();
		CollectionsMarshal.SetCount(list8, 1);
		span = CollectionsMarshal.AsSpan(list8);
		num = 0;
		span[num] = 813;
		num++;
		dictionary.Add(2, ("Warrior", "SP1W", false, list7, list8));
		List<int> item = new List<int>();
		List<int> list9 = new List<int>();
		CollectionsMarshal.SetCount(list9, 1);
		span = CollectionsMarshal.AsSpan(list9);
		num = 0;
		span[num] = 831;
		num++;
		dictionary.Add(3, ("Ninja", "SP2W", false, item, list9));
		List<int> item2 = new List<int>();
		List<int> list10 = new List<int>();
		CollectionsMarshal.SetCount(list10, 2);
		span = CollectionsMarshal.AsSpan(list10);
		num = 0;
		span[num] = 842;
		num++;
		span[num] = 834;
		num++;
		dictionary.Add(4, ("Ranger", "SP1A", false, item2, list10));
		List<int> item3 = new List<int>();
		List<int> list11 = new List<int>();
		CollectionsMarshal.SetCount(list11, 1);
		span = CollectionsMarshal.AsSpan(list11);
		num = 0;
		span[num] = 845;
		num++;
		dictionary.Add(5, ("Assassin", "SP2A", false, item3, list11));
		List<int> list12 = new List<int>();
		CollectionsMarshal.SetCount(list12, 1);
		span = CollectionsMarshal.AsSpan(list12);
		num = 0;
		span[num] = 861;
		num++;
		List<int> list13 = new List<int>();
		CollectionsMarshal.SetCount(list13, 1);
		span = CollectionsMarshal.AsSpan(list13);
		num = 0;
		span[num] = 858;
		num++;
		dictionary.Add(6, ("RedMagician", "SP1M", false, list12, list13));
		List<int> list14 = new List<int>();
		CollectionsMarshal.SetCount(list14, 4);
		span = CollectionsMarshal.AsSpan(list14);
		num = 0;
		span[num] = 874;
		num++;
		span[num] = 875;
		num++;
		span[num] = 873;
		num++;
		span[num] = 871;
		num++;
		List<int> list15 = new List<int>();
		CollectionsMarshal.SetCount(list15, 1);
		span = CollectionsMarshal.AsSpan(list15);
		num = 0;
		span[num] = 869;
		num++;
		dictionary.Add(7, ("HollyMage", "SP2M", false, list14, list15));
		dictionary.Add(8, ("Chicken", "Chicken", true, new List<int>(), new List<int>()));
		List<int> item4 = new List<int>();
		List<int> list16 = new List<int>();
		CollectionsMarshal.SetCount(list16, 1);
		span = CollectionsMarshal.AsSpan(list16);
		num = 0;
		span[num] = 885;
		num++;
		dictionary.Add(9, ("Jajamaru", "Jajamaru", true, item4, list16));
		List<int> list17 = new List<int>();
		CollectionsMarshal.SetCount(list17, 2);
		span = CollectionsMarshal.AsSpan(list17);
		num = 0;
		span[num] = 898;
		num++;
		span[num] = 897;
		num++;
		List<int> list18 = new List<int>();
		CollectionsMarshal.SetCount(list18, 1);
		span = CollectionsMarshal.AsSpan(list18);
		num = 0;
		span[num] = 893;
		num++;
		dictionary.Add(10, ("Crusader", "SP3W", false, list17, list18));
		List<int> item5 = new List<int>();
		List<int> list19 = new List<int>();
		CollectionsMarshal.SetCount(list19, 2);
		span = CollectionsMarshal.AsSpan(list19);
		num = 0;
		span[num] = 903;
		num++;
		span[num] = 909;
		num++;
		dictionary.Add(11, ("Berserker", "SP4W", false, item5, list19));
		List<int> item6 = new List<int>();
		List<int> list20 = new List<int>();
		CollectionsMarshal.SetCount(list20, 2);
		span = CollectionsMarshal.AsSpan(list20);
		num = 0;
		span[num] = 914;
		num++;
		span[num] = 918;
		num++;
		dictionary.Add(12, ("Destroyer", "SP3A", false, item6, list20));
		List<int> list21 = new List<int>();
		CollectionsMarshal.SetCount(list21, 3);
		span = CollectionsMarshal.AsSpan(list21);
		num = 0;
		span[num] = 928;
		num++;
		span[num] = 926;
		num++;
		span[num] = 931;
		num++;
		List<int> list22 = new List<int>();
		CollectionsMarshal.SetCount(list22, 1);
		span = CollectionsMarshal.AsSpan(list22);
		num = 0;
		span[num] = 925;
		num++;
		dictionary.Add(13, ("WildKeeper", "SP4A", false, list21, list22));
		List<int> list23 = new List<int>();
		CollectionsMarshal.SetCount(list23, 1);
		span = CollectionsMarshal.AsSpan(list23);
		num = 0;
		span[num] = 940;
		num++;
		List<int> list24 = new List<int>();
		CollectionsMarshal.SetCount(list24, 1);
		span = CollectionsMarshal.AsSpan(list24);
		num = 0;
		span[num] = 938;
		num++;
		dictionary.Add(14, ("BlueMagician", "SP3M", false, list23, list24));
		List<int> list25 = new List<int>();
		CollectionsMarshal.SetCount(list25, 1);
		span = CollectionsMarshal.AsSpan(list25);
		num = 0;
		span[num] = 949;
		num++;
		List<int> list26 = new List<int>();
		CollectionsMarshal.SetCount(list26, 1);
		span = CollectionsMarshal.AsSpan(list26);
		num = 0;
		span[num] = 947;
		num++;
		dictionary.Add(15, ("DarkGunner", "SP4M", false, list25, list26));
		dictionary.Add(16, ("Pirate", "Pirate", true, new List<int>(), new List<int>()));
		List<int> item7 = new List<int>();
		List<int> list27 = new List<int>();
		CollectionsMarshal.SetCount(list27, 1);
		span = CollectionsMarshal.AsSpan(list27);
		num = 0;
		span[num] = 1064;
		num++;
		dictionary.Add(17, ("Gladiator", "SP5W", false, item7, list27));
		List<int> item8 = new List<int>();
		List<int> list28 = new List<int>();
		CollectionsMarshal.SetCount(list28, 1);
		span = CollectionsMarshal.AsSpan(list28);
		num = 0;
		span[num] = 1076;
		num++;
		dictionary.Add(18, ("Cannoneer", "SP5A", false, item8, list28));
		List<int> list29 = new List<int>();
		CollectionsMarshal.SetCount(list29, 1);
		span = CollectionsMarshal.AsSpan(list29);
		num = 0;
		span[num] = 1083;
		num++;
		List<int> list30 = new List<int>();
		CollectionsMarshal.SetCount(list30, 1);
		span = CollectionsMarshal.AsSpan(list30);
		num = 0;
		span[num] = 1081;
		num++;
		dictionary.Add(19, ("Vulcano", "SP5M", false, list29, list30));
		dictionary.Add(20, ("BattleMonk", "SP6W", false, new List<int>(), new List<int>()));
		List<int> item9 = new List<int>();
		List<int> list31 = new List<int>();
		CollectionsMarshal.SetCount(list31, 1);
		span = CollectionsMarshal.AsSpan(list31);
		num = 0;
		span[num] = 1119;
		num++;
		dictionary.Add(21, ("Scout", "SP6A", false, item9, list31));
		List<int> list32 = new List<int>();
		CollectionsMarshal.SetCount(list32, 1);
		span = CollectionsMarshal.AsSpan(list32);
		num = 0;
		span[num] = 1105;
		num++;
		List<int> list33 = new List<int>();
		CollectionsMarshal.SetCount(list33, 1);
		span = CollectionsMarshal.AsSpan(list33);
		num = 0;
		span[num] = 1111;
		num++;
		dictionary.Add(22, ("TideLord", "SP6M", false, list32, list33));
		List<int> list34 = new List<int>();
		CollectionsMarshal.SetCount(list34, 1);
		span = CollectionsMarshal.AsSpan(list34);
		num = 0;
		span[num] = 1173;
		num++;
		dictionary.Add(23, ("DeathReaper", "SP7W", false, list34, new List<int>()));
		List<int> item10 = new List<int>();
		List<int> list35 = new List<int>();
		CollectionsMarshal.SetCount(list35, 1);
		span = CollectionsMarshal.AsSpan(list35);
		num = 0;
		span[num] = 1160;
		num++;
		dictionary.Add(24, ("DemonHunter", "SP7A", false, item10, list35));
		dictionary.Add(25, ("Seer", "SP7M", false, new List<int>(), new List<int>()));
		List<int> item11 = new List<int>();
		List<int> list36 = new List<int>();
		CollectionsMarshal.SetCount(list36, 2);
		span = CollectionsMarshal.AsSpan(list36);
		num = 0;
		span[num] = 1344;
		num++;
		span[num] = 1346;
		num++;
		dictionary.Add(26, ("Renegade", "SP8W", false, item11, list36));
		dictionary.Add(27, ("AvengingAngel", "SP8A", false, new List<int>(), new List<int>()));
		List<int> list37 = new List<int>();
		CollectionsMarshal.SetCount(list37, 1);
		span = CollectionsMarshal.AsSpan(list37);
		num = 0;
		span[num] = 1332;
		num++;
		List<int> list38 = new List<int>();
		CollectionsMarshal.SetCount(list38, 1);
		span = CollectionsMarshal.AsSpan(list38);
		num = 0;
		span[num] = 1328;
		num++;
		dictionary.Add(28, ("Archmage", "SP8M", false, list37, list38));
		List<int> list39 = new List<int>();
		CollectionsMarshal.SetCount(list39, 2);
		span = CollectionsMarshal.AsSpan(list39);
		num = 0;
		span[num] = 1585;
		num++;
		span[num] = 1590;
		num++;
		List<int> list40 = new List<int>();
		CollectionsMarshal.SetCount(list40, 1);
		span = CollectionsMarshal.AsSpan(list40);
		num = 0;
		span[num] = 1581;
		num++;
		dictionary.Add(29, ("DraconicFistA", "SP1MSWa", false, list39, list40));
		List<int> list41 = new List<int>();
		CollectionsMarshal.SetCount(list41, 1);
		span = CollectionsMarshal.AsSpan(list41);
		num = 0;
		span[num] = 1590;
		num++;
		dictionary.Add(30, ("DraconicFistB", "SP1MSWb", false, list41, new List<int>()));
		dictionary.Add(31, ("MysticArts", "SP2MSW", false, new List<int>(), new List<int>()));
		List<int> list42 = new List<int>();
		CollectionsMarshal.SetCount(list42, 1);
		span = CollectionsMarshal.AsSpan(list42);
		num = 0;
		span[num] = 631;
		num++;
		dictionary.Add(32, ("WeedingCostume", "WeedingCostume", true, list42, new List<int>()));
		List<int> item12 = new List<int>();
		List<int> list43 = new List<int>();
		CollectionsMarshal.SetCount(list43, 2);
		span = CollectionsMarshal.AsSpan(list43);
		num = 0;
		span[num] = 652;
		num++;
		span[num] = 654;
		num++;
		dictionary.Add(33, ("MasterWolf", "SP3MSW", false, item12, list43));
		List<int> list44 = new List<int>();
		CollectionsMarshal.SetCount(list44, 1);
		span = CollectionsMarshal.AsSpan(list44);
		num = 0;
		span[num] = 726;
		num++;
		dictionary.Add(34, ("DemonWarrior", "SP4MSW", false, list44, new List<int>()));
		List<int> item13 = new List<int>();
		List<int> list45 = new List<int>();
		CollectionsMarshal.SetCount(list45, 1);
		span = CollectionsMarshal.AsSpan(list45);
		num = 0;
		span[num] = 970;
		num++;
		dictionary.Add(35, ("Angler", "Angler", true, item13, list45));
		List<int> item14 = new List<int>();
		List<int> list46 = new List<int>();
		CollectionsMarshal.SetCount(list46, 1);
		span = CollectionsMarshal.AsSpan(list46);
		num = 0;
		span[num] = 970;
		num++;
		dictionary.Add(36, ("AnglerPremium", "AnglerPremium", true, item14, list46));
		dictionary.Add(37, ("ChefPremium", "ChefPremium", true, new List<int>(), new List<int>()));
		dictionary.Add(38, ("Chef", "Chef", true, new List<int>(), new List<int>()));
		List<int> item15 = new List<int>();
		List<int> list47 = new List<int>();
		CollectionsMarshal.SetCount(list47, 2);
		span = CollectionsMarshal.AsSpan(list47);
		num = 0;
		span[num] = 1383;
		num++;
		span[num] = 1386;
		num++;
		dictionary.Add(39, ("WaterfallBerserker", "SP9W", false, item15, list47));
		dictionary.Add(40, ("Sunchaser", "Sunchaser", false, new List<int>(), new List<int>()));
		List<int> item16 = new List<int>();
		List<int> list48 = new List<int>();
		CollectionsMarshal.SetCount(list48, 1);
		span = CollectionsMarshal.AsSpan(list48);
		num = 0;
		span[num] = 1646;
		num++;
		dictionary.Add(41, ("VoodooPriest", "SP9M", false, item16, list48));
		dictionary.Add(42, ("FlameDruidA", "SP9MSWa", false, new List<int>(), new List<int>()));
		dictionary.Add(43, ("FlameDruidB", "SP9MSWb", false, new List<int>(), new List<int>()));
		dictionary.Add(44, ("Unknown", "Unknown", false, new List<int>(), new List<int>()));
		List<int> item17 = new List<int>();
		List<int> list49 = new List<int>();
		CollectionsMarshal.SetCount(list49, 1);
		span = CollectionsMarshal.AsSpan(list49);
		num = 0;
		span[num] = 1730;
		num++;
		dictionary.Add(45, ("DragonKnight", "SP10W", false, item17, list49));
		dictionary.Add(46, ("Blaster", "SP10A", false, new List<int>(), new List<int>()));
		List<int> item18 = new List<int>();
		List<int> list50 = new List<int>();
		CollectionsMarshal.SetCount(list50, 2);
		span = CollectionsMarshal.AsSpan(list50);
		num = 0;
		span[num] = 1750;
		num++;
		span[num] = 1747;
		num++;
		dictionary.Add(47, ("Gravitas", "SP10M", false, item18, list50));
		dictionary.Add(48, ("HydraulicFist", "SP10MSW", false, new List<int>(), new List<int>()));
		dictionary.Add(49, ("PetTrainer", "PetTrainer", true, new List<int>(), new List<int>()));
		dictionary.Add(50, ("PetTrainerPremium", "PetTrainerPremium", true, new List<int>(), new List<int>()));
		dictionary.Add(51, ("StoneBreaker", "StoneBreaker", false, new List<int>(), new List<int>()));
		dictionary.Add(52, ("FogHunter", "FogHunter", false, new List<int>(), new List<int>()));
		dictionary.Add(53, ("FireStorm", "FireStorm", false, new List<int>(), new List<int>()));
		dictionary.Add(54, ("Thunderer", "Thunderer", false, new List<int>(), new List<int>()));
		Dict = dictionary;
	}
}
