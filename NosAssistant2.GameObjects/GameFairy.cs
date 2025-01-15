using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NosAssistant2.GameObjects;

public class GameFairy
{
	private static int NoElement = 0;

	private static int Fire = 1;

	private static int Water = 2;

	private static int Light = 3;

	private static int Shadow = 4;

	private static int All = 5;

	public static readonly Dictionary<int, GameFairy> dict = new Dictionary<int, GameFairy>
	{
		{
			0,
			new GameFairy
			{
				ID = 0,
				Name = "Normal Fairy",
				Element = NoElement,
				IsBoosted = false
			}
		},
		{
			1,
			new GameFairy
			{
				ID = 1,
				Name = "Chicken Fairy Full Shell",
				Element = NoElement,
				IsBoosted = false
			}
		},
		{
			2,
			new GameFairy
			{
				ID = 2,
				Name = "Chicken Fairy Half Shell",
				Element = NoElement,
				IsBoosted = false
			}
		},
		{
			3,
			new GameFairy
			{
				ID = 3,
				Name = "Chicken Fairy No Shell",
				Element = NoElement,
				IsBoosted = false
			}
		},
		{
			4,
			new GameFairy
			{
				ID = 4,
				Name = "Solaris",
				Element = Light,
				IsBoosted = false
			}
		},
		{
			5,
			new GameFairy
			{
				ID = 5,
				Name = "Sellaim",
				Element = Fire,
				IsBoosted = false
			}
		},
		{
			6,
			new GameFairy
			{
				ID = 6,
				Name = "Woondine",
				Element = Water,
				IsBoosted = false
			}
		},
		{
			7,
			new GameFairy
			{
				ID = 7,
				Name = "Eperial",
				Element = Light,
				IsBoosted = false
			}
		},
		{
			8,
			new GameFairy
			{
				ID = 8,
				Name = "Turik",
				Element = Shadow,
				IsBoosted = false
			}
		},
		{
			9,
			new GameFairy
			{
				ID = 9,
				Name = "Azuris",
				Element = Light,
				IsBoosted = false
			}
		},
		{
			10,
			new GameFairy
			{
				ID = 10,
				Name = "Sellaim Boosted",
				Element = Fire,
				IsBoosted = true
			}
		},
		{
			11,
			new GameFairy
			{
				ID = 11,
				Name = "Woondine Boosted",
				Element = Water,
				IsBoosted = true
			}
		},
		{
			12,
			new GameFairy
			{
				ID = 12,
				Name = "Eperial Boosted",
				Element = Light,
				IsBoosted = true
			}
		},
		{
			13,
			new GameFairy
			{
				ID = 13,
				Name = "Turik Boosted",
				Element = Shadow,
				IsBoosted = true
			}
		},
		{
			40,
			new GameFairy
			{
				ID = 40,
				Name = "Elkaim",
				Element = Fire,
				IsBoosted = false
			}
		},
		{
			41,
			new GameFairy
			{
				ID = 41,
				Name = "Ladine",
				Element = Water,
				IsBoosted = false
			}
		},
		{
			42,
			new GameFairy
			{
				ID = 42,
				Name = "Rumial",
				Element = Light,
				IsBoosted = false
			}
		},
		{
			43,
			new GameFairy
			{
				ID = 43,
				Name = "Varik",
				Element = Shadow,
				IsBoosted = false
			}
		},
		{
			45,
			new GameFairy
			{
				ID = 45,
				Name = "Elkaim Boosted",
				Element = Fire,
				IsBoosted = true
			}
		},
		{
			46,
			new GameFairy
			{
				ID = 46,
				Name = "Ladine Boosted",
				Element = Water,
				IsBoosted = true
			}
		},
		{
			47,
			new GameFairy
			{
				ID = 47,
				Name = "Rumial Boosted",
				Element = Light,
				IsBoosted = true
			}
		},
		{
			48,
			new GameFairy
			{
				ID = 48,
				Name = "Varik Boosted",
				Element = Shadow,
				IsBoosted = true
			}
		},
		{
			49,
			new GameFairy
			{
				ID = 49,
				Name = "Zenas",
				Element = All,
				IsBoosted = false
			}
		},
		{
			50,
			new GameFairy
			{
				ID = 50,
				Name = "Erenia",
				Element = All,
				IsBoosted = false
			}
		},
		{
			51,
			new GameFairy
			{
				ID = 51,
				Name = "Fernon",
				Element = All,
				IsBoosted = false
			}
		},
		{
			54,
			new GameFairy
			{
				ID = 54,
				Name = "Zenas Boosted",
				Element = All,
				IsBoosted = true
			}
		},
		{
			55,
			new GameFairy
			{
				ID = 55,
				Name = "Erenia Boosted",
				Element = All,
				IsBoosted = true
			}
		},
		{
			56,
			new GameFairy
			{
				ID = 56,
				Name = "Fernon Boosted",
				Element = All,
				IsBoosted = true
			}
		},
		{
			57,
			new GameFairy
			{
				ID = 57,
				Name = "Steam Drone Fire",
				Element = Fire,
				IsBoosted = false
			}
		},
		{
			58,
			new GameFairy
			{
				ID = 58,
				Name = "Steam Drone Water",
				Element = Water,
				IsBoosted = false
			}
		},
		{
			59,
			new GameFairy
			{
				ID = 59,
				Name = "Steam Drone Light",
				Element = Light,
				IsBoosted = false
			}
		},
		{
			60,
			new GameFairy
			{
				ID = 60,
				Name = "Steam Drone Shadow",
				Element = Shadow,
				IsBoosted = false
			}
		},
		{
			62,
			new GameFairy
			{
				ID = 62,
				Name = "Steam Drone Fire Boosted",
				Element = Fire,
				IsBoosted = true
			}
		},
		{
			63,
			new GameFairy
			{
				ID = 63,
				Name = "Steam Drone Water Boosted",
				Element = Water,
				IsBoosted = true
			}
		},
		{
			64,
			new GameFairy
			{
				ID = 64,
				Name = "Steam Drone Light Boosted",
				Element = Light,
				IsBoosted = true
			}
		},
		{
			65,
			new GameFairy
			{
				ID = 65,
				Name = "Steam Drone Shadow Boosted",
				Element = Shadow,
				IsBoosted = true
			}
		}
	};

