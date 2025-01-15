using System.Collections.Generic;

namespace NosAssistant2.GameObjects;

public class GameSummon
{
	public static readonly Dictionary<int, GameSummon> dict = new Dictionary<int, GameSummon>
	{
		{
			OnyxID,
			new GameSummon
			{
				summon_id = OnyxID,
				name = "Onyx",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			2112,
			new GameSummon
			{
				summon_id = 2112,
				name = "SeerClone1",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			2113,
			new GameSummon
			{
				summon_id = 2113,
				name = "SeerClone2",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			2114,
			new GameSummon
			{
				summon_id = 2114,
				name = "SeerClone3",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			2115,
			new GameSummon
			{
				summon_id = 2115,
				name = "SeerClone4",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			2352,
			new GameSummon
			{
				summon_id = 2352,
				name = "ArchmageStarsType1",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			2353,
			new GameSummon
			{
				summon_id = 2353,
				name = "ArchmageStarsType2",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			989,
			new GameSummon
			{
				summon_id = 989,
				name = "VoodoSpiders",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			946,
			new GameSummon
			{
				summon_id = 946,
				name = "DestroyerMines",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			945,
			new GameSummon
			{
				summon_id = 945,
				name = "DestroyerBomb",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			416,
			new GameSummon
			{
				summon_id = 416,
				name = "MiniJajamaru",
				single_use = false,
				is_pet_summon = false
			}
		},
		{
			1437,
			new GameSummon
			{
				summon_id = 1437,
				name = "ScoutEagle",
				single_use = true,
				is_pet_summon = false
			}
		},
		{
			845,
			new GameSummon
			{
				summon_id = 845,
				name = "Mini Ninja Bushtail",
				single_use = true,
				is_pet_summon = true
			}
		},
		{
			2104,
			new GameSummon
			{
				summon_id = 2104,
				name = "Mini-Inferno",
				single_use = true,
				is_pet_summon = true
			}
		},
		{
			2341,
			new GameSummon
			{
				summon_id = 2341,
				name = "Aggressive Hongbi Clone",
				single_use = true,
				is_pet_summon = true
			}
		},
		{
			2342,
			new GameSummon
			{
				summon_id = 2342,
				name = "Aggressive Cheongbi Clone",
				single_use = true,
				is_pet_summon = true
			}
		},
		{
			2558,
			new GameSummon
			{
				summon_id = 2558,
				name = "Giant Mandra",
				single_use = false,
				is_pet_summon = true
			}
		}
	};

	public static int OnyxID = 2371;

	public int summon_id { get; set; } = -1;


	public string name { get; set; } = "";


	public int server_id { get; set; } = -1;


	public int owner_id { get; set; } = -1;


	public bool single_use { get; set; } = true;


	public bool is_pet_summon { get; set; } = true;


	public GameSummon()
	{
	}

	public GameSummon(int ID, int serverID, int ownerID)
	{
		GameSummon gameSummon = (dict.ContainsKey(ID) ? dict[ID] : null);
		if (gameSummon != null)
		{
			Copy(gameSummon);
			server_id = serverID;
			owner_id = ownerID;
		}
	}

	public void Copy(GameSummon other)
	{
		summon_id = other.summon_id;
		name = other.name;
		single_use = other.single_use;
		is_pet_summon = other.is_pet_summon;
	}

	public static bool isSummon(int id)
	{
		return dict.ContainsKey(id);
	}
}
