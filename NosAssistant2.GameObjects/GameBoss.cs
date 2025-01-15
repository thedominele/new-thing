using System.Collections.Generic;

namespace NosAssistant2.GameObjects;

public class GameBoss
{
	public static readonly Dictionary<int, (string, string)> dict = new Dictionary<int, (string, string)>
	{
		{
			282,
			("Mother Cuby", "Cuby")
		},
		{
			284,
			("Ginseng", "Ginseng")
		},
		{
			285,
			("Dark Castra", "Castra")
		},
		{
			289,
			("Giant Black Spider", "Spider")
		},
		{
			286,
			("Massive Slade", "Slade")
		},
		{
			388,
			("Chicken King", "KFC")
		},
		{
			414,
			("Namaju", "Namaju")
		},
		{
			450,
			("Giant Grasslin", "Grasslin")
		},
		{
			533,
			("Huge Snowman Head", "Snowman")
		},
		{
			1028,
			("Ibrahim", "Ibrahim")
		},
		{
			1381,
			("Jack O'Lantern", "Jack")
		},
		{
			774,
			("Chicken Queen", "QFC")
		},
		{
			1500,
			("Captain Pete O'Peng", "Captain")
		},
		{
			1046,
			("Kertos the Demon Dog", "Kertos")
		},
		{
			1044,
			("Valakus King of Fire", "Valakus")
		},
		{
			1058,
			("Fire God Grenigas", "Grenigas")
		},
		{
			2034,
			("Lord Draco", "Draco")
		},
		{
			2049,
			("Glacerus the Ice Cold", "Glacerus")
		},
		{
			2305,
			("Caligor", "Caligor")
		},
		{
			2309,
			("Foxy", "Foxy")
		},
		{
			2316,
			("Maru", "Maru")
		},
		{
			2326,
			("Witch Laurena", "Laurena")
		},
		{
			2331,
			("Imp Hongbi", "Cheongbi")
		},
		{
			2332,
			("Imp Cheongbi", "Cheongbi")
		},
		{
			2357,
			("Lola Lopears", "Lola")
		},
		{
			2504,
			("Zenas", "Zenas")
		},
		{
			2514,
			("Erenia", "Erenia")
		},
		{
			2574,
			("Incomplete Fernon", "Fernon")
		},
		{
			2619,
			("Greedy Fafnir", "Fafnir")
		},
		{
			2639,
			("Twisted Yertirand", "Yertirand")
		},
		{
			2687,
			("Angry White Witch Laurena", "Angry Laurena")
		},
		{
			2685,
			("Mad Professor Macavity", "Mad Professor")
		},
		{
			2317,
			("Mad March Hare", "March Hare")
		},
		{
			3027,
			("Twisted Spirit King Kirollas", "Kirollas")
		},
		{
			3028,
			("Twisted Beast King Carno", "Carno")
		},
		{
			3029,
			("Demon God Belial", "Belial")
		},
		{
			3124,
			("Evil Overlord Paimon", "Paimon")
		},
		{
			563,
			("Lord Morcos", "Morcos")
		},
		{
			577,
			("Lord Hatus", "Hatus")
		},
		{
			629,
			("Lady Calvinas", "Calvinas")
		},
		{
			624,
			("Lord Berios", "Berios")
		},
		{
			2722,
			("Revenant Paimon", "Revenant Paimon")
		},
		{
			3217,
			("Zombie Dragon Valehir", "Valehir")
		},
		{
			3140,
			("Ice Dragon Alzanor", "Alzanor")
		},
		{
			2730,
			("Weak Asgobas", "Weak Asgobas")
		},
		{
			1540,
			("Moss Giant Pollutus", "Pollutus")
		},
		{
			1542,
			("Giant Arma", "Arma")
		},
		{
			1552,
			("Ultimate Giant Arma", "Ultimate Arma")
		},
		{
			979,
			("Giant Kenko Raider", "Giant Kenko Raider")
		},
		{
			724,
			("Strong Hell Knight", "Strong Hell Knight")
		},
		{
			583,
			("Strong Basilisk", "Strong Basilisk")
		},
		{
			725,
			("Strong Greylander", "Strong Greylander")
		},
		{
			748,
			("Weak Fire Devil", "Weak Fire Devil")
		},
		{
			637,
			("Strong Demon Dog Kertos", "Kertos")
		},
		{
			3218,
			("Dragon Lord Asgobas", "Asgobas")
		}
	};

	public int ID { get; set; } = -1;


	public string Name { get; set; } = "";


	public string FriendlyName { get; set; } = "";


	public static bool isBoss(int id)
	{
		return dict.ContainsKey(id);
	}

	public static string getBossName(int id)
	{
		if (!dict.TryGetValue(id, out (string, string) value))
		{
			return "";
		}
		return value.Item2;
	}

	public static string getRaidName(int id)
	{
		if (!dict.TryGetValue(id, out (string, string) value))
		{
			return "";
		}
		return value.Item1;
	}
}
