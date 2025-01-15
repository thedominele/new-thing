using System.Collections.Generic;
using NosAssistant2.GameObjects;

namespace NosAssistant2.Helpers;

public class GameMap
{
	public int ID { get; set; } = -1;


	public Dictionary<int, int> MobsList { get; set; } = new Dictionary<int, int>();


	public List<MapNPC> NPCsList { get; set; } = new List<MapNPC>();


	public List<GameTimeSpace> TimeSpacesList { get; set; } = new List<GameTimeSpace>();


	public List<int> adjacents { get; set; } = new List<int>();

}
