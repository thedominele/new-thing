using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public static class NAvigator
{
	public static Dictionary<int, GameMap> game_world { get; set; } = new Dictionary<int, GameMap>();


	public static List<MobDto> world_monsters { get; set; } = new List<MobDto>();


	public static List<NPCDto> world_NPCs { get; set; } = new List<NPCDto>();


	public static List<MapDto> world_maps { get; set; } = new List<MapDto>();


	public static List<TimeSpaceDto> world_time_spaces { get; set; } = new List<TimeSpaceDto>();


	public static bool show_time_space_map { get; set; } = false;


	public static Bitmap? time_space_map { get; set; } = null;


	public static Bitmap? time_space_map_fresh_map { get; set; } = null;


	public static void AddMap(int map_id)
	{
		if (!game_world.ContainsKey(map_id))
		{
			game_world[map_id] = new GameMap
			{
				ID = map_id
			};
		}
	}

	public static void AddEdge(int from, int to)
	{
		AddMap(from);
		AddMap(to);
		if (!game_world[from].adjacents.Contains(to))
		{
			game_world[from].adjacents.Add(to);
		}
	}

	public static List<int> GetNeighbors(int map_id)
	{
		if (game_world.ContainsKey(map_id))
		{
			return game_world[map_id].adjacents;
		}
		return new List<int>();
	}

	public static void UpdateCurrentMapData(int id)
	{
		LoadGameWorld();
		AddMap(id);
		GameMap gameMap = ((!game_world.ContainsKey(id)) ? new GameMap
		{
			ID = id
		} : game_world[id]);
		foreach (GameEntity entity in GUI.entities)
		{
			if (!(entity.type_name != "NPC") && !gameMap.NPCsList.Any((MapNPC x) => x.ID == entity.id))
			{
				gameMap.NPCsList.Add(new MapNPC
				{
					ID = entity.id,
					destination_maps = new List<int>()
				});
			}
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (GameMonster monster in GUI.monsters)
		{
			if (!dictionary.ContainsKey(monster.id))
			{
				dictionary[monster.id] = 1;
			}
			else
			{
				dictionary[monster.id]++;
			}
		}
		foreach (int key in dictionary.Keys)
		{
			if (!gameMap.MobsList.ContainsKey(key))
			{
				gameMap.MobsList[key] = dictionary[key];
			}
			else if (gameMap.MobsList[key] < dictionary[key])
			{
				gameMap.MobsList[key] = dictionary[key];
			}
		}
		foreach (GameTimeSpace ts in GUI.time_spaces)
		{
			if (!gameMap.TimeSpacesList.Any((GameTimeSpace x) => x.ID == ts.ID))
			{
				gameMap.TimeSpacesList.Add(ts);
			}
		}
		foreach (GameEntity item in GUI.entities.Where((GameEntity x) => x.type_name == "portal").ToList())
		{
			AddEdge(gameMap.ID, item.portal_target_map_id);
		}
		SaveGameWorld();
	}

	public static void SaveGameWorld()
	{
		string contents = JsonConvert.SerializeObject(game_world, Formatting.Indented);
		File.WriteAllText("test\\game_world.json", contents);
	}

	public static void LoadGameWorld()
	{
		game_world = new Dictionary<int, GameMap>();
		string path = "test\\game_world.json";
		if (File.Exists(path))
		{
			game_world = JsonConvert.DeserializeObject<Dictionary<int, GameMap>>(File.ReadAllText(path));
		}
	}

	public static List<int> FindShortestPath(int source_map, int dest_id, string type)
	{
		if (game_world.Count == 0)
		{
			FetchGameWorld();
		}
		if (!game_world.ContainsKey(source_map))
		{
			return new List<int>();
		}
		if (type == "Raid")
		{
			if (!MapID.raid_boss_to_start_map.ContainsKey(dest_id))
			{
				return new List<int>();
			}
			dest_id = MapID.raid_boss_to_start_map[dest_id];
			type = "Map";
		}
		List<int> list = new List<int>();
		CollectionsMarshal.SetCount(list, 5);
		Span<int> span = CollectionsMarshal.AsSpan(list);
		int num = 0;
		span[num] = 4995;
		num++;
		span[num] = 4996;
		num++;
		span[num] = 4997;
		num++;
		span[num] = 4998;
		num++;
		span[num] = 20001;
		num++;
		List<int> list2 = list;
		Queue<int> queue = new Queue<int>();
		Dictionary<int, int?> dictionary = new Dictionary<int, int?>();
		queue.Enqueue(source_map);
		dictionary[source_map] = null;
		while (queue.Count > 0)
		{
			int num2 = queue.Dequeue();
			if (IsDestination(num2, dest_id, type))
			{
				List<int> list3 = new List<int>();
				while (num2 != source_map)
				{
					list3.Add(num2);
					num2 = dictionary[num2].Value;
				}
				list3.Add(source_map);
				list3.Reverse();
				return list3;
			}
			foreach (int adjacent in game_world[num2].adjacents)
			{
				if (!list2.Contains(adjacent) && !dictionary.ContainsKey(adjacent))
				{
					queue.Enqueue(adjacent);
					dictionary[adjacent] = num2;
				}
			}
		}
		return new List<int>();
	}

	private static bool IsDestination(int map_id, int dest_id, string type)
	{
		if (!game_world.ContainsKey(map_id))
		{
			return false;
		}
		GameMap gameMap = game_world[map_id];
		return type switch
		{
			"Map" => gameMap.ID == dest_id, 
			"Mob" => gameMap.MobsList.ContainsKey(dest_id), 
			"NPC" => gameMap.NPCsList.Any((MapNPC npc) => npc.ID == dest_id), 
			"TimeSpace" => gameMap.TimeSpacesList.Any((GameTimeSpace ts) => ts.ID == dest_id), 
			_ => throw new ArgumentException("Invalid type specified."), 
		};
	}

	public static async void FetchGameWorld()
	{
		game_world = new Dictionary<int, GameMap>();
		List<GameMapDto> list = await NAHttpClient.GetGameWorld();
		if (list == null)
		{
			return;
		}
		foreach (GameMapDto item in list)
		{
			if (!game_world.ContainsKey(item.mapId))
			{
				game_world[item.mapId] = new GameMap();
			}
			game_world[item.mapId].ID = item.mapId;
			game_world[item.mapId].adjacents = item.adjacents;
			foreach (GameMapTimeSpacesDto timeSpace in item.timeSpaces)
			{
				game_world[item.mapId].TimeSpacesList.Add(new GameTimeSpace
				{
					ID = timeSpace.timeSpaceId,
					x = timeSpace.x,
					y = timeSpace.y,
					min_lvl = timeSpace.minLvl,
					max_lvl = timeSpace.maxLvl,
					state = -1
				});
			}
			foreach (GameMapNPCsDto npc in item.npcs)
			{
				game_world[item.mapId].NPCsList.Add(new MapNPC
				{
					ID = npc.npcId,
					destination_maps = npc.destinationMaps
				});
			}
			foreach (GameMapMonsterDto monster in item.monsters)
			{
				game_world[item.mapId].MobsList[monster.vnum] = monster.mobCount;
			}
		}
		world_monsters = await NAHttpClient.GetAllMobsData();
		world_NPCs = await NAHttpClient.GetAllNPCsData();
		world_maps = await NAHttpClient.GetAllMapsData();
		world_time_spaces = await NAHttpClient.GetAllTSData();
	}
}
