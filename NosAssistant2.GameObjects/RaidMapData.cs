using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NosAssistant2.GameObjects;

public static class RaidMapData
{
	private static readonly Dictionary<int, (string Name, List<(int x, int y)> Coords)> Dict = new Dictionary<int, (string, List<(int, int)>)>
	{
		{
			2501,
			("CastraMap1", new List<(int, int)>())
		},
		{
			2511,
			("CubyMap1", new List<(int, int)>())
		},
		{
			2521,
			("GinsengMap1", new List<(int, int)>
			{
				(62, 112),
				(61, 49),
				(53, 112),
				(83, 173),
				(155, 115),
				(121, 129),
				(121, 155),
				(80, 112),
				(81, 116),
				(84, 110),
				(85, 118),
				(88, 115)
			})
		},
		{
			2531,
			("SpiderMap1", new List<(int, int)>
			{
				(44, 66),
				(58, 77),
				(33, 100),
				(117, 90),
				(126, 122),
				(133, 122)
			})
		},
		{
			2541,
			("SladeMap1", new List<(int, int)>
			{
				(36, 73),
				(95, 34),
				(111, 35),
				(142, 34),
				(99, 112),
				(98, 131)
			})
		},
		{
			2591,
			("IbrahimMap1", new List<(int, int)>
			{
				(69, 27),
				(72, 56),
				(76, 56),
				(49, 207)
			})
		},
		{
			2592,
			("IbrahimMap2", new List<(int, int)>
			{
				(23, 137),
				(23, 131),
				(199, 208),
				(202, 204),
				(205, 206),
				(205, 211)
			})
		},
		{
			2537,
			("KertosMap1", new List<(int, int)>())
		},
		{
			2538,
			("KertosMap2", new List<(int, int)>())
		},
		{
			2554,
			("KertosMap1", new List<(int, int)>())
		},
		{
			2557,
			("ValakusMap1", new List<(int, int)>())
		},
		{
			2654,
			("RevenantPaimonMap1", new List<(int, int)>
			{
				(36, 72),
				(30, 78),
				(128, 3),
				(126, 5),
				(126, 2)
			})
		},
		{
			2655,
			("RevenantPaimonMap2", new List<(int, int)>
			{
				(60, 89),
				(48, 43),
				(135, 34),
				(135, 31)
			})
		},
		{
			2721,
			("ValehirMap1", new List<(int, int)>
			{
				(110, 108),
				(85, 69),
				(84, 58),
				(73, 33),
				(72, 31),
				(71, 32),
				(50, 168),
				(50, 166),
				(48, 167),
				(31, 69)
			})
		},
		{
			2719,
			("ValehirMap2", new List<(int, int)>())
		},
		{
			2720,
			("AlzanorMap1", new List<(int, int)>
			{
				(157, 47),
				(155, 46),
				(86, 81),
				(41, 76),
				(40, 79),
				(23, 35),
				(16, 33),
				(14, 30)
			})
		},
		{
			2718,
			("AlzanorMap2", new List<(int, int)>
			{
				(173, 87),
				(170, 79),
				(167, 78),
				(136, 7),
				(23, 43),
				(22, 98),
				(21, 38)
			})
		},
		{
			2722,
			("AsgobasMap1", new List<(int, int)>
			{
				(171, 174),
				(115, 196),
				(113, 198),
				(107, 103),
				(107, 101),
				(105, 99),
				(88, 89),
				(50, 225),
				(47, 226),
				(31, 14),
				(28, 30),
				(26, 29)
			})
		},
		{
			2723,
			("AsgobasMap2", new List<(int, int)>())
		},
		{
			2756,
			("PollutusMap1", new List<(int, int)>
			{
				(24, 97),
				(51, 108),
				(37, 30),
				(16, 14),
				(70, 55),
				(86, 48)
			})
		},
		{
			2757,
			("PollutusMap2", new List<(int, int)>())
		},
		{
			2754,
			("ArmaMap1", new List<(int, int)>())
		},
		{
			2755,
			("ArmaMap2", new List<(int, int)>())
		},
		{
			2759,
			("UltimateArmaMap1", new List<(int, int)>())
		}
	};

	public static int ID { get; set; } = -1;


	public static string Name { get; set; } = "";


	public static List<(int x, int y)> IgnoreButtons { get; set; } = new List<(int, int)>();


	public static void UpdateRaidData(int map_id)
	{
		ID = map_id;
		if (Dict.TryGetValue(map_id, out (string, List<(int, int)>) value))
		{
			(Name, IgnoreButtons) = value;
		}
		else
		{
			ID = -1;
			Name = "";
			IgnoreButtons = new List<(int, int)>();
		}
	}

	public static List<int> MobsToKill(int map_id)
	{
		switch (map_id)
		{
		case 2720:
		{
			List<int> list8 = new List<int>();
			CollectionsMarshal.SetCount(list8, 8);
			Span<int> span = CollectionsMarshal.AsSpan(list8);
			int num = 0;
			span[num] = 35;
			num++;
			span[num] = 50;
			num++;
			span[num] = 65;
			num++;
			span[num] = 75;
			num++;
			span[num] = 75;
			num++;
			span[num] = 75;
			num++;
			span[num] = 150;
			num++;
			span[num] = 150;
			num++;
			return list8;
		}
		case 2718:
		{
			List<int> list7 = new List<int>();
			CollectionsMarshal.SetCount(list7, 6);
			Span<int> span = CollectionsMarshal.AsSpan(list7);
			int num = 0;
			span[num] = 20;
			num++;
			span[num] = 60;
			num++;
			span[num] = 125;
			num++;
			span[num] = 170;
			num++;
			span[num] = 170;
			num++;
			span[num] = 170;
			num++;
			return list7;
		}
		case 2721:
		{
			List<int> list6 = new List<int>();
			CollectionsMarshal.SetCount(list6, 1);
			Span<int> span = CollectionsMarshal.AsSpan(list6);
			int num = 0;
			span[num] = 300;
			num++;
			return list6;
		}
		case 2754:
		{
			List<int> list5 = new List<int>();
			CollectionsMarshal.SetCount(list5, 4);
			Span<int> span = CollectionsMarshal.AsSpan(list5);
			int num = 0;
			span[num] = 20;
			num++;
			span[num] = 70;
			num++;
			span[num] = 110;
			num++;
			span[num] = 200;
			num++;
			return list5;
		}
		case 2755:
		{
			List<int> list4 = new List<int>();
			CollectionsMarshal.SetCount(list4, 3);
			Span<int> span = CollectionsMarshal.AsSpan(list4);
			int num = 0;
			span[num] = 45;
			num++;
			span[num] = 105;
			num++;
			span[num] = 150;
			num++;
			return list4;
		}
		case 2722:
		{
			List<int> list3 = new List<int>();
			CollectionsMarshal.SetCount(list3, 2);
			Span<int> span = CollectionsMarshal.AsSpan(list3);
			int num = 0;
			span[num] = 75;
			num++;
			span[num] = 240;
			num++;
			return list3;
		}
		case 2723:
		{
			List<int> list2 = new List<int>();
			CollectionsMarshal.SetCount(list2, 1);
			Span<int> span = CollectionsMarshal.AsSpan(list2);
			int num = 0;
			span[num] = 800;
			num++;
			return list2;
		}
		case 2759:
		{
			List<int> list = new List<int>();
			CollectionsMarshal.SetCount(list, 2);
			Span<int> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			span[num] = 120;
			num++;
			span[num] = 422;
			num++;
			return list;
		}
		default:
			return new List<int>();
		}
	}
}