	public static readonly Dictionary<(int, int), int> FairyIDToItemID = new Dictionary<(int, int), int>
	{
		{
			(0, Fire),
			800
		},
		{
			(0, Water),
			801
		},
		{
			(0, Light),
			802
		},
		{
			(0, Shadow),
			803
		},
		{
			(1, NoElement),
			255
		},
		{
			(2, NoElement),
			254
		},
		{
			(3, NoElement),
			256
		},
		{
			(4, Light),
			273
		},
		{
			(5, Fire),
			274
		},
		{
			(6, Water),
			275
		},
		{
			(7, Light),
			276
		},
		{
			(8, Shadow),
			277
		},
		{
			(9, Light),
			425
		},
		{
			(10, Fire),
			274
		},
		{
			(11, Water),
			275
		},
		{
			(12, Light),
			276
		},
		{
			(13, Shadow),
			277
		},
		{
			(40, Fire),
			4129
		},
		{
			(41, Water),
			4130
		},
		{
			(42, Light),
			4131
		},
		{
			(43, Shadow),
			4132
		},
		{
			(45, Fire),
			4129
		},
		{
			(46, Water),
			4130
		},
		{
			(47, Light),
			4131
		},
		{
			(48, Shadow),
			4132
		},
		{
			(49, Fire),
			4705
		},
		{
			(49, Water),
			4706
		},
		{
			(49, Light),
			4707
		},
		{
			(49, Shadow),
			4708
		},
		{
			(50, Fire),
			4709
		},
		{
			(50, Water),
			4710
		},
		{
			(50, Light),
			4711
		},
		{
			(50, Shadow),
			4712
		},
		{
			(51, Fire),
			4713
		},
		{
			(51, Water),
			4714
		},
		{
			(51, Light),
			4715
		},
		{
			(51, Shadow),
			4716
		},
		{
			(54, Fire),
			4705
		},
		{
			(54, Water),
			4706
		},
		{
			(54, Light),
			4707
		},
		{
			(54, Shadow),
			4708
		},
		{
			(55, Fire),
			4709
		},
		{
			(55, Water),
			4710
		},
		{
			(55, Light),
			4711
		},
		{
			(55, Shadow),
			4712
		},
		{
			(56, Fire),
			4713
		},
		{
			(56, Water),
			4714
		},
		{
			(56, Light),
			4715
		},
		{
			(56, Shadow),
			4716
		},
		{
			(57, Fire),
			8672
		},
		{
			(58, Water),
			8673
		},
		{
			(59, Light),
			8674
		},
		{
			(60, Shadow),
			8675
		},
		{
			(62, Fire),
			8672
		},
		{
			(63, Water),
			8673
		},
		{
			(64, Light),
			8674
		},
		{
			(65, Shadow),
			8675
		}
	};

	public static readonly Dictionary<int, int> ItemIDtoFairyID = new Dictionary<int, int>
	{
		{ 800, 0 },
		{ 801, 0 },
		{ 802, 0 },
		{ 803, 0 },
		{ 255, 1 },
		{ 254, 2 },
		{ 256, 3 },
		{ 273, 4 },
		{ 274, 5 },
		{ 275, 6 },
		{ 276, 7 },
		{ 277, 8 },
		{ 425, 9 },
		{ 4129, 40 },
		{ 4130, 41 },
		{ 4131, 42 },
		{ 4132, 43 },
		{ 4705, 49 },
		{ 4706, 49 },
		{ 4707, 49 },
		{ 4708, 49 },
		{ 4709, 50 },
		{ 4710, 50 },
		{ 4711, 50 },
		{ 4712, 50 },
		{ 4713, 51 },
		{ 4714, 51 },
		{ 4715, 51 },
		{ 4716, 51 },
		{ 8672, 57 },
		{ 8673, 58 },
		{ 8674, 59 },
		{ 8675, 60 }
	};

	public static readonly Dictionary<int, (string, bool)> IdToEffect = new Dictionary<int, (string, bool)>
	{
		{
			1,
			("Increases HP by", false)
		},
		{
			2,
			("Increases MP by", false)
		},
		{
			3,
			("Increases MP by", true)
		},
		{
			4,
			("Reduces critical damage by", true)
		},
		{
			5,
			("Reduces chance of suffering critical hits by", true)
		},
		{
			6,
			("All defence powers are increased by", false)
		},
		{
			7,
			("Increases critical damage by", true)
		},
		{
			8,
			("Experience gain is increased by", true)
		},
		{
			9,
			("*x* chance not to receive debuffs up to level 4", true)
		},
		{
			10,
			("Reduces damage taken in PvP by", true)
		},
		{
			11,
			("All defences are increased by", true)
		},
		{
			12,
			("All elemental resistances are increased by", false)
		},
		{
			13,
			("Increases champion XP by", true)
		},
		{
			14,
			("Increases the equipped fairy's element by", false)
		},
		{
			15,
			("All attacks are increased by", true)
		},
		{
			16,
			("Increases critical hit chance by", true)
		},
		{
			17,
			("All attacks are increased by", false)
		},
		{
			18,
			("Increases Gold earned by", true)
		},
		{
			19,
			("Reduces opponent's elemental resistances in PvP by", false)
		},
		{
			20,
			("Increases PvP attack power by", true)
		},
		{
			21,
			("Increases HP by", true)
		}
	};

	public static readonly Dictionary<int, (string, Color)> IdToEffectClass = new Dictionary<int, (string, Color)>
	{
		{
			1,
			("C", Color.Orange)
		},
		{
			2,
			("C", Color.Yellow)
		},
		{
			3,
			("B", Color.Green)
		},
		{
			4,
			("A", Color.Lime)
		},
		{
			5,
			("S", Color.Purple)
		}
	};

	public int ID { get; set; } = -1;


	public string Name { get; set; } = "";


	public int Element { get; set; } = -1;


	public bool IsBoosted { get; set; }

	public static bool IsFairyBoosted(int fairyId)
	{
		if (!dict.TryGetValue(fairyId, out GameFairy value))
		{
			return false;
		}
		return value.IsBoosted;
	}

	public static string IDToName(int fairyId)
	{
		if (!dict.TryGetValue(fairyId, out GameFairy value))
		{
			return "";
		}
		return value.Name;
	}

	public static bool isDrone(int item_id)
	{
		return new List<int> { 57, 58, 59, 60, 62, 63, 64, 65 }.Contains(item_id);
	}

	public static List<FairyEffect> StringToEffects(string effects_string)
	{
		List<FairyEffect> list = new List<FairyEffect>();
		foreach (string item4 in effects_string.Split(" ").ToList())
		{
			List<string> source = item4.Split(".").ToList();
			(string, bool) tuple = IdToEffect[Convert.ToInt32(source.ElementAt(1))];
			string item = tuple.Item1;
			bool item2 = tuple.Item2;
			FairyEffect item3 = new FairyEffect
			{
				effect_class = IdToEffectClass[Convert.ToInt32(source.ElementAt(0))].Item1,
				effect_color = IdToEffectClass[Convert.ToInt32(source.ElementAt(0))].Item2,
				effect = item,
				isPercent = item2,
				value = ((Convert.ToInt32(source.ElementAt(1)) > 2) ? Convert.ToInt32(source.ElementAt(2)) : (Convert.ToInt32(source.ElementAt(2)) * 10))
			};
			list.Add(item3);
		}
		return list;
	}
}
